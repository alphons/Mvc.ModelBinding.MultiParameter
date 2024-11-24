using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Mvc.ModelBinding.MultiParameter.Testing
{
	public class CustomFormValueProviderFactory : IValueProviderFactory
	{
		public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
		{
			ArgumentNullException.ThrowIfNull(context);

			// Check if the request is of the correct content type
			if (context.ActionContext.HttpContext.Request.ContentType == "application/x-www-form-urlencoded" ||
				context.ActionContext.HttpContext.Request.ContentType.StartsWith("multipart/form-data"))
			{
				var formCollection = context.ActionContext.HttpContext.Request.Form;
				var valueProvider = new FormValueProvider(
					BindingSource.Form,
					formCollection,
					CultureInfo.CurrentCulture
				);

				context.ValueProviders.Add(valueProvider);
			}

			return Task.CompletedTask;
		}
	}
}
