consists# Mvc.ModelBinding.MultiParameter

Calling controller methods using multiple parameter binding.

```c#
[HttpPost]
[Route("~/api/SomeMethod/{SomeParameter2}")]
public async Task<IActionResult> SomeMethod(
	[FromCooky(Name = ".AspNetCore.Session")] string SomeParameter0, // #######
	[FromHeader(Name = "Referer")] string SomeParameter1,	// "https://localhost:44346/"
	[FromRoute] string SomeParameter2,			// "two"
	[FromQuery] string SomeParameter3,			// "three"
	[FromBody] ApiModel SomeParameter4,			//  {four}
	[FromBody] string SomeParameter5,			//  "five" (multi binding FromBody)
	[FromQuery]string SomeParameter6)			//  "six" (multi binding FromQuery)
{
	await Task.Yield();
	return Ok();
}
```
And if parameter names are unique this can be simplified to:
```c#
[HttpPost]
[Route("~/api/OtherMethod/{SomeParameter2}")]
public async Task<IActionResult> OtherMethod(
	string Referer,			// "https://localhost:44346/"
	string SomeParameter2,		// "two"
	string SomeParameter3,		// "three"
	ApiModel SomeParameter4,	//  {four}
	string SomeParameter5,		//  "five" (multi binding FromBody)
	string SomeParameter6)		//  "six" (multi binding FromQuery)
{
	await Task.Yield();
	return Ok();
}
```

This test uses `netproxy` javascript caller for posting Json to controllers.

```javascript
r = await netproxyasync("./api/DemoProposal/two?SomeParameter3=three&SomeParameter6=six",
{
  "SomeParameter4": // Now the beast has a name
  {
    Name: "four",
    "Users":
    [
      [{ Name: "User00", Alias: ['aliasa', 'aliasb', 'aliasc'] }, { Name: "User01" }],
      [{ Name: "User10" }, { Name: "User11" }],
      [{ Name: "User20" }, { Name: "User21" }]
    ]
  },
  "SomeParameter5": "five" // double binder
});
```

For using the multi parameter binding use the `AddMvcCoreCorrected` extension, complete Program.cs:

```c#
using Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

var builder = WebApplication.CreateBuilder();

builder.Services.AddMvcCore().WithMultiParameterModelBinding();

var app = builder.Build();
app.UseSession();
app.UseRouting();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();
```
The extension `WithMultiParameterModelBinding()` consists of:
```c#
public static IMvcCoreBuilder WithMultiParameterModelBinding(this IMvcCoreBuilder builder, JsonSerializerOptions? jsonSerializerOptions = null)
{
	return builder.AddMvcOptions(options =>
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
```
Every ValueProviderFactory deserializes by `JsonSerializerOptions`.


