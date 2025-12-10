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


    function sendMessage() {
        let msg = $("#chatInput").val();

        if (msg.trim() !== "") {
            // Hiển thị tin nhắn người dùng
            $("#chatContent").append("<p><b>Bạn:</b> " + msg + "</p>");
            $("#chatInput").val("");
            scrollToBottom();

            // Gửi request đến API
            fetch("/api/advice", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ question: msg })
            })
                .then(response => response.json())
                .then(data => {
                    let botMsg = (data.answer || "Bot không phản hồi.").replace(/\n/g, "<br>");
                    $("#chatContent").append("<p><b>Bot:</b> " + botMsg + "</p>");
                    scrollToBottom();
                })
                .catch(err => {
                    $("#chatContent").append("<p><b>Bot:</b> Lỗi kết nối đến server!</p>");
                    scrollToBottom();
                });
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

    // Hàm gửi tin nhắn
    //async function sendMessage() {
    //    async function callapi(msg_chat) {
    //        const response = await fetch("https://api.openai.com/v1/chat/completions", {

    //            method: "POST",

    //            headers: {

    //                "Content-Type": "application/json",

    //                "Authorization": "Bearer "

    //            },

    //            body: JSON.stringify({

    //                model: "gpt-4o",

    //                messages: [

    //                    { role: "system", content: "You are a helpful assistant." },

    //                    { role: "user", content: msg_chat }

    //                ]

    //            })

    //        });
    //        const data = await response.json();
    //        return data.choices[0].message.content;
    //    }
    //    let msg = $("#chatInput").val();
    //    if (msg.trim() !== "") {
    //        $("#chatContent").append("<p><b>Bạn:</b> " + msg + "</p>");
    //        $("#chatInput").val("");
    //        scrollToBottom();
    //        const reply = await callapi(msg);
    //        setTimeout(function () {
    //            $("#chatContent").append("<p><b>Bot:</b>" + reply + "</p>");
    //            scrollToBottom();
    //        }, 500);
    //    }
    //}

    //// Bấm nút gửi
    //$("#sendBtn").click(sendMessage);

    //// Nhấn Enter để gửi
    //$("#chatInput").keypress(function (e) {
    //    if (e.which === 13) {
    //        sendMessage();
    //        return false;
    //    }
    //});
});

// jQuery demo nhỏ: alert khi đăng ký
$(".newsletter").on("submit", function (e) {
    e.preventDefault();
    alert("Bạn đã đăng ký thành công!");
});



$(document).ready(function () {
    $(".item_diachi").click(function () {
        let dc = $(this).text();
        $(".diachi_ht").text(dc);
    });
});