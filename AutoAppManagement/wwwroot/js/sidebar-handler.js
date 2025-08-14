// Sidebar Handler for Dynamic Content Adjustment with Pin/Unpin functionality
(function() {
    'use strict';
    
    document.addEventListener('DOMContentLoaded', function() {
        const sidebar = document.querySelector('.app-sidebar');
        const sidebarContent = document.querySelector('.sidebar-content');
        const content = document.querySelector('.app-content');
        const pinToggle = document.getElementById('sidebarPin');
        
        if (!sidebar || !content || !sidebarContent || !pinToggle) return;
        
        // State management
        let isPinned = localStorage.getItem('sidebarPinned') === 'true';
        let expandMain = localStorage.getItem('sidebarExpandMain') !== 'false'; // mặc định true
        
        // Initialize pin state
        function initializePinState() {
            if (isPinned) {
                sidebarContent.classList.add('menu-expanded');
                sidebarContent.classList.add('pinned'); // Thêm class pinned
                sidebar.classList.add('pinned'); // Thêm class pinned cho app-sidebar
                pinToggle.classList.add('pinned');
                // Force width cho app-sidebar
                sidebar.style.width = '300px';
                updateTooltip();
            } else {
                sidebarContent.classList.remove('menu-expanded');
                sidebarContent.classList.remove('pinned'); // Remove class pinned
                sidebar.classList.remove('pinned'); // Remove class pinned từ app-sidebar
                pinToggle.classList.remove('pinned');
                // Reset width cho app-sidebar
                sidebar.style.width = '80px';
                pinToggle.setAttribute('data-tooltip', 'Ghim menu');
            }
            
            if (expandMain) {
                sidebarContent.classList.add('expand-main');
            } else {
                sidebarContent.classList.remove('expand-main');
            }
        }
        
        // Update tooltip based on current state
        function updateTooltip() {
            if (isPinned) {
                if (expandMain) {
                    pinToggle.setAttribute('data-tooltip', 'Bỏ ghim (Nhấp đôi: Thu gọn content)');
                } else {
                    pinToggle.setAttribute('data-tooltip', 'Bỏ ghim (Nhấp đôi: Mở rộng content)');
                }
            } else {
                pinToggle.setAttribute('data-tooltip', 'Ghim menu');
            }
        }
        
        // Function to update content margin based on sidebar state
        function updateContentMargin() {
            if (window.innerWidth <= 768) {
                content.style.marginLeft = '0';
                content.style.width = '100vw';
                return;
            }
            
            let contentMargin;
            
            if (isPinned) {
                // Khi pinned, sidebar luôn rộng 300px
                // Chỉ quyết định có đẩy content ra không
                contentMargin = expandMain ? 300 : 80; // 80px để content không bị che
            } else {
                // Không pinned, luôn giữ margin 80px (không đẩy content khi hover)
                contentMargin = 80;
            }
            
            content.style.marginLeft = contentMargin + 'px';
            content.style.width = `calc(100vw - ${contentMargin}px)`;
        }
        
        // Pin toggle functionality
        pinToggle.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            isPinned = !isPinned;
            localStorage.setItem('sidebarPinned', isPinned);
            
            if (isPinned) {
                sidebarContent.classList.add('menu-expanded');
                sidebarContent.classList.add('pinned'); // Thêm class pinned
                pinToggle.classList.add('pinned');
                // Force width cho app-sidebar
                sidebar.style.width = '300px';
                console.log('Added menu-expanded class when pinned');
            } else {
                sidebarContent.classList.remove('menu-expanded');
                sidebarContent.classList.remove('pinned'); // Remove class pinned
                pinToggle.classList.remove('pinned');
                // Reset width cho app-sidebar
                sidebar.style.width = '80px';
                console.log('Removed menu-expanded class when unpinned');
            }
            
            updateTooltip();
            updateContentMargin();
        });
        
        // Double click to toggle expand main
        pinToggle.addEventListener('dblclick', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            expandMain = !expandMain;
            localStorage.setItem('sidebarExpandMain', expandMain);
            
            if (expandMain) {
                sidebarContent.classList.add('expand-main');
            } else {
                sidebarContent.classList.remove('expand-main');
            }
            
            updateTooltip();
            updateContentMargin();
        });
        
        // Hover events để add/remove class menu-expanded
        sidebarContent.addEventListener('mouseenter', function() {
            if (!isPinned) {
                sidebarContent.classList.add('menu-expanded');
                console.log('Added menu-expanded class on hover');
                // Không gọi updateContentMargin() để không đẩy content
            }
        });
        
        sidebarContent.addEventListener('mouseleave', function() {
            if (!isPinned) {
                sidebarContent.classList.remove('menu-expanded');
                console.log('Removed menu-expanded class on hover leave');
                // Không gọi updateContentMargin() để không đẩy content
            }
        });
        
        // Use ResizeObserver if available
        if (window.ResizeObserver) {
            const resizeObserver = new ResizeObserver(function(entries) {
                if (!isPinned) {
                    updateContentMargin();
                }
            });
            resizeObserver.observe(sidebarContent);
        }
        
        // Initial setup
        initializePinState();
        updateContentMargin();
        
        // Responsive handling
        function handleResize() {
            updateContentMargin();
        }
        
        window.addEventListener('resize', handleResize);
    });
})();
