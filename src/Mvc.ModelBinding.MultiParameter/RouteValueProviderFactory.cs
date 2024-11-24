
using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public class RouteValueProviderFactory(JsonSerializerOptions? jsonSerializerOptions) : IValueProviderFactory
{
	/// <inheritdoc />
	public Task CreateValueProviderAsync(ValueProviderFactoryContext? context)
	{
		if (context?.ActionContext?.HttpContext?.Request?.RouteValues is { Count: > 0 } routeValues)
		{
			var jsonString = JsonSerializer.Serialize(routeValues, jsonSerializerOptions);

			var jsonDocument = JsonDocument.Parse(jsonString);

			context.ValueProviders.Add(new GenericValueProvider(
				BindingSource.Path,
				jsonDocument: jsonDocument,
				formCollection: null,
				jsonSerializerOptions: jsonSerializerOptions));
		}

		return Task.CompletedTask;
	}
}
