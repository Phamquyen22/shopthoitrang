function order_list(url) {
    var orderlistUrl = url;
    $.ajax({
        url: orderlistUrl, // Gọi action GetForm trong HomeController
        method: 'GET',
        success: function (response) {
            $('#list_order').html(response);
            
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi
            console.error(error);
        }
    });

}


function xacnhan_order(url,id,mes) {
    $.ajax({
        url: url,
        method: 'POST',
        success: function (res) {
            if (res.sucsses) {
                var a = document.getElementById(id);
                a.className = "badge badge-warning";
                a.innerHTML = mes;
            }
        }
    })
}

function xoa_order(url, id) {
    $.ajax({
        url: url,
        method: 'POST',
        success: function (res) {
            if (res.sucsses) {
                var a = document.getElementById(id);
                a.innerHTML = "";
            }
        }
    })
}



function thanhcong_order(url, id, mes) {
    $.ajax({
        url: url,
        method: 'POST',
        success: function (res) {
            if (res.sucsses) {
                var a = document.getElementById(id);
                a.className = "badge badge-success";
                a.innerHTML = mes;
            }
        }
    })
}



function markall(url) {
    $.ajax({
        url: url,
        method: 'POST',
        success: function (res) {
            if (res.sucsses) {
                console.log('done');
            }
        }
    })
}

function chat(url) {
    $.ajax({
        url: url,
        method: 'GET',
        success: function (res) {
            $('#mycontact').html(res);
            
        }
    })
}

var scrollableDiv = document.getElementById('mycontact');

var prevHeight = scrollableDiv.scrollHeight;

setInterval(function () {

    if (scrollableDiv.scrollHeight !== prevHeight) {

        prevHeight = scrollableDiv.scrollHeight;

        scrollableDiv.scrollTop = scrollableDiv.scrollHeight;
    }
}, 500);
function post_chat(url) {
    var form = document.getElementById('input_message');
    var a = document.getElementById('id_chat');
    $.ajax({
        url: url,
        type: 'POST',
        data: { message: form.value, id: a.value },
        success: function (res) {
            if (res.result == true) {
                console.log("thành công");
                form.value = "";
            }
        },
    });
}

setInterval(function () {
    var a = document.getElementById('id_chat');
    if (a != null) {
        $.ajax({
            url: '/admin/Contact/chat',
            method: 'GET',
            data: { id: a.value },
            success: function (response) {
                $('#mycontact').html(response);
            },
            error: function (xhr, status, error) {
                // Xử lý lỗi
                console.error(error);
            },
        })
    }

}, 2000);


function loctheothang(url,id) {
    $.ajax({
        url: url,
        type: 'POST',
        success: function (res) {
            if (res.sucsses == true) {
                console.log("thành công thang");
                var total = document.getElementById('total');
                var Pending = document.getElementById('Pending');
                var Shipping = document.getElementById('Shipping');
                var Completed = document.getElementById('Completed');
                var chonthang = document.getElementById('orders-month');
                var xuatfile = document.getElementById('xuatfile');
                chonthang.innerText = document.getElementById(id).innerText;
                xuatfile.setAttribute('href', '/admin/admin/ExportToExcel?thang='+id);
                total.innerText =res.total;
                Pending.innerText = res.Pending;
                Shipping.innerText = res.Shipping;
                Completed.innerText = res.Completed;
                
            }
        },
    });
}

