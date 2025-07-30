/**
 * Spike Components JavaScript
 * Reusable JavaScript functions for Spike Bootstrap components
 */

// ===== CHART UTILITIES =====
const SpikeCharts = {
    // Default chart colors
    colors: {
        primary: '#0085db',
        success: '#4bd08b', 
        danger: '#fb977d',
        warning: '#f8c076',
        info: '#46caeb',
        secondary: '#6c757d'
    },

    // Create profit chart
    createProfitChart: function(elementId, options = {}) {
        const defaultOptions = {
            series: [
                {
                    name: "Doanh thu",
                    data: [9, 5, 3, 7, 5, 10, 3],
                },
                {
                    name: "Chi phí", 
                    data: [6, 3, 9, 5, 4, 6, 4],
                },
            ],
            chart: {
                fontFamily: "Plus Jakarta Sans, sans-serif",
                type: "bar",
                height: 360,
                offsetY: 10,
                toolbar: { show: false },
            },
            grid: {
                show: true,
                strokeDashArray: 3,
                borderColor: "rgba(0,0,0,.1)",
            },
            colors: [this.colors.primary, this.colors.success],
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: "30%",
                    endingShape: "flat",
                },
            },
            dataLabels: { enabled: false },
            stroke: {
                show: true,
                width: 5,
                colors: ["transparent"],
            },
            xaxis: {
                type: "category",
                categories: ['T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'CN'],
                axisTicks: { show: false },
                axisBorder: { show: false },
                labels: { style: { colors: "#a1aab2" } },
            },
            yaxis: {
                labels: { style: { colors: "#a1aab2" } },
            },
            fill: { opacity: 1 },
            tooltip: {
                theme: "dark",
                y: {
                    formatter: function (val) {
                        return val + " VNĐ";
                    },
                },
            },
            legend: {
                show: true,
                position: "top",
                horizontalAlign: "left",
                labels: { colors: "#a1aab2" },
            },
        };

        const mergedOptions = { ...defaultOptions, ...options };
        const chart = new ApexCharts(document.querySelector(elementId), mergedOptions);
        chart.render();
        return chart;
    },

    // Create donut chart
    createDonutChart: function(elementId, data = [55, 45], options = {}) {
        const defaultOptions = {
            series: data,
            chart: {
                type: "donut",
                fontFamily: "Plus Jakarta Sans, sans-serif",
                height: 130,
            },
            colors: [this.colors.primary, this.colors.danger],
            plotOptions: {
                pie: {
                    donut: {
                        size: "70%",
                        background: "none",
                        labels: { show: false },
                    },
                },
            },
            stroke: { show: false },
            dataLabels: { enabled: false },
            legend: { show: false },
            tooltip: {
                theme: "dark",
                fillSeriesColor: false,
            },
        };

        const mergedOptions = { ...defaultOptions, ...options };
        const chart = new ApexCharts(document.querySelector(elementId), mergedOptions);
        chart.render();
        return chart;
    },

    // Create area chart
    createAreaChart: function(elementId, data = [25, 66, 20, 40, 12, 58, 20], options = {}) {
        const defaultOptions = {
            series: [{
                name: "Doanh số",
                data: data,
            }],
            chart: {
                type: "area",
                height: 60,
                sparkline: { enabled: true },
                group: "sparklines",
                fontFamily: "Plus Jakarta Sans, sans-serif",
            },
            stroke: {
                curve: "smooth",
                width: 2,
            },
            fill: {
                colors: [this.colors.primary],
                type: "solid",
                opacity: 0.05,
            },
            markers: { size: 0 },
            tooltip: {
                theme: "dark",
                fixed: {
                    enabled: true,
                    position: "right",
                },
                x: { show: false },
            },
        };

        const mergedOptions = { ...defaultOptions, ...options };
        const chart = new ApexCharts(document.querySelector(elementId), mergedOptions);
        chart.render();
        return chart;
    }
};

// ===== NOTIFICATION UTILITIES =====
const SpikeNotifications = {
    // Show success notification
    success: function(title, message, timer = 3000) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'success',
                title: title,
                text: message,
                timer: timer,
                showConfirmButton: false,
                toast: true,
                position: 'top-end'
            });
        } else {
            alert(title + ': ' + message);
        }
    },

    // Show error notification
    error: function(title, message, timer = 3000) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'error',
                title: title,
                text: message,
                timer: timer,
                showConfirmButton: false,
                toast: true,
                position: 'top-end'
            });
        } else {
            alert(title + ': ' + message);
        }
    },

    // Show warning notification
    warning: function(title, message, timer = 3000) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'warning',
                title: title,
                text: message,
                timer: timer,
                showConfirmButton: false,
                toast: true,
                position: 'top-end'
            });
        } else {
            alert(title + ': ' + message);
        }
    },

    // Show info notification
    info: function(title, message, timer = 3000) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'info',
                title: title,
                text: message,
                timer: timer,
                showConfirmButton: false,
                toast: true,
                position: 'top-end'
            });
        } else {
            alert(title + ': ' + message);
        }
    }
};

// ===== TABLE UTILITIES =====
const SpikeTables = {
    // Initialize DataTable with Spike styling
    initDataTable: function(tableId, options = {}) {
        const defaultOptions = {
            responsive: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Tất cả"]],
            language: {
                url: '//cdn.datatables.net/plug-ins/1.11.5/i18n/vi.json'
            },
            dom: '<"row"<"col-sm-6"l><"col-sm-6"f>>' +
                 '<"row"<"col-sm-12"tr>>' +
                 '<"row"<"col-sm-5"i><"col-sm-7"p>>',
            ...options
        };

        return $(tableId).DataTable(defaultOptions);
    }
};

// ===== FORM UTILITIES =====
const SpikeForms = {
    // Validate form
    validateForm: function(formId) {
        const form = document.querySelector(formId);
        if (!form) return false;

        let isValid = true;
        const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
        
        inputs.forEach(input => {
            if (!input.value.trim()) {
                input.classList.add('is-invalid');
                isValid = false;
            } else {
                input.classList.remove('is-invalid');
                input.classList.add('is-valid');
            }
        });

        return isValid;
    },

    // Reset form validation
    resetValidation: function(formId) {
        const form = document.querySelector(formId);
        if (!form) return;

        const inputs = form.querySelectorAll('.is-invalid, .is-valid');
        inputs.forEach(input => {
            input.classList.remove('is-invalid', 'is-valid');
        });
    }
};

// ===== INITIALIZATION =====
$(document).ready(function() {
    // Initialize tooltips
    $('[data-bs-toggle="tooltip"]').tooltip();

    // Initialize popovers
    $('[data-bs-toggle="popover"]').popover();

    // Add fade-in animation to cards
    $('.card').addClass('fade-in');

    // Handle dropdown menu clicks
    $('.dropdown-menu a').on('click', function(e) {
        if ($(this).attr('href') === '#') {
            e.preventDefault();
        }
    });
});

// ===== EXPORT =====
window.SpikeCharts = SpikeCharts;
window.SpikeNotifications = SpikeNotifications;
window.SpikeTables = SpikeTables;
window.SpikeForms = SpikeForms;
