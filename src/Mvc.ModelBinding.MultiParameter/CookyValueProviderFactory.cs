
// CookyValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-10
// Version: 1.2

using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

#nullable enable
public class CookyValueProviderFactory : IValueProviderFactory
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;
	public CookyValueProviderFactory(JsonSerializerOptions? Options)
	{
		this.jsonSerializerOptions = Options;
	}
	public CookyValueProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}
	public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}
		var cookies = context.ActionContext.HttpContext.Request.Cookies;
		if (cookies != null && cookies.Count > 0)
		{
			var list = cookies.Select(x => $"\"{x.Key}\": \"{x.Value}\"").ToArray();
			var json = $"{{{string.Join(',', list)}}}";
			var jsonDocument = JsonDocument.Parse(json, options: default);

			var valueProvider = new GenericValueProvider(
				BindingSource.Special,
				jsonDocument,
				null,
				jsonSerializerOptions);

			context.ValueProviders.Add(valueProvider);
		}
		return Task.CompletedTask;
	}
}

