using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace MyServer
{
    public class IncrementingCounter
    {
        public void Increment(int amount)
        {
            Count += amount;
        }

        public int Count { get; private set; }
    }

    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly IncrementingCounter _counter;


        public GreeterService(IncrementingCounter counter,ILogger<GreeterService> logger)
        {  _counter = counter;
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task<CounterReply> AccumulateCount(IAsyncStreamReader<CounterRequest> requestStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                _logger.LogInformation($"Incrementing count by {message.Count}");

                _counter.Increment(message.Count);
            }

            return new CounterReply { Count = _counter.Count };
        }
    }
}
