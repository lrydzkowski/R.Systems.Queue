using R.Systems.Queue.Core;
using R.Systems.Queue.Infrastructure.ServiceBus;
using R.Systems.Queue.WebApi;
using R.Systems.Queue.WebApi.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureWebApiServices();
builder.Services.ConfigureCoreServices();
builder.Services.ConfigureServiceBusServices(builder.Configuration);

WebApplication app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program
{
}
