using System;
using System.Collections.Generic;
using System.Text;
using Sum;
using Grpc.Core;
using System.Threading.Tasks;
using static Sum.SumService;

namespace server
{
    class SumServiceImpl : SumServiceBase
    {
        public override Task<SumResponse> Sum(SumRequest request, ServerCallContext context)
        {
            Int32 result = request.A + request.B;

            return Task.FromResult(new SumResponse() { Result = result });
        }

        public override async Task Prime(PrimeNumberDecompRequest request, IServerStreamWriter<PrimeNumberDecompResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The Server received the request : ");
            Console.WriteLine(request.ToString());
            Int32 k = 2;
            Int32 N = request.Num;
            while(N > 1)
            {
                if (N % k == 0)
                {
                    await responseStream.WriteAsync(new PrimeNumberDecompResponse() { Primenum = k });
                    N /= k;
                } 
                else
                {
                    k++;
                }
            }
        }

        public override async Task<ComputeAverageResponse> Average(IAsyncStreamReader<ComputeAverageRequest> requestStream, ServerCallContext context)
        {
            double total = 0;
            double count = 0;
            while(await requestStream.MoveNext())
            {
                total += requestStream.Current.Num;
                count++;
            }
            return new ComputeAverageResponse() { AverageCalc = total / count };
        }

        public override async Task Max(IAsyncStreamReader<FindMaxRequest> requestStream, IServerStreamWriter<FindMaxResponse> responseStream, ServerCallContext context)
        {
            int? max = null;
            while(await requestStream.MoveNext())
            {
                if (max == null || max < requestStream.Current.Num)
                {
                    max = requestStream.Current.Num;
                    await responseStream.WriteAsync(new FindMaxResponse() { Result = max.Value });
                }
            }
        }
    }
}
