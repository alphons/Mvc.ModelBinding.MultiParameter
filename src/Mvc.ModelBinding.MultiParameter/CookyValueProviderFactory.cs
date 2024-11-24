
// CookyValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Version: 1.2 Date: 2022-04-10
// Version: 1.3 Date: 2024-11-23

using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public class CookyValueProviderFactory(JsonSerializerOptions? jsonSerializerOptions) : IValueProviderFactory
{
	public Task CreateValueProviderAsync(ValueProviderFactoryContext? context)
	{
		if (context?.ActionContext?.HttpContext?.Request?.Cookies is { Count: > 0 } cookies)
		{
			var jsonString = JsonSerializer.Serialize(
				cookies.ToDictionary(c => c.Key, c => c.Value), 
				jsonSerializerOptions);

			var jsonDocument = JsonDocument.Parse(jsonString);

			context.ValueProviders.Add(new GenericValueProvider(
				BindingSource.Special,
				jsonDocument: jsonDocument,
				formCollection: null,
				jsonSerializerOptions: jsonSerializerOptions));
		}
		return Task.CompletedTask;
	}


}

