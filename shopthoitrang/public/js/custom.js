function orderlist(url) {
    var orderlistUrl =url;
    $.ajax({
        url: orderlistUrl, // Gọi action GetForm trong HomeController
        method: 'GET',
        success: function (response) {
            $('#formContainer').html(response);
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi
            console.error(error);
        }
    });
}
function favouritelist(url) {
    var orderlistUrl = url;
    $.ajax({
        url: orderlistUrl, // Gọi action GetForm trong HomeController
        method: 'GET',
        success: function (response) {
            $('#formContainer').html(response);
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi
            console.error(error);
        }
    });
}
function update_acc(url) {
    var form = document.getElementById('update_acc');
    var formData = new FormData(form);

    $.ajax({
        url: url,
        type: 'POST',
        processData: false,
        contentType: false,
        data: formData,

        success: function (res) {
            if (res.result == true) {
                console.log("thay đổi thành công");
                var notification = document.getElementById('notification');
                notification.style.display = "block";
                setTimeout(function () {
                    notification.style.display = "none";
                }, 2000);
            } else {
                console.log("lỗi mật khẩu");
            }
        },
    });
}

function yeu(idbutton, id_product,url) {

    $.ajax({
        url: url,
        type: 'POST',
        data: {
            id_pro: id_product,
        },
        success: function (response) {
            if (response.success == true) {
                console.log('Thêm sản phẩm yêu thích thành công');
                var yeuthichElement = document.getElementById(idbutton);

                if (yeuthichElement.getAttribute('src') == '/public/img/icon/heart_red.png') {
                    yeuthichElement.setAttribute('src', '/public/img/icon/heart.png');
                } else {
                    yeuthichElement.setAttribute('src', '/public/img/icon/heart_red.png');
                }
            }
            else if (response.redirectUrl != null) {
                window.location.href = response.redirectUrl;
            }
        },
        error: function (xhr, status, error) {
            // Handle errors, if any
            console.error(xhr.responseText);
        }
    })


}

function nextpage(url,pageid,kq) {
    var orderlistUrl = url;
    $.ajax({
        url: orderlistUrl, // Gọi action GetForm trong HomeController
        method: 'GET',
        success: function (response) {
            $('#nextpage').html(response);
            for (var i = 1; i <= kq+1; i++) {
                if (i == pageid) {
                    var page = document.getElementById('page' + i);
                    page.className = 'active';
                }
                else {
                    var page = document.getElementById('page' + i);
                    page.className = '';
                }
            }
            console.log("page next");
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi
            console.error(error);
        }
    });
}


function category(url) {
    var orderlistUrl = url;
    $.ajax({
        url: orderlistUrl, // Gọi action GetForm trong HomeController
        method: 'GET',
        success: function (response) {
            $('#layout_product').html(response);
            
            console.log("page next");
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi
            console.error(error);
        }
    });
}


function show() {
    var s = document.getElementById('upfile');
    if (s.style.display != 'block')
        s.style.display = 'block';
    else
        s.style.display = 'none';
}
function order_list_hide() {
    var s = document.getElementById('deltail_order');
    if (s.style.display != 'block')
        s.style.display = 'block';
    else
        s.style.display = 'none';
}
function order_list(url) {
    var orderlistUrl = url;
    $.ajax({
        url: orderlistUrl, // Gọi action GetForm trong HomeController
        method: 'GET',
        success: function (response) {
            $('#list_product').html(response);
            
            var s = document.getElementById('deltail_order');
            if (s.style.display != 'block')
                s.style.display = 'block';
            else
                s.style.display = 'none';
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi
            console.error(error);
        }
    });
    
}