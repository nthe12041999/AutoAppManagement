(function($) {
  'use strict';
  $(function() {
    if ($("#performanceLine").length) { 
      const ctx = document.getElementById('performanceLine');
      var graphGradient = document.getElementById("performanceLine").getContext('2d');
      var graphGradient2 = document.getElementById("performanceLine").getContext('2d');
      var saleGradientBg = graphGradient.createLinearGradient(5, 0, 5, 100);
      saleGradientBg.addColorStop(0, 'rgba(26, 115, 232, 0.18)');
      saleGradientBg.addColorStop(1, 'rgba(26, 115, 232, 0.02)');
      var saleGradientBg2 = graphGradient2.createLinearGradient(100, 0, 50, 150);
      saleGradientBg2.addColorStop(0, 'rgba(0, 208, 255, 0.19)');
      saleGradientBg2.addColorStop(1, 'rgba(0, 208, 255, 0.03)');

      new Chart(ctx, {
        type: 'line',
        data: {
        labels: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"],
        datasets: [{
            label: 'Tiền vào',
            data: [50, 110, 60, 290, 200, 115, 130, 170, 90, 210, 240, 280, 200],
            backgroundColor: saleGradientBg,
            borderColor: [
                '#1F3BB3',
            ],
            borderWidth: 1.5,
            fill: true, // 3: no fill
            pointBorderWidth: 1,
            pointRadius: [4, 4, 4, 4, 4,4, 4, 4, 4, 4,4, 4, 4],
            pointHoverRadius: [2, 2, 2, 2, 2,2, 2, 2, 2, 2,2, 2, 2],
            pointBackgroundColor: ['#1F3BB3)', '#1F3BB3', '#1F3BB3', '#1F3BB3','#1F3BB3)', '#1F3BB3', '#1F3BB3', '#1F3BB3','#1F3BB3)', '#1F3BB3', '#1F3BB3', '#1F3BB3','#1F3BB3)'],
            pointBorderColor: ['#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff',],
        },{
          label: 'Tiền ra',
          data: [30, 150, 190, 250, 120, 150, 130, 20, 30, 15, 40, 95, 180],
          backgroundColor: saleGradientBg2,
          borderColor: [
              '#52CDFF',
          ],
          borderWidth: 1.5,
          fill: true, // 3: no fill
          pointBorderWidth: 1,
          pointRadius: [0, 0, 0, 4, 0],
          pointHoverRadius: [0, 0, 0, 2, 0],
          pointBackgroundColor: ['#52CDFF)', '#52CDFF', '#52CDFF', '#52CDFF','#52CDFF)', '#52CDFF', '#52CDFF', '#52CDFF','#52CDFF)', '#52CDFF', '#52CDFF', '#52CDFF','#52CDFF)'],
            pointBorderColor: ['#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff','#fff',],
      }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          elements: {
            line: {
                tension: 0.4,
            }
          },
        
          scales: {
            y: {
              border: {
                display: false
              },
              grid: {
                display: true,
                color:"#F0F0F0",
                drawBorder: false,
              },
              ticks: {
                beginAtZero: false,
                autoSkip: true,
                maxTicksLimit: 4,
                color:"#6B778C",
                font: {
                  size: 10,
                }
              }
            },
            x: {
              border: {
                display: false
              },
              grid: {
                display: false,
                drawBorder: false,
              },
              ticks: {
                beginAtZero: false,
                autoSkip: true,
                maxTicksLimit: 7,
                color:"#6B778C",
                font: {
                  size: 10,
                }
              }
            }
          },
        }
      });
    }

    if ($("#marketingOverview").length) { 
      const marketingOverviewCanvas = document.getElementById('marketingOverview');
      new Chart(marketingOverviewCanvas, {
        type: 'bar',
        data: {
            labels: ["Tuần 1", "Tuần 2", "Tuần 3", "Tuần 4"],
          datasets: [{
            label: 'Tiền vào',
            data: [110, 220, 200, 190],
            backgroundColor: "#52CDFF",
            borderColor: [
                '#52CDFF',
            ],
              borderWidth: 0,
              barPercentage: 0.35,
              fill: true, // 3: no fill
              
          },{
              label: 'Tiền ra',
            data: [215, 290, 210, 250],
            backgroundColor: "#1F3BB3",
            borderColor: [
                '#1F3BB3',
            ],
            borderWidth: 0,
              barPercentage: 0.35,
              fill: true, // 3: no fill
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          elements: {
            line: {
                tension: 0.4,
            }
        },
        
          scales: {
            y: {
              border: {
                display: false
              },
              grid: {
                display: true,
                drawTicks: false,
                color:"#F0F0F0",
                zeroLineColor: '#F0F0F0',
              },
              ticks: {
                beginAtZero: false,
                autoSkip: true,
                maxTicksLimit: 4,
                color:"#6B778C",
                font: {
                  size: 10,
                }
              }
            },
            x: {
              border: {
                display: false
              },
              stacked: true,
              grid: {
                display: false,
                drawTicks: false,
              },
              ticks: {
                beginAtZero: false,
                autoSkip: true,
                maxTicksLimit: 7,
                color:"#6B778C",
                font: {
                  size: 10,
                }
              }
            }
          }
        }
      });
    }


    let itemAdminSidebarIcon = localStorage.getItem('adminSidebarIcon');
    if (itemAdminSidebarIcon && itemAdminSidebarIcon === "true") {
        $('body').addClass('sidebar-icon-only');
        document.querySelector('.admin-body .main-panel >div').style.width = 'calc(100% - 100px)';
    }
    else {
        $('body').removeClass('sidebar-icon-only');
        document.querySelector('.admin-body .main-panel>div').style.width = '100%';
    }
    document.querySelector('nav .navbar-toggler').addEventListener('click', function () {
        if ($('body').hasClass('sidebar-icon-only')) {
            localStorage.setItem('adminSidebarIcon', true);
            document.querySelector('.admin-body .main-panel').style.width = 'calc(100% - 100px)';
        }
        else {
            localStorage.setItem('adminSidebarIcon', false);
            document.querySelector('.admin-body .main-panel').style.width = '100%';
        }
    });
    
  });

  
})(jQuery);