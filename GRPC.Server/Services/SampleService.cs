using Grpc.Core;
using GRPC.Server.Protos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRPC.Server.Services
{
    public class SampleService : Sample.SampleBase
    {
        private readonly ILogger logger;

        public SampleService(ILogger<SampleService> logger)
        {
            this.logger = logger;
        }

        public override Task<SimpleRPCResponse> SimpleRPC(SimpleRPCRequest request, ServerCallContext context)
        {
            logger.LogInformation("Simple RPC request received,");

            return Task.FromResult(new SimpleRPCResponse { Text = $"{request.Text} {DateTime.UtcNow}" });
        }

        public override async Task ServerSideStreamingRPC(EmptyRequest request, IServerStreamWriter<SimpleRPCResponse> responseStream, ServerCallContext context)
        {
            logger.LogInformation("Server Side Streaming RPC request received,");

            for (int i = 0; i < 10; i++)
            {
                await responseStream.WriteAsync(new SimpleRPCResponse { Text = DateTime.UtcNow.ToString() });
                await Task.Delay(250);
            }
        }

        public override async Task<SimpleRPCResponse> ClientSideStreamingRPC(IAsyncStreamReader<SimpleRPCRequest> requestStream, ServerCallContext context)
        {
            var list = new List<DateTime>();

            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;

                logger.LogInformation($"Client Side Streaming RPC request received. {request.Text}");

                if (DateTime.TryParse(request.Text, out var time))
                {
                    list.Add(time);
                }
            }

            if (list.Count >= 2)
            {
                var diff = list[0] - list[list.Count - 1];
                return await Task.FromResult(new SimpleRPCResponse { Text = $"{diff.TotalMilliseconds}" });
            }
            else {
                return await Task.FromResult(new SimpleRPCResponse { Text = $"Error" });
            }
        }
    }
}
