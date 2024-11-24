document.addEventListener("DOMContentLoaded", function () {
    const loadingOverlay = document.getElementById("loading-overlay");
    const navLinks = document.querySelectorAll(".nav-link");

    // Lắng nghe sự kiện click vào các liên kết trong navbar
    navLinks.forEach(link => {
        link.addEventListener("click", function (event) {
            event.preventDefault(); // Ngăn không cho chuyển trang ngay lập tức
            loadingOverlay.style.display = "flex"; // Hiển thị loading overlay

            // Đảm bảo loading hiển thị trong đúng 5 giây
            setTimeout(() => {
                loadingOverlay.style.display = "none"; // Ẩn loading overlay
                window.location.href = link.href; // Chuyển đến trang mới
            }, 5000); // Đảm bảo 5 giây
        });
    });
});
