
var connection = new signalR.HubConnectionBuilder().withUrl("/SignalrHub").withAutomaticReconnect().build();

connection.on("receivemessage", (user, message)=> {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedmsg = user + " says " + msg;
    var li = document.createElement("li");
    li.innerText = encodedmsg;
    document.getElementById("messageslist").appendChild(li);
});
connection.on("RegisterToken", obj=> {
    var li = document.createElement("li");
    li.innerText = "RegisterToken";
    document.getElementById("messageslist").appendChild(li);
});

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};
connection.onclose(async () => {
    //await start();
    var li = document.createElement("li");
    li.innerText = "Signalr is Cancel";
    document.getElementById("messageslist").appendChild(li);
});
document.getElementById("sendbutton").addEventListener("click", function (event) {
    var user = document.getElementById("userinput").value;
    var message = document.getElementById("messageinput").value;
    connection.invoke("sendmessage", user, message).catch(function (err) {
        return console.error(err.tostring());
    });
    event.preventDefault();
});
start();
