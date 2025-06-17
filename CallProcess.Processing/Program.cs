using CallProcess.Application;
using CallProcess.Application.Interfaces.CallProcessing;
using CallProcess.Processing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

try
{
    var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        // Initialize Redis and other Application services
        var cacheConnectionString = configuration.GetSection("CacheSettings:ConnectionString").Value ?? string.Empty;
        services.InitializeApplication(cacheConnectionString);

        // Register CallManager and ProcessFile
        services.AddTransient<ICallManager, CallManager>();
        services.AddTransient<IProcessFile, ProcessFile>();

        // Register CallProcessor as Hosted Service
        services.AddHostedService<CallProcessor>();
    });

    await builder.RunConsoleAsync();
}
catch(Exception ex)
{
    Console.WriteLine("Error starting an application :: " + ex.ToString());
}
