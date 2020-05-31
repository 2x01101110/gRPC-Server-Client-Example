const protoLoader = require("@grpc/proto-loader");
const grpc = require("grpc");

class gRPCClient {
  constructor() {
    const packageDefinition = grpc.loadPackageDefinition(
      protoLoader.loadSync("./protos/sensor.proto", {
        keepCase: true,
        longs: true,
        enums: String,
        defaults: true,
        oneofs: true,
      })
    ).sensor;

    this.client = new packageDefinition.Sensor(
      "localhost:5001",
      grpc.credentials.createInsecure()
    );
  }

  status(status) {
    return new Promise((resolve, reject) => {
      this.client.Status(status, (error, response) => {
        if (error) {
          reject(error);
        } else {
          resolve(response);
        }
      });
    });
  }

  streamReadings() {

    return new Promise((resolve, reject) => {
      const stream = this.client.ReadingsStream((error) => {
        reject(error);
      });
      resolve({ 
        write: (reading) => new Promise((resolve, e) => {
          console.log('writing');
          stream.write(reading);
          resolve();
        }), 
        end: () => new Promise((resolve, e) => {
          console.log('ending');
          stream.end();
          resolve();
        })
      });
    });





    // return new Promise((resolve, reject) => {
    //   const commandRequest = this.client.RequestCommands({ id });

    //   commandRequest.on("data", (data) => {
    //     commandCallback(data);
    //   });

    //   commandRequest.on("error", (error) => {
    //     reject(error);
    //   });

    //   commandRequest.on('end', () => {
    //     resolve();
    //   });
    // });
  }
}

module.exports = gRPCClient;
