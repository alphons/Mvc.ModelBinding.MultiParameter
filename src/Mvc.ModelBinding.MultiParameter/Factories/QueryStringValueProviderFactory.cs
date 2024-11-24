using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public class QueryStringValueProviderFactory(JsonSerializerOptions? jsonSerializerOptions) : IValueProviderFactory
{
	/// <inheritdoc />
	public Task CreateValueProviderAsync(ValueProviderFactoryContext? context)
	{
		if (context?.ActionContext?.HttpContext?.Request?.Query is { Count: > 0 } query)
		{
			var queryDictionary = query.ToDictionary(
			pair => pair.Key,
			pair => pair.Value.Count > 1
				? pair.Value.ToArray()              // Convert multiple values to an array
				: (object)pair.Value.ToString());   // Use a string for a single value


			var jsonDocument = JsonDocument.Parse(
				JsonSerializer.Serialize(
					queryDictionary,
					jsonSerializerOptions));

			context.ValueProviders.Add(new GenericValueProvider(
				BindingSource.Query,
				jsonDocument,
				formCollection: null,
				jsonSerializerOptions: jsonSerializerOptions
			));
		}

		return Task.CompletedTask;
	}
}
