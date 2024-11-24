
using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public class JsonValueProviderFactory(JsonSerializerOptions? jsonSerializerOptions) : IValueProviderFactory
{
	public Task CreateValueProviderAsync(ValueProviderFactoryContext? context)
	{
		if (context?.ActionContext?.HttpContext?.Request is { Method: "POST" } request)
		{
			if ((request.ContentType == null || 
				request.ContentType.StartsWith("application/json"))
				&& (request.ContentLength == null || 
				request.ContentLength >= 2)) // matches an empty "{}" post
			{
				return AddValueProviderAsync(context);
			}
		}
		return Task.CompletedTask;
	}

	private async Task AddValueProviderAsync(ValueProviderFactoryContext? context)
	{
		if (context?.ActionContext?.HttpContext?.Request?.Body is not Stream body)
			return;

		try
		{
			var jsonDocument = await JsonDocument.ParseAsync(body);

			context.ValueProviders.Add(new GenericValueProvider(
				BindingSource.Body,
				jsonDocument: jsonDocument,
				formCollection: null,
				jsonSerializerOptions: jsonSerializerOptions));
		}
		catch (JsonException ex)
		{
			throw new ValueProviderException("Invalid JSON format.", ex);
		}
		catch (Exception ex)
		{
			throw new ValueProviderException("An error occurred while adding the value provider.", ex);
		}
	}
}
