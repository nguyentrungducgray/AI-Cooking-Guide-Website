document.addEventListener('DOMContentLoaded', function () {
    // Lấy phần tử cần đọc
    const readButton = document.getElementById("read-content");
    const readAgainButton = document.getElementById("read-again");
    const content = document.getElementById("container"); // Đảm bảo ID là 'container'
    let isReading = false; // Biến trạng thái đọc
    let isPaused = false; // Biến trạng thái tạm dừng
    let speechInstance; // Thể hiện của SpeechSynthesisUtterance
    let textToRead = ''; // Biến lưu trữ nội dung cần đọc

    // Hàm đọc nội dung bằng SpeechSynthesis API
    function readText(text) {
        if ('speechSynthesis' in window) {
            speechInstance = new SpeechSynthesisUtterance();
            speechInstance.text = text;
            speechInstance.lang = 'vi-VN'; // Cài đặt ngôn ngữ mặc định là tiếng việt
            speechInstance.rate = 1;
            speechInstance.pitch = 1;

            // Hủy bỏ các bài đọc trước để không chồng lặp
            window.speechSynthesis.cancel();

            // Bắt đầu đọc
            window.speechSynthesis.speak(speechInstance);
            isReading = true; // Đặt trạng thái là đang đọc
            isPaused = false; // Đặt trạng thái là không tạm dừng

            // Cập nhật nội dung nút
            readButton.textContent = "Tạm dừng"; // Khi bắt đầu đọc, nút chuyển thành "Tạm dừng"
            readButton.classList.add('reading'); // Thêm lớp để thay đổi kiểu dáng khi đang đọc

        } else {
            alert("Trình duyệt của bạn không hỗ trợ đọc văn bản!");
        }
    }

    // Hàm tạm dừng hoặc tiếp tục đọc
    function togglePause() {
        if (isPaused) {
            window.speechSynthesis.resume(); // Tiếp tục đọc
            isPaused = false;
            readButton.textContent = "Tạm dừng"; // Khi tiếp tục, nút là "Tạm dừng"
        } else {
            window.speechSynthesis.pause(); // Tạm dừng đọc
            isPaused = true;
            readButton.textContent = "Đọc tiếp"; // Khi tạm dừng, nút là "Đọc tiếp"
        }
    }

    // Hàm đọc lại từ đầu
    function readAgain() {
        readText(textToRead); // Đọc lại từ đầu
        readButton.textContent = "Tạm dừng"; // Đổi lại nội dung nút khi bắt đầu đọc lại
    }

    // Lắng nghe sự kiện khi nhấn nút và điều khiển đọc/tạm dừng
    readButton.addEventListener("click", () => {
        if (isReading) {
            togglePause(); // Nếu đang đọc, chuyển giữa tạm dừng và tiếp tục
        } else {
            textToRead = content.innerText; // Lưu lại nội dung cần đọc
            readText(textToRead); // Bắt đầu đọc
        }
    });

    // Lắng nghe sự kiện khi nhấn nút "Đọc lại từ đầu"
    readAgainButton.addEventListener("click", readAgain);
});
