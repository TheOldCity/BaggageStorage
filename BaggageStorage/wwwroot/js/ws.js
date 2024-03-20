var websocket;


$(function () {
    //initBroadcast($("#wsUrl").val());
});


function initBroadcast(uri) {
    console.log('Connecting to: ' + uri);

    websocket = new WebSocket(uri);

    websocket.onopen = function () {
        console.log('Connected.');
        $("#connection_lost").dxLoadPanel('instance').hide();
    };

    websocket.onerror = function () {
        console.log('WebSocket error.');
    };

    websocket.onclose = function () {

        $("#connection_lost").dxLoadPanel('instance').show();

        console.log('WebSocket closed.');

        var timer = window.setInterval(function () {
            window.clearInterval(timer);
            initBroadcast(uri);
        }, 5000);

    };

    websocket.onmessage = function (event) {
        console.log(event.data);
        var obj = JSON.parse(event.data);
        switch (obj.messageType) {
            case -1:
                websocket.close();                
                break;
            case -2:
                location.href = "/";
                break;
            case 0:
                $.ajax({
                    type: "GET",
                    url: "/account/logout"
                })
                    .always(function () {
                        location.href = '/';
                    });

                break;
            default:
                break;
        }
    };
};
