using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;
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

