using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyServer;
using MyWebClient.Models;
using Polly;

namespace MyWebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("api/hi")]
        public async Task<IActionResult> Hello()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            var maxRetryAttempts = 10;
            var pauseBetweenFailures = TimeSpan.FromSeconds(3);

            var retryPolicy = Policy
                .Handle<RpcException>()
                .WaitAndRetryAsync(maxRetryAttempts,
                i => pauseBetweenFailures, (ex, pause) =>
                {
                    _logger.LogError(ex.Message + " => " + pause.TotalSeconds);
                });

            HelloReply reply=new HelloReply();

            await retryPolicy.ExecuteAsync(async () =>
               {
                   reply = await client.SayHelloAsync(
                                new HelloRequest { Name = "Codehaks" }, null, DateTime.UtcNow.AddSeconds(2));
                   
               });


            return Ok(reply.Message);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
