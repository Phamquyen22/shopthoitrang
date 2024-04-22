function handleKeyDown(event) {
    if (event.key === 'Enter') {
        post_chat('/User/post_chat');
    }
}

function post_chat(url) {
    var form = document.getElementById('input_message');
    $.ajax({
        url: url,
        type: 'POST',
        data: { message: form.value },
        success: function (res) {
            if (res.result == true) {
                console.log("thành công");
                form.value = "";
                $.ajax({
                    url: '/User/message',
                    method: 'GET',
                    success: function (response) {
                        $('#message').html(response);
                    },
                    error: function (xhr, status, error) {
                        // Xử lý lỗi
                        console.error(error);
                    },
                })
            }
        },
    });
}
setInterval(function () {

    $.ajax({
        url: '/User/message',
        method: 'GET',
        success: function (response) {
           
            var mycontact = $('#message');

            if (response.trim() !== mycontact.html().trim()) {

                mycontact.html(response);
            }
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi
            console.error(error);
        },
    })

}, 2000);

