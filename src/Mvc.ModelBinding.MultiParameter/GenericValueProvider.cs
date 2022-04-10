using Microsoft.AspNetCore.Http;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

// JsonModelProviderFactory, JsonModelProvider
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-04
// Version: 1.0

using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

#nullable enable

public class GenericValueProvider : BindingSourceValueProvider
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;
	private readonly JsonDocument? jsonDocument;
	private readonly IFormCollection? form;

	public GenericValueProvider(BindingSource bindingSource, JsonDocument? jsonDocument, IFormCollection? form, JsonSerializerOptions? options) : base(bindingSource)
	{
		this.jsonSerializerOptions = options;

		this.jsonDocument = jsonDocument;

		this.form = form;
	}

	public override bool ContainsPrefix(string prefix)
	{
		Debug.WriteLine($"ContainsPrefix({prefix})");

		if (jsonDocument != null && jsonDocument.RootElement.ValueKind == JsonValueKind.Object)
			return jsonDocument.RootElement.TryGetProperty(prefix, out _);

		if (this.form != null)
		{
			if (this.form.ContainsKey(prefix))
				return true;

			if (this.form.Files != null)
				return this.form.Files.Any(x => x.Name == prefix);
		}

		return false;
	}

	public override ValueProviderResult GetValue(string key)
	{
		return ValueProviderResult.None;
	}

	/// <summary>
	/// Returns object of type when available
	/// </summary>
	/// <param name="key">name of the model</param>
	/// <param name="t">type of the model</param>
	/// <returns>null or object model of type</returns>
	public override object? GetModel(string key, Type t)
	{
		Debug.WriteLine($"GetModel({key})");

		if (jsonDocument != null)
		{
			var prop = jsonDocument.RootElement.GetProperty(key);
			// this needs some tweaking!!!
			if (prop.ValueKind != JsonValueKind.Array || t.IsArray || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)))
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

		if(this.form != null)
		{
			if (this.form.ContainsKey(key))
			{
				var model = TypeDescriptor.GetConverter(t).ConvertFrom(
				   context: null,
				   culture: System.Globalization.CultureInfo.InvariantCulture,
				   value: this.form[key][0]); // Needs some tweaking
				return model;
			}

			if (this.form.Files != null && t == typeof(IFormFile))
				return this.form.Files.FirstOrDefault(x => x.Name == key);
		}

		return null;
	}

}


