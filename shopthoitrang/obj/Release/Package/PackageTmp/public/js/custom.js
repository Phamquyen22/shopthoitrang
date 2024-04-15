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


function formatCurrency(amount) {
    return amount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function soluong(id_product, gia) {
    var onhap = document.getElementById('soluonghang' + id_product).value;
    var tong = document.getElementById('tongtien' + id_product);
    var sl = parseInt(onhap);
    var giasp = sl * gia;
    var tongtien = document.getElementById('totaltien');

    tong.innerHTML = formatCurrency(giasp) + '₫';

    $.ajax({
        url: '/Cart/update_cart',
        type: 'POST',
        data: {
            id_pro: id_product,
            quantity: sl,
        },
        success: function (response) {
            if (response.success === true) {

                $.ajax({
                    url: '/Cart/resulttotal',
                    type: 'POST',

                    success: function (response) {
                        if (response.success) {
                            console.log('cập nhập giá thành công ');
                            tongtien.innerHTML = response.giatien;
                        }
                        else if (response.redirectUrl != null) {
                            window.location.href = response.redirectUrl;
                        }
                    },
                    error: function (xhr, status, error) {

                        console.error(xhr.responseText);
                    }

                });
            }
            else if (response.redirectUrl != null) {
                window.location.href = response.redirectUrl;
            }
        },
        error: function (xhr, status, error) {

            console.error(xhr.responseText);
        }

    });

}


function xoacart(url, id) {
    $.ajax({
        url: url,
        method: 'POST',
        success: function (response) {
            var a = document.getElementById(id);
            a.innerHTML = "";
            $.ajax({
                url: '/Cart/resulttotal',
                type: 'POST',
                success: function (response) {
                    if (response.success) {
                        var tongtien = document.getElementById('totaltien');
                        console.log('cập nhập giá thành công ');
                        tongtien.innerHTML = response.giatien;
                    }
                },
                error: function (xhr, status, error) {

                    console.error(xhr.responseText);
                }

            });
        }
    });
}

function them(id_product) {
    $.ajax({
        url: '/Shop"/favourite',
        type: 'POST',
        data: {
            id_pro: id_product,
        },
    })
    var tomau = document.getElementById('tomau');
    if (tomau.style.color == 'palevioletred') {
        tomau.style.color = '';
    }
    else {
        tomau.style.color = 'palevioletred';
    }
    var notification = document.getElementById('notification');
    notification.style.display = "block";
    setTimeout(function () {
        notification.style.display = "none";
    }, 2000);
}
function themgiohang() {
    var form = document.getElementById('myform');
    var formData = new FormData(form);

    $.ajax({
        url: '/Cart/addtocart',
        type: 'POST',
        processData: false,
        contentType: false,
        data: formData,
        success: function (response) {
            if (response.success === true) {
                console.log('Thêm sản phẩm vào giỏ hàng thành công');
                alert("Thêm sản phẩm vào giỏ hàng thành công");
            } else if (response.redirectUrl != null) {
                window.location.href = response.redirectUrl;
            }
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi, nếu có
            console.error(xhr.responseText);
        }
    });
}