import * as signalR from "@microsoft/signalr";

let connection: signalR.HubConnection; 

async function connectGameHub() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:44460/hub/game")
        .build();
    
    await connection.start();

}

function useGameHub() {
    return connection;
}

export { connectGameHub, useGameHub };