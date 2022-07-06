using System;
using Grpc.Core;
using System.IO;
using Grpc.AspNetCore.Server;
using Greet;
using Sum;

namespace server
{
    class Program
    {
        const int Port = 50051;
        static void Main(string[] args)
        {
            Server server = null;

            try
            {
                server = new Server()
                {
                     Services = { GreetingService.BindService(new GreetingServiceImpl()) },
                     // Services = { SumService.BindService(new SumServiceImpl())},
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
                };

                server.Start();
                Console.WriteLine("The Server is listening on the port: " + Port);
                Console.ReadKey();
            }
            catch(IOException e)
            {
                Console.WriteLine("The server failed to start: " + e.Message);
            }
            finally
            {
                if (server != null)
                {
                    server.ShutdownAsync().Wait();
                }
            }
            
        }
    }
}
