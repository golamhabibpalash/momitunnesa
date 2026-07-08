(function ($) {
    'use strict';

    $(document).ready(function () {

        initSidebarToggle();
        initThemeToggle();
        initDataTable();
        initTinyMCE();
        initImagePreview();
        initFormValidation();
        initNotificationAutoHide();
        initConfirmDialogs();
        initSelectSearch();

        function initSidebarToggle() {
            var sidebarToggle = $('.sidebar-toggle');
            var sidebar = $('.admin-sidebar');

            sidebarToggle.on('click', function () {
                sidebar.toggleClass('collapsed');
                $(window).trigger('resize');
            });

            if ($(window).width() <= 768) {
                $('.admin-sidebar .nav-link').on('click', function () {
                    if ($(this).attr('data-bs-toggle') !== 'collapse') {
                        sidebar.removeClass('mobile-open');
                    }
                });

                $('.sidebar-toggle').on('click', function () {
                    sidebar.toggleClass('mobile-open');
                });
            }
        }

        function initThemeToggle() {
            var themeToggle = $('#themeToggle');
            if (themeToggle.length === 0) return;

            var currentTheme = localStorage.getItem('adminTheme') || 'light';
            applyTheme(currentTheme);

            themeToggle.on('click', function () {
                var newTheme = $('body').attr('data-theme') === 'dark' ? 'light' : 'dark';
                applyTheme(newTheme);
                localStorage.setItem('adminTheme', newTheme);
            });
        }

        function applyTheme(theme) {
            if (theme === 'dark') {
                $('body').attr('data-theme', 'dark');
                $('#themeToggle i').removeClass('fa-moon').addClass('fa-sun');
            } else {
                $('body').removeAttr('data-theme');
                $('#themeToggle i').removeClass('fa-sun').addClass('fa-moon');
            }
        }

        function initDataTable() {
            if (typeof $.fn.DataTable === 'undefined') return;

            $('.table-datatable').each(function () {
                var config = {
                    pageLength: 25,
                    responsive: true,
                    searching: true,
                    ordering: true,
                    language: {
                        search: '_INPUT_',
                        searchPlaceholder: 'Search...',
                        lengthMenu: 'Show _MENU_ entries',
                        info: 'Showing _START_ to _END_ of _TOTAL_ entries',
                        infoEmpty: 'No entries found',
                        infoFiltered: '(filtered from _MAX_ total entries)',
                        zeroRecords: 'No matching records found',
                        paginate: {
                            first: '<i class="fas fa-angle-double-left"></i>',
                            previous: '<i class="fas fa-angle-left"></i>',
                            next: '<i class="fas fa-angle-right"></i>',
                            last: '<i class="fas fa-angle-double-right"></i>'
                        }
                    },
                    dom: '<"row"<"col-sm-6"l><"col-sm-6"f>>' +
                         '<"row"<"col-sm-12"tr>>' +
                         '<"row"<"col-sm-5"i><"col-sm-7"p>>'
                };

                if ($(this).data('export')) {
                    config.dom = 'Bfrtip';
                    config.buttons = [
                        { extend: 'copy', className: 'btn btn-sm btn-outline-secondary' },
                        { extend: 'csv', className: 'btn btn-sm btn-outline-success' },
                        { extend: 'excel', className: 'btn btn-sm btn-outline-primary' },
                        { extend: 'pdf', className: 'btn btn-sm btn-outline-danger' },
                        { extend: 'print', className: 'btn btn-sm btn-outline-info' }
                    ];
                }

                $(this).DataTable(config);
            });
        }

        function initTinyMCE() {
            if (typeof tinymce === 'undefined') return;

            $('.tinymce-editor').each(function () {
                var height = $(this).data('height') || 500;
                tinymce.init({
                    selector: '#' + $(this).attr('id'),
                    height: height,
                    menubar: true,
                    plugins: [
                        'advlist', 'autolink', 'lists', 'link', 'image', 'charmap',
                        'preview', 'anchor', 'searchreplace', 'visualblocks', 'code',
                        'fullscreen', 'insertdatetime', 'media', 'table', 'help', 'wordcount'
                    ],
                    toolbar: 'undo redo | blocks | ' +
                        'bold italic backcolor | alignleft aligncenter ' +
                        'alignright alignjustify | bullist numlist outdent indent | ' +
                        'removeformat | image media link | code fullscreen | help',
                    content_style: 'body { font-family: -apple-system, BlinkMacSystemFont, sans-serif; font-size: 16px; line-height: 1.6; }',
                    relative_urls: false,
                    remove_script_host: false,
                    document_base_url: $('base').attr('href') || '/'
                });
            });
        }

        function initImagePreview() {
            $('.image-input').on('change', function () {
                var wrapper = $(this).closest('.image-upload-wrapper');
                var img = wrapper.find('img');
                var placeholder = wrapper.find('.upload-placeholder');
                var file = this.files[0];

                if (file) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        img.attr('src', e.target.result).show();
                        if (placeholder) placeholder.hide();
                    };
                    reader.readAsDataURL(file);
                }
            });
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

        function initNotificationAutoHide() {
            $('.alert-auto-hide').each(function () {
                var alert = $(this);
                var delay = alert.data('delay') || 5000;
                setTimeout(function () {
                    alert.fadeOut(500, function () { $(this).remove(); });
                }, delay);
            });
        }

        function initConfirmDialogs() {
            $('[data-confirm]').on('click', function (e) {
                var message = $(this).data('confirm') || 'Are you sure?';
                if (!confirm(message)) {
                    e.preventDefault();
                }
            });
        }

        function initSelectSearch() {
            if ($.fn.select2) {
                $('.select-search').select2({
                    theme: 'bootstrap-5',
                    width: '100%'
                });
            }
        }
    });

})(jQuery);
