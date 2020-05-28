const gRPCClient = require("./client");
const moment = require("moment");

async function main() {
  this.startTime = moment().format();
  this.client = new gRPCClient();
  await sendStatus();
  await requestCommands();
}

async function sendStatus() {
  const uptime = Math.ceil(
    moment.duration(moment().diff(this.startTime)).asSeconds()
  );
  await this.client.status({
    id: 1,
    uptime: uptime,
    errors: [],
  });
}

async function requestCommands() {
  const commandMappings = [{ command: "sendStatus", function: sendStatus }];
  await this.client.requestCommands(1, async (response) => {
    const commandMapping = commandMappings.find(
      (x) => x.command === response.command
    );
    if (commandMapping) {
      await commandMapping.function.call(this);
    }
  });
}

main();
