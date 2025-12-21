
var imageList = [];
var currentIndex = 0;
$(document).ready(function () {
    $(".card_product").hover(function () {

        $(this).children(".quick_view").show();
    }, function () {
        $(this).children(".quick_view").hide();
    });

    // Click "Xem nhanh"
    $(".btn-quick-view").on("click", function (e) {
        e.preventDefault();
        e.stopPropagation();

        // Lấy dữ liệu từ thuộc tính data
        var id = $(this).data("id");
        var name = $(this).data("name");
        var price = $(this).data("price");
        var desc = $(this).data("description");

        // Xóa toàn bộ thẻ HTML, chỉ giữ text
        desc = $("<div>").html(desc).text();
        var imagesString = $(this).data("image");
        var slb = $(this).data("slb");
        var giathat = $(this).data("giathat");
        var giamgia = $(this).data("sell");

        imageList = imagesString.split(";").filter(img => img.trim() !== "");
        currentIndex = 0;


        if (imageList.length > 0) {
            $("#modalImage").attr("src", imageBaseUrl + imageList[0]);
        }
        // Gán vào modal
        $("#modalName").text(name);
        $("#modalgiamgia").text("-"+giamgia+"%");
        $("#soluongban").text("Đã bán " + slb);
        $("#modalrealPrice").text(giathat);
        $("#modalPrice").text(price);
        $("#modalDescription").text(desc);
       
        // Hiển thị các ảnh nhỏ bên dưới
        var thumbContainer = $("#thumbContainer");
        thumbContainer.empty(); // Xóa cũ


        imageList.forEach(function (img, index) {
            var thumb = $('<img>')
                .attr("src", "/Images/Product_images/" + img)
                .attr("data-index", index) 
                .addClass("img-thumbnail thumb-item me-2")
                .css({ width: "70px", height: "70px", objectFit: "cover", cursor: "pointer" })
                .on("click", function () {
                    $("#modalImage").attr("src", imageBaseUrl + img);
                });

            thumbContainer.append(thumb);
        });

        $(document).ready(function () {
            $("#btn_add_cart").click(function () {
                window.location.href = base_link_cart + "?id=" + id;
            })
        });

        $(document).ready(function () {
            $("#quick_buy").click(function () {
                window.location.href = base_link_quickbuy + "?id_sp=" + id;
            });
        });

        // Hiển thị modal
        var modal = new bootstrap.Modal(document.getElementById('quickViewModal'));
        modal.show();
    });
});

// Khi click vào thumbnail nhỏ
$(document).on("click", ".thumb-item", function () {
    var index = $(this).data("index");
    currentIndex = index;
    $("#modalImage").attr("src", imageBaseUrl + imageList[currentIndex]);

    // Đổi viền active
    $(".thumb-item").removeClass("border-primary");
    $(this).addClass("border-primary");
});

// Nút chuyển ảnh
$("#prevImage").on("click", function () {
    if (imageList.length === 0) return;
    currentIndex = (currentIndex - 1 + imageList.length) % imageList.length;
    updateMainImage();
});

$("#nextImage").on("click", function () {
    if (imageList.length === 0) return;
    currentIndex = (currentIndex + 1) % imageList.length;
    updateMainImage();
});

function updateMainImage() {
    $("#modalImage").attr("src", imageBaseUrl + imageList[currentIndex]);

    $(".thumb-item").removeClass("border-primary");
    $(".thumb-item").eq(currentIndex).addClass("border-primary");
}

//Lọc fiilter cho sản phẩm nhé
function onSortChange(sort) {
    window.location.href = base_link + '?sort=' + sort ;
}

function changedisplayproduct(display, sort) {
    window.location.href = base_link + '?sort=' + sort +'&display=' + display;
}

//Hiển thị thanh chạy giá để lọc
const minInput = document.getElementById('minPrice');
const maxInput = document.getElementById('maxPrice');
const minValue = document.getElementById('minPriceValue');
const maxValue = document.getElementById('maxPriceValue');

minInput.addEventListener('input', () => {
    if (parseInt(minInput.value) > parseInt(maxInput.value)) {
        maxInput.value = minInput.value;
    }
    minValue.textContent = parseInt(minInput.value).toLocaleString();
    maxValue.textContent = parseInt(maxInput.value).toLocaleString();
});

maxInput.addEventListener('input', () => {
    if (parseInt(maxInput.value) < parseInt(minInput.value)) {
        minInput.value = maxInput.value;
    }
    maxValue.textContent = parseInt(maxInput.value).toLocaleString();
    minValue.textContent = parseInt(minInput.value).toLocaleString();
});

//Xử lý lọc theo giá
function groupby_price() {
    var str = $("#minPriceValue").text();
    var price_start = parseInt(str.replace(/\./g, ""), 10);
    str = $("#maxPriceValue").text();
    var price_end = parseInt(str.replace(/\./g, ""), 10);
    window.location.href = base_link + "?price_start=" + price_start + "&price_end=" +price_end;
}

//button lọc danh mục
function btn_dm(value) {
    window.location.href = base_link + "?categories_id=" + value;
}


