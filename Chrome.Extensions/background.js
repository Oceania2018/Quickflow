var baseUrl = "http://localhost:3112";
var connection = new signalR.HubConnectionBuilder().withUrl(baseUrl + "/signalHub").build();

connection.on("ReceiveMessage", function (message) {
    var msg = message.Name + " " + message.Symbol + " $" + message.Amount; // message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    //document.getElementsByTagName("body").appendChild(li);
    //window.scrollTo(0, document.body.scrollHeight);

    chrome.tabs.create({ url: "https://www.google.com" }, function(tab){
        alert("new tab created");
    });

});

connection.start().catch(function (err) {
    return console.error(err.toString());
});