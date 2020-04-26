using Grpc.Core;
using Grpc.Net.Client;
using scanfer.studio.GrpcTest;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace scanfer.studio.GrpcClient
{
    class Program
    {
        static object o = new object();
        public static void DeadLockTest(int i)
        {
            lock (o)   //或者lock一个静态object变量
            {
                if (i > 10)
                {
                    Console.WriteLine(i--);
                    DeadLockTest(i);
                }
            }
        }
        static ConcurrentQueue<Task> _Products;
        static void Main(string[] args)
        {
            
            var tuple = Tuple.Create(1, "2", new string[] { }, Task.Factory, 1, false, "a".GetType());

            Span<string> span = new Span<string>();

            var taskFactory = Task.Factory;
            ThreadPool.SetMaxThreads(2, 2);

            Action<string, string, string> ac1 = (p1, p2, p3) =>
            {
                //do anything
            }
                ;
            Func<String, int> a = (b) =>
            {
                return Convert.ToInt32(b);
            };
            var c = a("ff");
            _Products = new ConcurrentQueue<Task>();
            Stopwatch swTask = new Stopwatch();//用于统计时间消耗的
            swTask.Start();
            Task t1 = taskFactory.StartNew(() =>
            {
                for (var i = 0; i < 10000; i++)
                {

                    if (_Products.Count == 5000)
                    { }
                    if (_Products.Count < 5000)
                    {

                        string iStr = $"{i}{i++}";


                        var action = new Action<object>((str) =>
                        {
                            Console.WriteLine(str);
                        });

                        _Products.Enqueue(taskFactory.StartNew(action, i));
                    }


                }
            });

            Task t2 = taskFactory.StartNew(() =>
            {
                for (var i = 0; i < 10000; i++)
                {
                    if (_Products.Count == 5000)
                    { }
                    if (_Products.Count < 5000)
                    {

                        var action = new Action<object>((str) =>
                        {
                            Console.WriteLine(str);
                        });

                        _Products.Enqueue(taskFactory.StartNew(action, i));
                    }


                }
            });
            Task.WaitAll(t1, t2);
            Task.WaitAll(_Products.ToArray());


            swTask.Stop();
            Console.WriteLine("List<Product> 当前数据量为：" + _Products.Count);
            Console.WriteLine("List<Product> 执行时间为：" + swTask.ElapsedMilliseconds);

            Console.ReadLine();


            //var channel = GrpcChannel.ForAddress("http://localhost:58001");
            //var client = new Greeter.GreeterClient(channel);
            //var result = client.SayHello(new HelloRequest()
            //{
            //    Name = "Scanfer"
            //});

            //var result2 = client.GetServerDateTime(new DateTimeRequest() { });

            //var channel = GrpcChannel.ForAddress("https://localhost:44360");
            //var client = new scCore.scControl.ExecuteServ.ExecuteServClient(channel);
            //var r = client.AddPersonAsync(new scCore.scControl.Person
            //{
            //    Email = "email",
            //    Id = 1,
            //    Name = "fd",

            //});


            Console.ReadLine();

            P().GetAwaiter().GetResult();

            Task.WaitAll(P(), P2());
        }

        public static async void Exe()
        {
            await P();
        }

        public static async Task P()
        {

            await Task.Factory.StartNew(() =>
            {
                var t = new TaskFactory()
                {

                };

            });
        }

        public static async Task P2()
        {
            await new TaskFactory().StartNew(() =>
            {

            });
        }


        class Product
        {
            public string Name { get; set; }
            public string Category { get; set; }
            public int SellPrice { get; set; }
        }
    }
}
