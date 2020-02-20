using Grpc.Core;
using Grpc.Net.Client;
using MyServer;
using Polly;
using System;
using System.Threading.Tasks;

namespace MyClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello gRPC Demo! \n");

            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            var maxRetryAttempts = 3;
            var pauseBetweenFailures = TimeSpan.FromSeconds(3);

            var retryPolicy = Policy
                .Handle<RpcException>()
                .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures,(ex,pause)=> {
                    Console.WriteLine(ex.Message + " => " +pause.TotalSeconds);
                });

            await retryPolicy.ExecuteAsync(async () =>
            {
                var reply = await client.SayHelloAsync(
                             new HelloRequest { Name = "Codehaks" },null,DateTime.UtcNow.AddSeconds(3));

                Console.WriteLine(reply.Message);
            });

           
        }
    }
}
