const grpc = require('grpc');
const protoLoader = require('@grpc/proto-loader');

const packageDefinition = protoLoader.loadSync('./protos/user.proto', 
{
    keepCase: true,
    longs: true, 
    enums: String,
    defaults: true,
    oneofs: true
});

const sample_proto = grpc.loadPackageDefinition(packageDefinition).sample;
const client = new sample_proto.Sample('localhost:5001', grpc.credentials.createInsecure());

function grpcResponseHandler(error, response) {
    if (error) {
        console.log(error);
    }
    else {
        console.log(response);
    }
}

function simpleRPC(value) {
    client.SimpleRPC({ text: value }, grpcResponseHandler);
}

function serverSideStreamingRPC(value) {
    const streamingRPC = client.ServerSideStreamingRPC({});
    streamingRPC.on('data', (data) => {
        console.log(data);
    });
    streamingRPC.on('end', (data) => {
        console.log('Call ended');
    });
    streamingRPC.on('error', (error) => {
        console.log(error);
    });
    streamingRPC.on('status', (status) => {
        console.log(status);
    });
}

async function clientSideStreamingRPC() {
    const streamingRPC = client.ClientSideStreamingRPC((error, stats) => {
        if (error) {
            console.log(error);
        }
        console.log(stats);
    });

    streamingRPC.on('data', (data) => {
        console.log(data);
    });

    const timeoutPromise = () => {
        console.log(Math.random() * 100);
        return new Promise(resolve => setTimeout(resolve, 200 + Math.random() * 100));
    };

    for (let i = 0; i < 10; i++) {
        await timeoutPromise(streamingRPC.write({ text: new Date().toUTCString() }));

        if (i + 1 === 10) {
            streamingRPC.end();
        }
    }
}


//simpleRPC('Hello World!');
//serverSideStreamingRPC();
clientSideStreamingRPC();