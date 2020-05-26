using Grpc.Core;
using GRPC.Server.Protos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRPC.Server.Services
{
    public class SensorService : Sensor.SensorBase
    {
        private readonly ILogger logger;
        public SensorService(ILogger<SensorService> logger)
        {
            this.logger = logger;
        }

        public override Task<ServiceStatusResponse> CheckServiceHealth(
            ServiceStatusRequest request, ServerCallContext context)
        {
            logger.LogInformation($"Server status update request received from sensor {request.Id}");

            return Task.FromResult(new ServiceStatusResponse
            {
                Uptime = 1000
            });
        }

        public override Task<SensorStatusReceivedResponse> UpdateStatus(
            SensorStatusRequest request, ServerCallContext context)
        {
            logger.LogInformation($"Sensor {request.Id} status update received");
            logger.LogInformation(JsonConvert.SerializeObject(request, Formatting.Indented));

            // Receive status and store in sql/nosql

            return Task.FromResult(new SensorStatusReceivedResponse { });
        }

        public override async Task<SensorReadingValuesReceivedResponse> Readings(
            IAsyncStreamReader<SensorReadingValuesRequest> requestStream, ServerCallContext context)
        {
            var readings = new List<SensorReadingValuesRequest>();

            logger.LogInformation($"Sensor streaming readings");

            while (await requestStream.MoveNext())
            {
                logger.LogInformation(JsonConvert.SerializeObject(requestStream.Current, Formatting.Indented));
                readings.Add(requestStream.Current);
            }

            // Receive status and store in sql/nosql

            return await Task.FromResult(new SensorReadingValuesReceivedResponse { });
        }


        public override async Task ReceiveCommands(ServerToSensorCommandsRequest request, 
            IServerStreamWriter<ServerToSensorCommandsResponse> responseStream, ServerCallContext context)
        {
            logger.LogInformation($"Sensor {request.Id} requests commands");

            var commands = new List<string> { "stream_values", "update_status" };

            foreach (var command in commands)
            {
                await responseStream.WriteAsync(new ServerToSensorCommandsResponse
                {
                    Command = command
                });

                await Task.Delay(1000);
            }
        }
    }
}
