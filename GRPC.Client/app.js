const gRPCClient = require("./client");
const moment = require("moment");

async function main() {
  this.startTime = moment().format();
  this.client = new gRPCClient();
  await sendStatus();
}

async function sendStatus() {
  const uptime = Math.ceil(
    moment.duration(moment().diff(this.startTime)).asSeconds()
  );
  const { commands } = await this.client.status({
    id: 1,
    uptime: uptime,
  });

  await processCommands(commands);
}

async function processCommands(commands) {
  for (let i = 0; i < commands.length; i++) {
    const command = JSON.parse(commands[i]);
    switch (command.commandName) {
      case "streamReadings":
        await streamReadings(command.streamDuration);
        break;
      case "sleep":
        console.log("Sleeping...");
        await sleep(command.sleepDuration);
        break;
      case "sendStatus":
        await sendStatus();
        break;
      default:
        break;
    }
  }
}

async function streamReadings(streamDuration) {
  const { write, end } = await this.client.streamReadings();
  do {
    console.log(streamDuration);
    await write({
      sensorId: 1,
      readingValues: [
        {
          field: "temperature",
          value: 23.43,
        },
      ],
    });
    await sleep(500);
    streamDuration -= 500;
  } while (streamDuration >= 0);
  await end();
}

function sleep(sleepDuration) {
  return new Promise((resolve, reject) => {
    setTimeout(() => {
      resolve();
    }, sleepDuration);
  });
}

main();
