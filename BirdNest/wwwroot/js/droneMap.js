const positionConnection = new signalR.HubConnectionBuilder()
    .withUrl("/birdNest")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();

async function start() {
    try {
        await positionConnection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

positionConnection.onclose(async () => {
    await start();
});

// Start the connection.
start();

positionConnection.on("updatePositions", (positions) => {

    const droneMap = document.getElementById('droneMap');
    droneMap.innerHTML = '<div class="circle"">';

    positions?.forEach((pos) => {
        const newPos = document.createElement('div');
        newPos.classList.add('node');
        if (pos.ndz) {
            newPos.classList.add('violation');
        }
        const posX = pos.x / 1000 - 5;
        const posY = pos.y / 1000 - 5;
        newPos.style.bottom = `${posX}px`;
        newPos.style.left = `${posY}px`;
        droneMap.appendChild(newPos);
    });
});
