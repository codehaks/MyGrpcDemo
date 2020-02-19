using Grpc.Core;
using Grpc.Net.Client;
using MyServer;
using System;
using System.Threading;
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
            var headers = new Grpc.Core.Metadata();
            headers.Add(new Metadata.Entry("user", "codehaks"));
   
            var options = new Grpc.Core.CallOptions(headers, deadline: DateTime.UtcNow.AddSeconds(4));
            CancellationTokenSource source = new CancellationTokenSource();

            source.CancelAfter(TimeSpan.FromSeconds(1));
            CancellationToken token = source.Token;


            try
            {
               
                var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "Codehaks" },
                  headers, DateTime.UtcNow.AddSeconds(4),token);

                


                Console.WriteLine(reply.Message);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.StatusCode);
            }

            await channel.ShutdownAsync();

        }
    }
}
