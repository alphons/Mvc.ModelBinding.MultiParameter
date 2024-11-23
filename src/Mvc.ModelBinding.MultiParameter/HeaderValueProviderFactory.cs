
// HeaderValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-10
// Version: 1.2


using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public class HeaderValueProviderFactory : IValueProviderFactory
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;
	public HeaderValueProviderFactory(JsonSerializerOptions? Options)
	{
		this.jsonSerializerOptions = Options;
	}
	public HeaderValueProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}
	public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		var headers = context.ActionContext.HttpContext.Request.Headers;
		if (headers != null && headers.Count > 0)
		{
			var json = JsonSerializer.Serialize(headers);

			var jsonDocument = JsonDocument.Parse(json, options: default);

			var valueProvider = new GenericValueProvider(
				BindingSource.Header,
				jsonDocument,
				null,
				jsonSerializerOptions);

			context.ValueProviders.Add(valueProvider);
		}

		return Task.CompletedTask;
	}
}


