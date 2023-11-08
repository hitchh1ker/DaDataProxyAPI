using DaDataProxyAPI.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ProxySettings>(builder.Configuration.GetSection("DaDataConfig"));

builder.Services.AddHttpClient();

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();

app.MapControllers();

app.Run();
