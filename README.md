# Mvc.ModelBinding.MultiParameter

Nuget package https://www.nuget.org/packages/Mvc.ModelBinding.MultiParameter/

Calling controller methods using multiple unique parameters.

```c#
[HttpPost("~/api/SomeMethod/{SomeParameter2}")]
public IActionResult SomeMethod(
	string Referer,			    // "https://localhost:44346/"
	string SomeParameter2,		// "two"
	string SomeParameter3,		// "three"
	ApiModel SomeParameter4,	//  {four}
	double SomeParameter5,		//  5.5 (multi binding FromBody)
	int SomeParameter6)		    //  6 (multi binding FromQuery)
{
	return Ok();
}
```
Using attributes to emphasize the source:
```c#
[HttpPost]
[Route("~/api/OtherMethod/{SomeParameter2}")]
public IActionResult OtherMethod(
	[FromCooky(Name = ".AspNetCore.Session")] string SomeParameter0, // #######
	[FromHeader(Name = "Referer")] string SomeParameter1,	// "https://localhost:44346/"
	[FromRoute] string SomeParameter2,	// "two"
	[FromQuery] string SomeParameter3,	// "three"
	[FromBody] ApiModel SomeParameter4,	//  {four}
	[FromBody] double SomeParameter5,	//  5.5 (multi binding FromBody)
	[FromQuery] int SomeParameter6)		//  6 (multi binding FromQuery)
{
	return Ok();
}

This test uses `netproxy` javascript caller for posting Json to controllers.

```javascript
r = await netproxyasync("./api/SomeMethod/two?SomeParameter3=three&SomeParameter6=6",
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
  "SomeParameter5": 5.5 // double binder
});
```

For the multi parameter binding use the `WithMultiParameterModelBinding`.
Example Program.cs:

```c#
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = AppDomain.CurrentDomain.BaseDirectory
});

builder.Services.AddMvcCore().WithMultiParameterModelBinding();

var app = builder.Build();
app.UseRouting();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();
```





