const violationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/droneInfo")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();

async function start() {
    try {
        await violationConnection.start();
        console.log("SignalR Connected");
        try {
            await violationConnection.invoke("GetLatestViolations")
        } catch (err) {
            console.error(err);
        }
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

violationConnection.onclose(async () => {
    await start();
});

// Start the connection.
start();

violationConnection.on("updateViolations", (violations) => {
    const violationList = document.getElementById('violationList');
    violationList.innerHTML = '';

    violations?.forEach((v) => {
        const newViolation = document.createElement('div');
        newViolation.classList.add('violatingDrone');

        const title = document.createElement('h4');
        title.innerText = v.firstName + ' ' + v.lastName;
        newViolation.appendChild(title);
        const distance = document.createElement('p');
        distance.innerText = 'Distance: ' + Math.floor(v.distance / 1000) + ' m';
        newViolation.appendChild(distance);
        const phone = document.createElement('p');
        phone.innerText = 'Phone: ' + v.phoneNumber;
        newViolation.appendChild(phone);
        const email = document.createElement('p');
        email.innerText = 'Email: ' + v.email;
        newViolation.appendChild(email);

        violationList.appendChild(newViolation);
    });
});
