using System.Text.Json;
using System.ComponentModel;

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public interface IGenericValueProvider : IValueProvider
{
	IValueProvider? Filter(BindingSource bindingSource);

	object? GetModel(string key, Type t);
}

public class GenericValueProvider(
	BindingSource bindingSource,
	JsonDocument? jsonDocument,
	IFormCollection? formCollection,
	JsonSerializerOptions? jsonSerializerOptions) : IGenericValueProvider
{
	public ValueProviderResult GetValue(string key) =>
		ValueProviderResult.None;

	public IValueProvider? Filter(BindingSource bindingSourceFilter) =>
		bindingSource.CanAcceptDataFrom(bindingSourceFilter) ? this : null;

	public bool ContainsPrefix(string prefix)
	{
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
	public object? GetModel(string key, Type t)
	{
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

			if (formCollection.Files != null)
			{
				if (t == typeof(IFormFile))
					return formCollection.Files.FirstOrDefault(x => x.Name == key);
				else
					return formCollection.Files;
			}
		}
		return null;
	}
}
