
// WithMultiParameterBindingExtensions
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-19
// Version: 1.6

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

namespace Microsoft.Extensions.DependencyInjection;

public static class WithMultiParameterModelBindingExtensions
{
	/// <summary>
	/// Cleanup the MvcCore pipeline and add ValueProviders based on GenericValueProvider. Serialization options can be used.
	/// </summary>
	/// <param name="builder">IMvcCoreBuilder</param>
	/// <param name="jsonSerializerOptions">JsonSerializerOptions</param>
	/// <returns>IMvcCoreBuilder</returns>
	public static IMvcCoreBuilder WithMultiParameterModelBinding(this IMvcCoreBuilder builder, JsonSerializerOptions? jsonSerializerOptions = null)
	{
		return builder.AddMvcOptions(options =>
		{
			options.InputFormatters.RemoveType<SystemTextJsonInputFormatter>();

			//options.ModelValidatorProviders.Clear();
			//options.Conventions.Clear();
			//options.Filters.Clear();
			//options.ModelMetadataDetailsProviders.Clear();
			//options.ModelValidatorProviders.Clear();
			//options.ModelMetadataDetailsProviders.Clear();
			//options.OutputFormatters.Clear();

			if (jsonSerializerOptions == null)
				jsonSerializerOptions = new JsonSerializerOptions();


			jsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;

			// Correct Json output formatting
			jsonSerializerOptions.DictionaryKeyPolicy = null;
			jsonSerializerOptions.PropertyNamingPolicy = null;

			// Reading Json POST, Query, Header and Route providing models for a binder
			// All are using GenericValueProvider and can have jsonSerializerOptions for deserializing Models

			options.ValueProviderFactories.Clear();
			options.ValueProviderFactories.Add(new JsonValueProviderFactory(jsonSerializerOptions));
			options.ValueProviderFactories.Add(new HeaderValueProviderFactory(jsonSerializerOptions));
			options.ValueProviderFactories.Add(new CookyValueProviderFactory(jsonSerializerOptions));
			options.ValueProviderFactories.Add(new QueryStringValueProviderFactory(jsonSerializerOptions));
			options.ValueProviderFactories.Add(new RouteValueProviderFactory(jsonSerializerOptions));
			options.ValueProviderFactories.Add(new FormValueProviderFactory(jsonSerializerOptions));

			// Generic binder gettings complete de-serialized models of
			// GenericValueProvider
			options.ModelBinderProviders.Clear();
			options.ModelBinderProviders.Add(new GenericModelBinderProvider());

			// delete existing JsonOutputFormatter
			for (int i = options.OutputFormatters.Count - 1; i >= 0; i--)
			{
				var formatter = options.OutputFormatters[i];
				if (formatter.GetType() == typeof(SystemTextJsonOutputFormatter))
				{
					options.OutputFormatters.Remove(formatter);
				}
			}

			// add new JsonOutputFormatter
			options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(new JsonSerializerOptions()
			{
				TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
				DictionaryKeyPolicy = null,
				PropertyNamingPolicy = null
			}));

		});
	}
}
