
// CookyValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Version: 1.2 Date: 2022-04-10
// Version: 1.3 Date: 2024-11-23

using System.Text.Json;
using System.Text.RegularExpressions;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public partial class CookyValueProviderFactory : IValueProviderFactory
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

	[GeneratedRegex("[\"\\\\\b\f\n\r\t]")]
	private static partial Regex MyRegex();
	private static string EscapeForJson(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return string.Empty;
		}

		return MyRegex().Replace(value, match =>
		{
			return match.Value switch
			{
				"\"" => "\\\"",
				"\\" => "\\\\",
				"\b" => "\\b",
				"\f" => "\\f",
				"\n" => "\\n",
				"\r" => "\\r",
				"\t" => "\\t",
				_ => match.Value
			};
		});
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
				.Select(cookie => $"\"{EscapeForJson(cookie.Key)}\": \"{EscapeForJson(cookie.Value)}\"")
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

