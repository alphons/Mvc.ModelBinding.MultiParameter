
using Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    // the 'real' root of the application
    ContentRootPath = AppDomain.CurrentDomain.BaseDirectory
});

builder.Services.AddMvcCore().WithMultiParameterModelBinding();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();



var app = builder.Build();

app.UseSession();

app.UseRouting();

app.MapControllers();

app.UseDefaultFiles();

app.UseStaticFiles();

app.Run();
