
using Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvcCoreCorrected();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();



var app = builder.Build();

app.UseSession();

app.UseRouting();

app.MapControllers();

app.UseDefaultFiles();

app.UseStaticFiles();

app.Run();
