

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    // the 'real' root of the application
    ContentRootPath = AppDomain.CurrentDomain.BaseDirectory
});

builder.Services.AddMvcCore().WithMultiParameterModelBinding(SanitizeAll: true);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();



var app = builder.Build();

app.UseRouting();

app.UseSession();

app.UseDefaultFiles();

app.UseStaticFiles();

app.MapControllers();

app.Run();
