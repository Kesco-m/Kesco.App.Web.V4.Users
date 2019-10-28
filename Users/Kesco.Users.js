$(document).ready(function() {
    $(window).resize(function() {
        resize();
    });
    resize();
});

function resize() {
    var hr = $(window).height() - 49;
    $("#UserPanel").height(hr);
}

