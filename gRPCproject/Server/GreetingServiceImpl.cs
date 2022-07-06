using System;
using System.Collections.Generic;
using System.Text;
using Greet;
using Grpc.Core;
using System.Threading.Tasks;
using static Greet.GreetingService;
using System.Linq;

namespace server
{
    class GreetingServiceImpl : GreetingServiceBase
    {
        public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            string result = String.Format("hello {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);

            return Task.FromResult(new GreetingResponse() { Result = result });
        }

        public override async Task GreetManyTimes(GreetingManyTimesRequest request, IServerStreamWriter<GreetingManyTimesResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The Server received the request : ");
            Console.WriteLine(request.ToString());

            string result = String.Format("hello {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);

            for(int i = 0; i < 10; i++)
            {
                await responseStream.WriteAsync(new GreetingManyTimesResponse() { Result = result });
            }
        }
        public override async Task<LongGreetResponse> LongGreet(IAsyncStreamReader<LongGreetRequest> requestStream, ServerCallContext context)
        {
            string result = "";
            while (await requestStream.MoveNext())
            {
                result += String.Format("Hello {0} {1} {2}",
                    requestStream.Current.Greeting.FirstName,
                    requestStream.Current.Greeting.LastName,
                    Environment.NewLine);
            }
            return new LongGreetResponse() { Response = result };
        }

        public override async Task GreetEveryone(IAsyncStreamReader<GreetEveryoneRequest> requestStream, IServerStreamWriter<GreetEveryoneResponse> responseStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                var result = String.Format("hello {0} {1}",
                    requestStream.Current.Greeting.FirstName,
                    requestStream.Current.Greeting.LastName);
                Console.WriteLine("Recieved: " + result);
                await responseStream.WriteAsync(new GreetEveryoneResponse() { Result = result });
            }
        }
    }
}
