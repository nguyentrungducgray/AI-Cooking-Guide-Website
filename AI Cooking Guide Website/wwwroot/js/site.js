// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", () => {
    const links = document.querySelectorAll("a.nav-link");
    const body = document.body;

    // Thêm hiệu ứng fade-in khi trang tải
    body.classList.add("fade-in");

    // Áp dụng hiệu ứng fade-out khi nhấn vào link
    links.forEach(link => {
        link.addEventListener("click", event => {
            event.preventDefault(); // Ngăn việc chuyển trang ngay lập tức
            const href = link.getAttribute("href");

            // Thêm lớp fade-out
            body.classList.add("fade-out");

            // Đợi hiệu ứng kết thúc rồi chuyển trang
            setTimeout(() => {
                window.location.href = href;
            }, 800); // Thời gian phải khớp với animation CSS
        });
    });
});


// Write your JavaScript code.
