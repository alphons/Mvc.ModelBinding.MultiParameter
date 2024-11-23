
// CookyValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Version: 1.2 Date: 2022-04-10
// Version: 1.3 Date: 2024-11-23

using Mvc.ModelBinding.MultiParameter;
using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

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
		ArgumentNullException.ThrowIfNull(context);
		var cookies = context.ActionContext.HttpContext.Request.Cookies;
		if (cookies != null && cookies.Count > 0)
		{
			try
			{
				var cookieList = cookies
				.Select(cookie => $"\"{Helper.EscapeForJson(cookie.Key)}\": \"{Helper.EscapeForJson(cookie.Value)}\"")
				.ToArray();

				var json = $"{{{string.Join(',', cookieList)}}}";
				var jsonDocument = JsonDocument.Parse(json, options: default);

				var valueProvider = new GenericValueProvider(
					BindingSource.Special,
					jsonDocument,
					null,
					jsonSerializerOptions);

				context.ValueProviders.Add(valueProvider);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"[CookyValueProvider] Error parsing cookies: {ex.Message}");
			}
		}
		return Task.CompletedTask;
	}


}

