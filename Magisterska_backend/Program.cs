using Magisterska_backend.Models;
using Magisterska_backend.Services;
using Magisterska_backend.Services.Interfaces;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0",
        Title = "Your API",
        Description = "API Documentation"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000");
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                          policy.AllowCredentials();
                      });
});


builder.Services.AddSingleton<IWebSocketService, WebSocketService>();
builder.Services.AddSingleton<IMQTTService, MQTTService>();
builder.Services.AddSingleton<IInfluxDBService, InfluxDBService>();
builder.Services.Configure<InfluxDbSettings>(builder.Configuration.GetSection("InfluxDbSettings"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
});


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<MatrixHub>("/matrixhub");
    endpoints.MapControllers();
});

var mqttService = app.Services.GetRequiredService<IMQTTService>();
mqttService.StartAsync().GetAwaiter().GetResult();

app.Run();
