using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace scanfer.studio.autotask.win
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);



            var builder = new ContainerBuilder();

            #region 注册 ,通过config文件进行注册
            var config = new ConfigurationBuilder();
            config.AddJsonFile("autofac.json");
            // Register the ConfigurationModule with Autofac.
            var module = new ConfigurationModule(config.Build());
            builder.RegisterModule(module);
            #endregion

            #region 通过.net Core DI进行注册
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            builder.Populate(serviceCollection);
            #endregion


            var container = builder.Build();


            var serviceProvider = new AutofacServiceProvider(container);

            
            {
                //@@测试 Scanfer 
                using (var scope = container.BeginLifetimeScope())
                {
                    var component = scope.Resolve<ILog>();
                    component.LogInfo("a");
                }
            }

        }

        public interface ILog
        {
            void LogInfo(string msg);
        }
        public class Log : ILog
        {
            public void LogInfo(string msg)
            {
                Console.WriteLine(msg);
            }
        }
    }
}
