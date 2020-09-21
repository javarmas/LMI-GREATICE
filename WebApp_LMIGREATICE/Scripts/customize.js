$(window).bind("load", function () {
    setTimeout(function () {
        $.get("/systemUsers/downAllMessages", { }, function (data) {
            $('#messageNotification').fadeOut().empty();
            $('#messageError').fadeOut().empty();
            $('#message').fadeOut().empty();
        });
    }, 10000);
});