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
            var config = new TelemetryConfiguration()
            {
                ConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")
            };

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

            builder.Services.AddSingleton(new MyService1());

            // write out current timestamp
            telemetryClient.TrackTrace($"MyService1 Complete timestamp: {DateTime.UtcNow}");
            Console.WriteLine($"Current timestamp: {DateTime.UtcNow}");

            builder.Services.AddSingleton(new MyService2());

            // write out current timestamp
            telemetryClient.TrackTrace($"MyService2 Complete  timestamp: {DateTime.UtcNow}");
            Console.WriteLine($"Current timestamp: {DateTime.UtcNow}");

            builder.Build().Run();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"EXCEPTION: {ex}");
        }
    }
}

public class MyService1
{
    public MyService1()
    {
        int sleepNum = Convert.ToInt32(Environment.GetEnvironmentVariable("SLEEP_SECONDS"));
        Thread.Sleep(this.GetRandomNumber(sleepNum) * 1000);
    }

    // write random generator between 1 and 60 
    private int GetRandomNumber(int max)
    {
        Random random = new Random();
        return random.Next(1, max + 1);
    }

    public string ReturnSomething()
    {
        return "Hello from MyService1!";
    }
}


public class MyService2
{
    public MyService2()
    {
        int sleepNum = Convert.ToInt32(Environment.GetEnvironmentVariable("SLEEP_SECONDS"));
        Thread.Sleep(this.GetRandomNumber(sleepNum) * 1000);
    }

    // write random generator between 1 and 60 
    private int GetRandomNumber(int max)
    {
        Random random = new Random();
        return random.Next(1, max + 1);
    }

    public string ReturnSomething()
    {
        return "Hello from MyService2!";
    }
}