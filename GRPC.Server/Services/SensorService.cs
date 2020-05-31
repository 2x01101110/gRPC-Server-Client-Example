using Grpc.Core;
using GRPC.Server.Models;
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

        public override Task<SensorCommands> Status(SensorStatus request, ServerCallContext context)
        {
            logger.LogInformation($"\r\nSensor {request.SensorId} sent status update \r\n");

            var response = new SensorCommands();
            response.Commands.AddRange(SensorCommands(request.SensorId));

            return Task.FromResult(response);
        }

        public override async Task<BlankResponse> ReadingsStream(IAsyncStreamReader<SensorReadings> requestStream, ServerCallContext context)
        {
            logger.LogInformation("\r\n");

            while (await requestStream.MoveNext())
            {
                foreach (var readingValue in requestStream.Current.ReadingValues)
                {
                    logger.LogInformation($"{requestStream.Current.SensorId} | {readingValue.Field} : {readingValue.Value}");
                }
            }

            logger.LogInformation("\r\n");

            return await Task.FromResult(new BlankResponse { });
        }

        private List<string> SensorCommands(int sensorId)
        {
            var commands = new List<string>();

            commands.Add(StreamReadingsCommand.Create(15000));
            commands.Add(SleepCommand.Create(1000));
            commands.Add(StatusCommand.Create());

            return commands;
        }
    }
}
