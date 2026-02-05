using System;
using System.Threading;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            var config = TelemetryConfiguration.CreateDefault();
            config.ConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

            var telemetryClient = new TelemetryClient(config);

            telemetryClient.TrackTrace("STARTING FUNCTION APP!");

            Console.WriteLine("STARTING FUNCTION APP CONSOLE!");

            var builder = FunctionsApplication.CreateBuilder(args);

            builder.ConfigureFunctionsWebApplication();

            builder.Services
                .AddApplicationInsightsTelemetryWorkerService()
                .ConfigureFunctionsApplicationInsights();

            Console.WriteLine("RUNNING host.Run()!");

            // write out current timestamp
            telemetryClient.TrackTrace($"Before Services timestamp: {DateTime.UtcNow}");
            Console.WriteLine($"Current timestamp: {DateTime.UtcNow}");

            builder.Services.AddSingleton<IMyService1, MyService1>();

            // write out current timestamp
            telemetryClient.TrackTrace($"MyService1 Complete timestamp: {DateTime.UtcNow}");
            Console.WriteLine($"Current timestamp: {DateTime.UtcNow}");

            builder.Services.AddSingleton(new MyService2());

            // write out current timestamp
            telemetryClient.TrackTrace($"MyService2 Complete  timestamp: {DateTime.UtcNow}");
            Console.WriteLine($"Current timestamp: {DateTime.UtcNow}");

            // Flush telemetry before starting the host to ensure startup logs are sent
            telemetryClient.Flush();
            Thread.Sleep(1000); // Give time for the flush to complete

            builder.Build().Run();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"EXCEPTION: {ex}");
        }
    }
}

public interface IMyService1
{
    string ReturnSomething();
}

public interface IMyService2
{
    string ReturnSomething();
}

public class MyService1 : IMyService1
{
    public void MyService1Start()
    {
        int sleepNum = Convert.ToInt32(Environment.GetEnvironmentVariable("SLEEP_SECONDS"));
        Thread.Sleep(this.GetRandomNumber(sleepNum) * 1000);
    }

    // write random generator between 1 and 60 
    private int GetRandomNumber(int max)
    {
        Random random = new Random();
        return random.Next(10, max + 1);
    }

    public string ReturnSomething()
    {
        return "Hello from MyService1!";
    }
}

public class MyService2 : IMyService2
{
    public void MyService2Start()
    {
        int sleepNum = Convert.ToInt32(Environment.GetEnvironmentVariable("SLEEP_SECONDS"));
        Thread.Sleep(this.GetRandomNumber(sleepNum) * 1000);
    }

    // write random generator between 1 and 60 
    private int GetRandomNumber(int max)
    {
        Random random = new Random();
        return random.Next(10, max + 1);
    }

    public string ReturnSomething()
    {
        return "Hello from MyService2!";
    }
}