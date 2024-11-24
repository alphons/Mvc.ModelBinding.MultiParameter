
// GenericValueProvider
// (C) 2022 Alphons van der Heijden
// Version: 1.2 Date: 2022-04-10
// Version: 1.3 Date: 2024-11-23

using System.Text.Json;
using System.ComponentModel;

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public class GenericValueProvider(
	BindingSource bindingSource, 
	JsonDocument? jsonDocument, 
	IFormCollection? formCollection, 
	JsonSerializerOptions? jsonSerializerOptions) : BindingSourceValueProvider(bindingSource)
{
	public override bool ContainsPrefix(string prefix)
	{
		//System.Diagnostics.Debug.WriteLine($"ContainsPrefix({prefix}) BIndingSource:{BindingSource.DisplayName}");

		if (jsonDocument != null &&
			jsonDocument.RootElement.ValueKind == JsonValueKind.Object &&
			jsonDocument.RootElement.TryGetProperty(prefix, out _))
			return true;

		if (formCollection != null)
		{
			if (formCollection.ContainsKey(prefix))
				return true;

			if (formCollection.Files != null &&
				formCollection.Files.Any(x => x.Name == prefix))
				return true;
		}

		return false;
	}

	/// <summary>
	/// Returns object of type when available
	/// </summary>
	/// <param name="key">name of the model</param>
	/// <param name="t">type of the model</param>
	/// <returns>null or object model of type</returns>
	public override object? GetModel(string key, Type t)
	{
		//System.Diagnostics.Debug.WriteLine($"GetModel({key}) BIndingSource:{BindingSource.DisplayName}");

		if (jsonDocument != null &&
			jsonDocument.RootElement.ValueKind == JsonValueKind.Object &&
			jsonDocument.RootElement.TryGetProperty(key, out JsonElement prop))
		{
			// this needs some tweaking!!!
			if (t.IsEnum && prop.ValueKind == JsonValueKind.String)
			{
				_ = Enum.TryParse(t, prop.GetString(), out object? result);

				return result;
			}
			else if (prop.ValueKind != JsonValueKind.Array || t.IsArray || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)))
			{
				return prop.Deserialize(t, jsonSerializerOptions);
			}
			else
			{
				var first = prop.EnumerateArray().FirstOrDefault();
				if (first.ValueKind != JsonValueKind.Null)
					return first.Deserialize(t, jsonSerializerOptions);
			}
		}

		if (formCollection != null)
		{
			if (formCollection.ContainsKey(key))
			{
				var model = TypeDescriptor.GetConverter(t).ConvertFrom(
				   context: null,
				   culture: System.Globalization.CultureInfo.InvariantCulture,
				   value: formCollection[key].FirstOrDefault() ?? new object()); // Needs some tweaking
				return model;
			}

			if (formCollection.Files != null && t == typeof(IFormFile))
				return formCollection.Files.FirstOrDefault(x => x.Name == key);
		}
		return null;
	}
}
