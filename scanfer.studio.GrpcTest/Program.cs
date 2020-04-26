using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace scanfer.studio.GrpcTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new TaskFactory().StartNew(() =>
            {
                CreateHostBuilder(args).Build().Run();
            });
            while (true) {
                var k = Console.ReadLine();
            }
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    
                    ;
                });
    }
}
