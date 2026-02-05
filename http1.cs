using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace proj2;

public class http1
{
    private readonly ILogger<http1> _logger;
    private readonly IMyService1 _myService1;
    private readonly IMyService2 _myService2;

    public http1(ILogger<http1> logger, IMyService1 myService1, IMyService2 myService2)
    {
        _logger = logger;
        _myService1 = myService1;
        _myService2 = myService2;
    }

    [Function("http1")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        _logger.LogInformation(_myService1.ReturnSomething());
        _logger.LogInformation(_myService2.ReturnSomething());

        return new OkObjectResult("Welcome to Azure Functions!");
    }
}
