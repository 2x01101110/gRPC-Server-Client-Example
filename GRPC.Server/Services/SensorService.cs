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

        public override Task<EmptyResponse> Status(SensorStatus request, ServerCallContext context)
        {
            logger.LogInformation($"Sensor {request.Id} status update received");
            logger.LogInformation(JsonConvert.SerializeObject(request, Formatting.Indented));

            return Task.FromResult(new EmptyResponse { });
        }

        public override async Task RequestCommands(CommandsRequest request,
            IServerStreamWriter<CommandResponse> responseStream, ServerCallContext context)
        {
            logger.LogInformation($"Sensor {request.Id} requested commands");

            var commands = new List<string> { "sendStatus", "sendReadings" };

            var random = new Random();

            await responseStream.WriteAsync(new CommandResponse
            {
                Command = commands[random.Next(0, 1)]
            });
        }





        //public override Task<EmptyResponse> Reading(SensorReadings request, ServerCallContext context)
        //{
        //    logger.LogInformation($"Received reading from sensor {request.Id}");

        //    return Task.FromResult(new EmptyResponse { });
        //}

        //public override async Task<EmptyResponse> Readings(
        //    IAsyncStreamReader<SensorReadings> requestStream, ServerCallContext context)
        //{
        //    logger.LogInformation($"Sensor streaming readings");

        //    while (await requestStream.MoveNext())
        //    {
        //        logger.LogInformation(JsonConvert.SerializeObject(requestStream.Current, Formatting.Indented));
        //    }

        //    // Receive status and store in sql/nosql

        //    return await Task.FromResult(new EmptyResponse { });
        //}

        //public override async Task RequestCommands(CommandsRequest request,
        //    IServerStreamWriter<CommandsResponse> responseStream, ServerCallContext context)
        //{
        //    logger.LogInformation($"Sensor {request.Id} requests commands");

        //    var commands = new List<string> { "stream_values", "update_status" };

        //    foreach (var command in commands)
        //    {
        //        await responseStream.WriteAsync(new CommandsResponse
        //        {
        //            Command = command
        //        });

        //        await Task.Delay(1000);
        //    }
        //}

    }
}
