var baseUrl = "http://localhost:3112";
var connection = new signalR.HubConnectionBuilder().withUrl(baseUrl + "/signalHub").build();

connection.on("ReceiveMessage", function (message) {

    chrome.tabs.create({ url: "https://www.google.com" }, function(tab){
        
    });

});

connection.start().catch(function (err) {
    return console.error(err.toString());
});