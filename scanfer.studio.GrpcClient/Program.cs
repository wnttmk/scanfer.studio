using Grpc.Net.Client;
using scanfer.studio.GrpcTest;
using System;

namespace scanfer.studio.GrpcClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var channel = GrpcChannel.ForAddress("https://localhost:58001");
            var client = new Greeter.GreeterClient(channel);
            var result = client.SayHello(new HelloRequest()
            {
                Name = "Scanfer"
            });

            var result2 = client.GetServerDateTime(new DateTimeRequest() { });
        }
    }
}
