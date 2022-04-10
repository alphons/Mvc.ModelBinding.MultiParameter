
// JsonValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-10
// Version: 1.2

using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

#nullable enable
public class JsonValueProviderFactory : IValueProviderFactory
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;
	public JsonValueProviderFactory(JsonSerializerOptions Options)
	{
		this.jsonSerializerOptions = Options;
	}
	public JsonValueProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}
	public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		var request = context.ActionContext.HttpContext.Request;

		if (request.Method == "POST")
		{
			if (request.ContentType == null || request.ContentType.StartsWith("application/json"))
			{
				if (request.ContentLength == null || // Chunked encoding
					request.ContentLength >= 2) // Normal encoding, using content length minimum '{}'
				{
					return AddValueProviderAsync(context);
				}
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

		var valueProvider = new GenericValueProvider(
			BindingSource.Body,
			jsonDocument,
			null,
			jsonSerializerOptions);

		context.ValueProviders.Add(valueProvider);
	}
}
