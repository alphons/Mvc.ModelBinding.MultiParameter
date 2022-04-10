
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public static class MvcCoreCorrectedExtensions
{
	/// <summary>
	/// Cleanup the MVC pipeline
	/// </summary>
	/// <param name="services">IServiceCollection</param>
	/// <returns>IMvcCoreBuilder</returns>
	public static IMvcCoreBuilder AddMvcCoreCorrected(this IServiceCollection services, bool CorrectDateTime = true)
	{
		return services.AddMvcCore().AddMvcOptions(options =>
		{
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

			var jsonOptions = new JsonSerializerOptions() { NumberHandling = JsonNumberHandling.AllowReadingFromString };
			//Reading Json POST, Query, Header and Route providing models for a binder

			options.ValueProviderFactories.Add(new JsonValueProviderFactory(jsonOptions));

			options.ValueProviderFactories.Add(new HeaderValueProviderFactory());
			options.ValueProviderFactories.Add(new CookyValueProviderFactory());

			options.ValueProviderFactories.Add(new QueryStringValueProviderFactory());
			options.ValueProviderFactories.Add(new RouteValueProviderFactory());
			options.ValueProviderFactories.Add(new FormFileValueProviderFactory());
			options.ValueProviderFactories.Add(new FormValueProviderFactory());	

			// Generic binder gettings complete de-serialized models of ModelProvider
			options.ModelBinderProviders.Add(new GenericModelBinderProvider());

			// Correct Json output formatting
			var jsonSerializerOptions = new JsonSerializerOptions()
			{
				DictionaryKeyPolicy = null,
				PropertyNamingPolicy = null
			};

			if (CorrectDateTime)
				jsonSerializerOptions.Converters.Add(new DateTimeConverter());

			options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(jsonSerializerOptions));
		});
	}

	private class DateTimeConverter : JsonConverter<DateTime>
	{
		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return new DateTime();// DateTime.Parse(reader.GetString());
		}

		public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToLocalTime().ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
		}
	}
}
