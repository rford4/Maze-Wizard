using MazeWizard.AppServices;
using MazeWizard.Presentation;
using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new();
ConfigureServices(services);

services.AddSingleton<Application>();

var application = services.BuildServiceProvider()
    .GetService<Application>();

if(application != null)
    await application.Run(args);

static void ConfigureServices(IServiceCollection services)
{

}