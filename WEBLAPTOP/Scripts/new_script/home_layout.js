const chatbotBtn = document.getElementById("chatbotBtn");
const chatbotClose = document.getElementById("chatbotClose");
const chatWindow = document.getElementById("chatWindow");

// Bấm X xanh -> ẩn hoàn toàn nút
chatbotClose.addEventListener("click", () => {
    chatbotBtn.style.display = "none";
    chatbotClose.style.display = "none";
    chatWindow.style.display = "none";
});

// Bấm nút chatbot -> mở/đóng khung chat
chatbotBtn.addEventListener("click", toggleChat);

function toggleChat() {
    chatWindow.style.display = (chatWindow.style.display === "flex") ? "none" : "flex";
}

// Gửi tin nhắn
function sendMessage() {
    const input = document.getElementById("userInput");
    const chatBody = document.getElementById("chatBody");
    if (input.value.trim() !== "") {
        const userMsg = document.createElement("div");
        userMsg.textContent = "Bạn: " + input.value;
        chatBody.appendChild(userMsg);

        // Giả lập phản hồi AI
        setTimeout(() => {
            const botMsg = document.createElement("div");
            botMsg.textContent = "AI: Tôi đã nhận được \"" + input.value + "\"";
            chatBody.appendChild(botMsg);
            chatBody.scrollTop = chatBody.scrollHeight;
        }, 800);

        input.value = "";
    }
}

// jQuery demo nhỏ: alert khi đăng ký
$(".newsletter").on("submit", function (e) {
    e.preventDefault();
    alert("Bạn đã đăng ký thành công!");
});