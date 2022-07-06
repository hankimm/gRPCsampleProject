﻿using System;
using Grpc.Core;
using System.IO;
using Grpc.AspNetCore.Server;
using Greet;
using Sum;
using System.Collections.Generic;

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
                var serverCert = File.ReadAllText("ssl/server.crt");
                var serverKey = File.ReadAllText("ssl/server.key");
                var keypair = new KeyCertificatePair(serverCert, serverKey);
                var cacert = File.ReadAllText("ssl/ca.crt");

                var credentials = new SslServerCredentials(new List<KeyCertificatePair>() { keypair }, cacert, true);
                server = new Server()
                {
                     Services = { GreetingService.BindService(new GreetingServiceImpl()) },
                     // Services = { SumService.BindService(new SumServiceImpl())},
                    Ports = { new ServerPort("localhost", Port, credentials) }
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
