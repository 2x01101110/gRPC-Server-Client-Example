using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRPC.Server.Models
{
    public class StreamReadingsCommand
    {
        public readonly string commandName = "streamReadings";
        public int streamDuration { get; private set; }

        private StreamReadingsCommand(int streamDuration)
        {
            this.streamDuration = streamDuration;
        }

        public static string Create(int streamDurationInMiliseconds) => new StreamReadingsCommand(streamDurationInMiliseconds).ToString();

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
