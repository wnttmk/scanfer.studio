using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace scanfer.studio.dockerWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()

                     //֧�������в���

                     .AddCommandLine(args)

                     //֧�ֻ�������

                     .AddEnvironmentVariables()

                     .Build();
            CreateHostBuilder(args).Build().Run();
            //new TaskFactory().StartNew(() =>
            //{
            //    CreateHostBuilder(args).Build().Run();
            //});
            //Task t = new Task(() =>
            //{
            //    CreateHostBuilder(args).Build().Run();
            //});
            //t.Start();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
