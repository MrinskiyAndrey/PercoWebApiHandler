using ConnectionService.Models;
using NewEmployeesService.Models.DTO;
using PercoWebAPIHandler;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.WebHost.UseUrls("http://0.0.0.0:49003");

var config = new ConfigurationService.Config();


app.MapGet("/UnloadingEvents", (IServiceScopeFactory scopeFactory) =>

_ = Task.Run(async () =>
{
    using (var scope = scopeFactory.CreateScope())
    {
        await UnloadingEventsService.UnloadingEvents.Unloading();
    }
    return Results.Ok("The UnloadingEventsService module was executed successfully.");
}));


app.MapGet("/AddNewEmployees", (IServiceScopeFactory scopeFactory) =>

_ = Task.Run(async () =>
{
    using (var scope = scopeFactory.CreateScope())
    {
        await NewEmployeesService.NewEmployees.NewEmployeesMethod();
    }
    return Results.Ok("The NewEmployeesService module was executed successfully.");
}));


app.MapGet("/", () => ConfigurationService.Logger.ReadLogFile());



app.Run();
