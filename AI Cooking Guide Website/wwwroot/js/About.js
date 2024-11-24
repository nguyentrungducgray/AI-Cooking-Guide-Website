// Khởi tạo hiệu ứng Parallax khi cuộn trang
window.addEventListener('scroll', function () {
    const scrollY = window.scrollY;
    const parallaxElements = document.querySelectorAll('.parallax-effect');
    parallaxElements.forEach(function (element) {
        element.style.backgroundPosition = 'center ' + (scrollY * 0.5) + 'px';
    });
});

// Khởi tạo hiệu ứng cho các phần tử trên trang
document.addEventListener('DOMContentLoaded', function () {
    AOS.init({
        duration: 1000,
        easing: 'ease-in-out',
        once: true,
    });
});
// Khởi tạo Auto-Slide cho Carousel (Image and Content)
document.addEventListener("DOMContentLoaded", function () {
    const carouselImage = new bootstrap.Carousel(document.querySelector('#carouselExample'), {
        interval: 5000,  // Đổi ảnh sau mỗi 5 giây
        ride: 'carousel',
    });

    const carouselContent = new bootstrap.Carousel(document.querySelector('#contentCarousel'), {
        interval: 5000,  // Đổi nội dung sau mỗi 5 giây
        ride: 'carousel',
    });
});
// JavaScript để kích hoạt hiệu ứng khi cuộn
document.addEventListener('DOMContentLoaded', function () {
    const steps = document.querySelectorAll('.step');

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
            }
        });
    }, { threshold: 0.5 });

    steps.forEach(step => observer.observe(step));
});
 