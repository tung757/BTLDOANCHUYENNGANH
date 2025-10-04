function scrollToBottom() {
    let chatContent = $("#chatContent");
    chatContent.scrollTop(chatContent[0].scrollHeight);
}

$(document).ready(function () {
    // Bấm vào nút tư vấn (ngoại trừ dấu X) để mở chat
    $("#chatbotBtn").click(function (e) {
        if (!$(e.target).hasClass("chatbot-btn-close")) {
            $("#chatbotBox")
                .css("display", "flex") // ép flex
                .hide()
                .fadeIn();

            $("#chatbotBtn").fadeOut(); // ẩn nút tư vấn
            scrollToBottom();
        }
    });

    // Bấm dấu X trên khung chat -> đóng chat, hiện lại nút tư vấn
    $("#chatbotClose").click(function () {
        $("#chatbotBox").fadeOut();
        $("#chatbotBtn").fadeIn(); // hiện lại nút tư vấn
    });

    // Bấm dấu X trên nút tư vấn -> ẩn luôn nút
    $("#chatbotBtnClose").click(function (e) {
        e.stopPropagation(); // ngăn mở chat
        $("#chatbotBtn").fadeOut();
    });

    // Hàm gửi tin nhắn
    function sendMessage() {
        let msg = $("#chatInput").val();
        if (msg.trim() !== "") {
            $("#chatContent").append("<p><b>Bạn:</b> " + msg + "</p>");
            $("#chatInput").val("");
            scrollToBottom();

            setTimeout(function () {
                $("#chatContent").append("<p><b>Bot:</b> Đây là phản hồi tự động 🤖</p>");
                scrollToBottom();
            }, 500);
        }
    }

    // Bấm nút gửi
    $("#sendBtn").click(sendMessage);

    // Nhấn Enter để gửi
    $("#chatInput").keypress(function (e) {
        if (e.which === 13) {
            sendMessage();
            return false;
        }
    });
});

// jQuery demo nhỏ: alert khi đăng ký
$(".newsletter").on("submit", function (e) {
    e.preventDefault();
    alert("Bạn đã đăng ký thành công!");
});