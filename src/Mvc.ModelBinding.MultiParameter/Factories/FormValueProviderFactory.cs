using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

/// <summary>
/// A <see cref="IValueProviderFactory"/> for <see cref="GenericValueProvider"/>.
/// </summary>
public class FormValueProviderFactory(JsonSerializerOptions? jsonSerializerOptions) : IValueProviderFactory
{
	/// <inheritdoc />
	public Task CreateValueProviderAsync(ValueProviderFactoryContext? context)
	{
		if (context?.ActionContext?.HttpContext?.Request is { HasFormContentType: true } request)
		{
			context.ValueProviders.Add(new GenericValueProvider(
				BindingSource.Form,
				jsonDocument: null,
				formCollection: request.Form,
				jsonSerializerOptions: jsonSerializerOptions));
		}

		return Task.CompletedTask;
	}

}

