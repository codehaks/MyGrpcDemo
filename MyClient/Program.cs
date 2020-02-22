using Grpc.Net.Client;
using MyServer;
using System;
using System.Threading.Tasks;
using static MyServer.Greeter;

namespace MyClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello gRPC Demo! \n");

            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            //var reply = await client.SayHelloAsync(
            //                  new HelloRequest { Name = "Codehaks" });

            await ClientStreamingCallExample(client);

            Console.ReadLine();


            //Console.WriteLine(reply.Message);
        }

        static Random RNG = new Random();


        private static async Task ClientStreamingCallExample(GreeterClient client)
        {
            using (var call = client.AccumulateCount())
            {
                for (var i = 0; i < 10; i++)
                {
                    var count = RNG.Next(5);
                    Console.WriteLine($"Accumulating with {count}");
                    await call.RequestStream.WriteAsync(new CounterRequest { Count = count });
                    await Task.Delay(1000);
                }

                await call.RequestStream.CompleteAsync();

                var response = await call;
                Console.WriteLine($"Count: {response.Count}");
            }
        }
    }
}
