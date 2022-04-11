
// AddMvcCoreCorrected
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-11
// Version: 1.4

using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public static class MvcCoreCorrectedExtensions
{
	/// <summary>
	/// Cleanup the MVC pipeline and add ValueProviders based on GenericValueProvider
	/// </summary>
	/// <param name="services">IServiceCollection</param>
	/// <param name="jsonSerializerOptions">JsonSerializerOptions</param>
	/// <returns>IMvcCoreBuilder</returns>
	public static IMvcCoreBuilder AddMvcCoreCorrected(this IServiceCollection services, JsonSerializerOptions? jsonSerializerOptions = null)
	{
		return services.AddMvcCore().AddMvcOptions(options =>
		{
			options.EnableEndpointRouting = false;

			options.InputFormatters.Clear();
			options.ValueProviderFactories.Clear();
			options.ModelValidatorProviders.Clear();
			options.Conventions.Clear();
			options.Filters.Clear();
			options.ModelMetadataDetailsProviders.Clear();
			options.ModelValidatorProviders.Clear();
			options.ModelMetadataDetailsProviders.Clear();
			options.ModelBinderProviders.Clear();
			options.OutputFormatters.Clear();

			if (jsonSerializerOptions == null)
				jsonSerializerOptions = new JsonSerializerOptions();

			jsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;

			// Correct Json output formatting
			jsonSerializerOptions.DictionaryKeyPolicy = null;
			jsonSerializerOptions.PropertyNamingPolicy = null;

			// Reading Json POST, Query, Header and Route providing models for a binder
			// All are using GenericValueProvider and can have jsonSerializerOptions for deserializing Models

			options.ValueProviderFactories.Add(new JsonValueProviderFactory(jsonSerializerOptions));
			options.ValueProviderFactories.Add(new HeaderValueProviderFactory(jsonSerializerOptions));
			options.ValueProviderFactories.Add(new CookyValueProviderFactory(jsonSerializerOptions));
			options.ValueProviderFactories.Add(new QueryStringValueProviderFactory(jsonSerializerOptions));
			options.ValueProviderFactories.Add(new RouteValueProviderFactory(jsonSerializerOptions));
			options.ValueProviderFactories.Add(new FormValueProviderFactory(jsonSerializerOptions));

			// Generic binder gettings complete de-serialized models of
			// GenericValueProvider
			options.ModelBinderProviders.Add(new GenericModelBinderProvider());

			options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(jsonSerializerOptions));
		});
	}
}
