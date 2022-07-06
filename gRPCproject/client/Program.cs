using Grpc.Core;
using System;
using System.Threading.Tasks;
using Dummy;
using Greet;
using Sum;
using System.Linq;

namespace client
{
    class Program
    {
        const string target = "127.0.0.1:50051";
        static async Task Main(string[] args)
        {
            Channel channel = new Channel(target, ChannelCredentials.Insecure);

            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("The Client connected successfully");
            });

            // var client = new DummyService.DummyServiceClient(channel);
             // var client = new GreetingService.GreetingServiceClient(channel);
            var client = new SumService.SumServiceClient(channel);

            // Sum Tasks
            //DoSum(client);
            //await DoComputeAverage(client);
            //await DoPrimeDecomp(client);
            await DoFindMax(client);

            // Greet Tasks
            //DoSimpleGreet(client);
            //await DoManyGreets(client);
            //await DoLongGreet(client);
            //await DoGreetEveryone(client);
            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
        public static void DoSum(SumService.SumServiceClient client)
        {
            var request = new SumRequest()
            {
                A = 14023,
                B = 12332,
            };
            var response = client.Sum(request);
            Console.WriteLine(response.Result);
        }
        public static async Task DoPrimeDecomp(SumService.SumServiceClient client)
        {
            var request = new PrimeNumberDecompRequest()
            {
                Num = 120,
            };
            var response = client.Prime(request);
            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Primenum);
                await Task.Delay(200);
            }
        }
        public static async Task DoFindMax(SumService.SumServiceClient client)
        {
            var stream = client.Max();
            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine(stream.ResponseStream.Current.Result);
                }
            });

            int[] numbers = { 1, 5, 3, 6, 2, 20 };

            foreach (var number in numbers)
            {
                await stream.RequestStream.WriteAsync(new FindMaxRequest() { Num = number });
            }

            await stream.RequestStream.CompleteAsync();
            await responseReaderTask;
        }
        public static async Task DoComputeAverage(SumService.SumServiceClient client)
        {
            var request = new ComputeAverageRequest()
            {
                Num = 1
            };
            var stream = client.Average();
            foreach (int i in Enumerable.Range(1, 4))
            {
                await stream.RequestStream.WriteAsync(request);
                request.Num++;
            }
            await stream.RequestStream.CompleteAsync();
            var response = stream.ResponseAsync;
            Console.WriteLine(response.Result.AverageCalc);
        }
        public static void DoSimpleGreet(GreetingService.GreetingServiceClient client)
        {
            var greeting = new Greeting()
            {
                FirstName = "John",
                LastName = "Kim"
            };
            var request = new GreetingRequest() { Greeting = greeting };
            var response = client.Greet(request);
            Console.WriteLine(response.Result);
        }
        public static async Task DoManyGreets(GreetingService.GreetingServiceClient client)
        {
            var greeting = new Greeting()
            {
                FirstName = "John",
                LastName = "Kim"
            };
            var request = new GreetingManyTimesRequest() { Greeting = greeting };
            var response = client.GreetManyTimes(request);

            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
        }
        public static async Task DoLongGreet(GreetingService.GreetingServiceClient client)
        {
            var greeting = new Greeting()
            {
                FirstName = "Han",
                LastName = "Kim"
            };
            var request = new LongGreetRequest() { Greeting = greeting };
            var stream = client.LongGreet();
            foreach (int i in Enumerable.Range(1, 10))
            {
                await stream.RequestStream.WriteAsync(request);
            }

            await stream.RequestStream.CompleteAsync();
            var response = await stream.ResponseAsync;
            Console.WriteLine(response.Response.ToString());
        }
        public static async Task DoGreetEveryone(GreetingService.GreetingServiceClient client)
        {
            var stream = client.GreetEveryone();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine("Received: " + stream.ResponseStream.Current.Result);
                }
            });
            Greeting[] greetings =
            {
                new Greeting() { FirstName = "Han", LastName = "Kim"},
                new Greeting() { FirstName = "Ryan", LastName = "Brown"},
                new Greeting() { FirstName = "Jalen", LastName = "Gu"}
            };

            foreach (var greeting in greetings)
            {
                Console.WriteLine("Sending: " + greeting.ToString());
                var temp = new GreetEveryoneRequest()
                {
                    Greeting = greeting
                };
                await stream.RequestStream.WriteAsync(temp);
            }
             await stream.RequestStream.CompleteAsync();
            await responseReaderTask;
        }
    }
}
