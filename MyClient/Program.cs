using Grpc.Net.Client;
using MyServer;
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

            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "Codehaks" });

            Console.WriteLine(reply.Message);
        }
    }
}
