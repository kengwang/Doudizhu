import * as signalR from "@microsoft/signalr";

let connection: signalR.HubConnection; 
const currentGame = useCurrentGame();
const currentUser = useLogginedUser();

async function connectGameHub() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("http://192.168.110.170:44460/hub/game")
        .withAutomaticReconnect()
        .withKeepAliveInterval(10000)
        .build();
    connection.on("UserJoined", (user) => {
        console.log("UserJoined", user);
        if (currentGame.game?.users) {
            currentGame.game.users.push(user);
        }
        currentGame.joinedUsers.push(user);
    });

    connection.on("RequireCallLandLord", (game) => {
        currentGame.shouldCallLandlord = true;
    });
    
    connection.on("UserCalledLandLord", (user, count) => {
        currentGame.joinedUsers.forEach((u) => {
            if (u.id === user.id) {
                u.calledLandLordCount = count;
            }
        });
    });

    connection.on("UserPlayCards", (user, cards) => {
        console.log("UserPlayCards", user, cards);
        currentGame.lastCards = cards.cards;
    });

    connection.on("LandLordSelected", (user) => { 
        console.log("LandLordSelected", user);
        currentGame.shouldCallLandlord = false;
        currentGame.landlordId = user.id;
    });

    connection.on("RequirePlayCards", (user) => {
        console.log("RequirePlayCards", user);
        currentGame.currentUserId = user.user.id;
        if (user.user.id === currentUser.logginedUser?.id) {
            currentGame.canPlayCard = true;
        } else {
            currentGame.canPlayCard = false;
        }
    });
    
    connection.on("GameStarted", (game) => {
        console.log("GameStarted", game);
        currentGame.game = game;
    })

    connection.on("ReceiveCards", (cards) => {
        console.log("ReceiveCards", cards);
        currentGame.cards = cards;
    });

    await connection.start();

}

function useGameHub() {
    return connection;
}

export { connectGameHub, useGameHub };