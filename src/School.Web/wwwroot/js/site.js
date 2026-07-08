(function ($) {
    'use strict';

    $(document).ready(function () {

        initCounterAnimation();
        initBackToTop();
        initCookieConsent();
        initFormValidation();
        initLightbox();
        initTooltips();
        initSmoothScroll();
        initVisitorCounter();
        initActiveNavHighlight();

        function initCounterAnimation() {
            var counters = $('.stat-number, .achievement-number');
            if (counters.length === 0) return;

            var options = {
                root: null,
                rootMargin: '0px',
                threshold: 0.5
            };

            var observer = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting) {
                        var counter = $(entry.target);
                        var target = parseInt(counter.data('target')) || 0;
                        animateCounter(counter, target);
                        observer.unobserve(entry.target);
                    }
                });
            }, options);

            counters.each(function () {
                observer.observe(this);
            });
        }

        function animateCounter(element, target) {
            var current = 0;
            var increment = Math.ceil(target / 60);
            var timer = setInterval(function () {
                current += increment;
                if (current >= target) {
                    current = target;
                    clearInterval(timer);
                }
                element.text(current.toLocaleString());
            }, 25);
        }

        function initBackToTop() {
            var backToTop = $('#backToTop');
            if (backToTop.length === 0) return;

            $(window).on('scroll', function () {
                if ($(this).scrollTop() > 300) {
                    backToTop.addClass('show');
                } else {
                    backToTop.removeClass('show');
                }
            });

            backToTop.on('click', function (e) {
                e.preventDefault();
                $('html, body').animate({ scrollTop: 0 }, 500);
            });
        }

        function initCookieConsent() {
            var cookieConsent = $('#cookieConsent');
            if (cookieConsent.length === 0) return;

            if (!getCookie('cookieConsent')) {
                cookieConsent.addClass('show');
            }

            $('#cookieAccept').on('click', function () {
                setCookie('cookieConsent', 'accepted', 365);
                cookieConsent.removeClass('show');
            });

            $('#cookieDecline').on('click', function () {
                setCookie('cookieConsent', 'declined', 30);
                cookieConsent.removeClass('show');
            });
        }

        function setCookie(name, value, days) {
            var expires = '';
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                expires = '; expires=' + date.toUTCString();
            }
            document.cookie = name + '=' + (value || '') + expires + '; path=/';
        }

        function getCookie(name) {
            var nameEQ = name + '=';
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) === ' ') c = c.substring(1, c.length);
                if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
            }
            return null;
        }

        function initFormValidation() {
            var forms = document.querySelectorAll('.needs-validation');
            Array.prototype.slice.call(forms).forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    if (!form.checkValidity()) {
                        event.preventDefault();
                        event.stopPropagation();
                    }
                    form.classList.add('was-validated');
                }, false);
            });
        }

        function initLightbox() {
            if (typeof lightbox !== 'undefined') {
                lightbox.option({
                    'resizeDuration': 200,
                    'wrapAround': true,
                    'albumLabel': 'Image %1 of %2'
                });
            }
        }

        function initTooltips() {
            var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
            tooltipTriggerList.map(function (el) {
                return new bootstrap.Tooltip(el);
            });
        }

        function initSmoothScroll() {
            $('a[href*="#"]:not([href="#"])').on('click', function () {
                if (location.pathname.replace(/^\//, '') === this.pathname.replace(/^\//, '')
                    && location.hostname === this.hostname) {
                    var target = $(this.hash);
                    target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
                    if (target.length) {
                        $('html, body').animate({
                            scrollTop: target.offset().top - 80
                        }, 800);
                        return false;
                    }
                }
            });
        }

        function initVisitorCounter() {
            var visitorEl = $('#visitorCount');
            if (visitorEl.length === 0) return;

            var stored = localStorage.getItem('visitorCount');
            if (stored) {
                visitorEl.text(stored);
            } else {
                var count = Math.floor(Math.random() * 10000) + 5000;
                visitorEl.text(count.toLocaleString());
                localStorage.setItem('visitorCount', count);
            }
        }

        function initActiveNavHighlight() {
            var path = window.location.pathname.toLowerCase();
            $('.main-nav .nav-link').each(function () {
                var href = $(this).attr('href');
                if (href && path.indexOf(href.toLowerCase()) !== -1 && href !== '/') {
                    $(this).addClass('active');
                }
            });
        }
    });

})(jQuery);
