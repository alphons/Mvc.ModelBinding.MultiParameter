using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public class HeaderValueProviderFactory(JsonSerializerOptions? jsonSerializerOptions) : IValueProviderFactory
{
	public Task CreateValueProviderAsync(ValueProviderFactoryContext? context)
	{
		if (context?.ActionContext?.HttpContext?.Request?.Headers is { Count: > 0 } headers)
		{
			var json = JsonSerializer.Serialize(headers, jsonSerializerOptions);

			var jsonDocument = JsonDocument.Parse(json);

			context.ValueProviders.Add(new GenericValueProvider(
				BindingSource.Header,
				jsonDocument: jsonDocument,
				formCollection: null,
				jsonSerializerOptions: jsonSerializerOptions));
		}

		return Task.CompletedTask;
	}
}


