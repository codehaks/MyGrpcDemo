using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace MyServer
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public async override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            await Task.Delay(5000);

            return new HelloReply
            {
                Message = "Hello " + request.Name
            };
        }
    }
}
