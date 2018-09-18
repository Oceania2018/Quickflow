"use strict";

$(document).ready(function () {

    var connection = new signalR.HubConnectionBuilder().withUrl("/signalHub").build();

    connection.on("ReceiveMessage", function (message) {
        var msg = message.Name + " " + message.Symbol + " $" + message.Amount; // message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var encodedMsg = msg;
        var li = document.createElement("li");
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
        window.scrollTo(0, document.body.scrollHeight);

        window.open('https://www.google.com', '_blank');

        // chrome.tabs.create({ url: message });
    });

    connection.start().catch(function (err) {
        return console.error(err.toString());
    });

    /*
    document.getElementById("sendButton").addEventListener("click", function (event) {
        var user = document.getElementById("userInput").value;
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
    */

});

