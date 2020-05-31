using Newtonsoft.Json;

namespace GRPC.Server.Models
{
    public class SleepCommand
    {
        public readonly string commandName = "sleep";
        public int sleepDuration { get; private set; }

        private SleepCommand(int sleepDuration)
        {
            this.sleepDuration = sleepDuration;
        }

        public static string Create(int sleepDuration) => new SleepCommand(sleepDuration).ToString();

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
