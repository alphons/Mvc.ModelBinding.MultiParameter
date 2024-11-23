using System.Text.RegularExpressions;

namespace Mvc.ModelBinding.MultiParameter;

internal partial class Helper
{
	[GeneratedRegex("[\"\\\\\b\f\n\r\t]")]
	private static partial Regex MyRegex();
	public static string EscapeForJson(string? value)
	{
		if (string.IsNullOrEmpty(value))
			return string.Empty;

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
}
