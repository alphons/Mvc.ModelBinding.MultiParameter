consists# Mvc.ModelBinding.MultiParameter

Calling controller methods using multiple parameter binding.

```c#
[HttpPost]
[Route("~/api/DemoProposal/{SomeParameter2}")]
public async Task<IActionResult> DemoProposal(
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
[Route("~/api/DemoProposal2/{SomeParameter2}")]
public async Task<IActionResult> DemoProposal(
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

builder.Services.AddMvcCoreCorrected();

var app = builder.Build();
app.UseSession();
app.UseRouting();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();
```
The extension `AddMvcCoreCorrected()` consists of:
```c#
builder.Services.AddMvcCore().AddMvcOptions(options =>
{
  // Clear out all legacy stuff
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

  // Adding our new custom valueproviders (all implementing GetModel)
  options.ValueProviderFactories.Add(new JsonValueProviderFactory());
  options.ValueProviderFactories.Add(new HeaderValueProviderFactory());
  options.ValueProviderFactories.Add(new CookyValueProviderFactory());

  options.ValueProviderFactories.Add(new QueryStringValueProviderFactory());
  options.ValueProviderFactories.Add(new RouteValueProviderFactory());
  options.ValueProviderFactories.Add(new FormFileValueProviderFactory());
  options.ValueProviderFactories.Add(new FormValueProviderFactory());	

  // One Generic binder getting complete de-serialized parameters
  // calling GetModel(name,type) on the new value-providers.
  options.ModelBinderProviders.Add(new GenericModelBinderProvider());
}
```
Every ValueProviderFactory can have its own `JsonSerializerOptions`.


