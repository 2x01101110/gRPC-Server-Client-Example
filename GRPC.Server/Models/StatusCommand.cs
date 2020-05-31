using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRPC.Server.Models
{
    public class StatusCommand
    {
        public readonly string commandName = "sendStatus";

        private StatusCommand() { }

        public static string Create() => new StatusCommand().ToString();

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
