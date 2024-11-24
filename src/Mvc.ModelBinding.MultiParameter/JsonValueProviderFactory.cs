
// JsonValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Version: 1.2 Date: 2022-04-10
// Version: 1.3 Date: 2024-11-23

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

	private async Task AddValueProviderAsync(ValueProviderFactoryContext context)
	{
		var request = context.ActionContext.HttpContext.Request;
		JsonDocument jsonDocument;
		try
		{
			jsonDocument = await JsonDocument.ParseAsync(request.Body);
		}
		catch (JsonException ex)
		{
			// ParseAsync can throw JsonException if the stream is no json element.
			// Wrap it in a ValueProviderException that the CompositeValueProvider special cases.
			throw new ValueProviderException(ex.Message, ex);
		}
		catch (Exception ex)
		{
			// Wrap it in a ValueProviderException that the CompositeValueProvider special cases.
			throw new ValueProviderException(ex.Message, ex);
		}

		context.ValueProviders.Add(new GenericValueProvider(
			BindingSource.Body,
			jsonDocument: jsonDocument,
			formCollection: null,
			jsonSerializerOptions: jsonSerializerOptions));
	}
}
