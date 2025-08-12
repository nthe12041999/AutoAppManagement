/**
 * Sidebar Scroll Enhancement
 * Provides smooth scrolling, scroll indicators, and enhanced UX for sidebar navigation
 */

class SidebarScroll {
    constructor() {
        this.sidebar = document.querySelector('.admin-sidebar');
        this.sidebarNav = document.querySelector('.sidebar-nav');
        this.scrollIndicator = null;
        this.isScrolling = false;
        this.scrollTimeout = null;

        this.init();
    }

    init() {
        if(!this.sidebar || !this.sidebarNav) {
            console.warn('Sidebar elements not found');
            return;
        }

        this.createScrollIndicator();
        this.addScrollToTopButton();
        this.attachEventListeners();
        this.updateScrollIndicator();
        this.handleActiveItemScroll();

        console.log('SidebarScroll initialized');
    }

    /**
     * Create scroll indicator
     */
    createScrollIndicator() {
        this.scrollIndicator = document.createElement('div');
        this.scrollIndicator.className = 'sidebar-scroll-indicator';
        this.sidebar.appendChild(this.scrollIndicator);
    }

    /**
     * Attach event listeners
     */
    attachEventListeners() {
        // Scroll event
        this.sidebarNav.addEventListener('scroll', this.handleScroll.bind(this));

        // Mouse enter/leave for scroll indicator
        this.sidebarNav.addEventListener('mouseenter', this.showScrollIndicator.bind(this));
        this.sidebarNav.addEventListener('mouseleave', this.hideScrollIndicator.bind(this));

        // Smooth scroll to active item on page load
        window.addEventListener('load', this.scrollToActiveItem.bind(this));

        // Handle submenu toggle
        const submenuToggles = document.querySelectorAll('[data-bs-toggle="collapse"]');
        submenuToggles.forEach(toggle => {
            toggle.addEventListener('click', this.handleSubmenuToggle.bind(this));
        });

        // Keyboard navigation
        this.sidebarNav.addEventListener('keydown', this.handleKeyboardNavigation.bind(this));

        // Resize handler
        window.addEventListener('resize', this.handleResize.bind(this));
    }

    /**
     * Handle scroll event
     */
    handleScroll() {
        this.updateScrollIndicator();
        this.handleScrollingState();
    }

    /**
     * Update scroll indicator position
     */
    updateScrollIndicator() {
        if(!this.scrollIndicator) return;

        const scrollTop = this.sidebarNav.scrollTop;
        const scrollHeight = this.sidebarNav.scrollHeight;
        const clientHeight = this.sidebarNav.clientHeight;

        if(scrollHeight <= clientHeight) {
            this.scrollIndicator.style.display = 'none';
            return;
        }

        this.scrollIndicator.style.display = 'block';

        const scrollPercentage = scrollTop / (scrollHeight - clientHeight);
        const indicatorHeight = 60;
        const maxTop = clientHeight - indicatorHeight;
        const indicatorTop = scrollPercentage * maxTop;

        this.scrollIndicator.style.top = `${indicatorTop}px`;

        // Update indicator thumb
        const thumb = this.scrollIndicator.querySelector('::before') || this.scrollIndicator;
        const thumbHeight = Math.max(20, (clientHeight / scrollHeight) * indicatorHeight);
        this.scrollIndicator.style.setProperty('--thumb-height', `${thumbHeight}px`);
    }

    /**
     * Show scroll indicator
     */
    showScrollIndicator() {
        if(this.scrollIndicator) {
            this.scrollIndicator.style.opacity = '1';
        }
    }

    /**
     * Hide scroll indicator
     */
    hideScrollIndicator() {
        if(this.scrollIndicator && !this.isScrolling) {
            this.scrollIndicator.style.opacity = '0';
        }
    }

    /**
     * Handle scrolling state
     */
    handleScrollingState() {
        this.isScrolling = true;
        this.showScrollIndicator();

        clearTimeout(this.scrollTimeout);
        this.scrollTimeout = setTimeout(() => {
            this.isScrolling = false;
            if(!this.sidebarNav.matches(':hover')) {
                this.hideScrollIndicator();
            }
        }, 1000);
    }

    /**
     * Scroll to active item
     */
    scrollToActiveItem() {
        const activeItem = this.sidebarNav.querySelector('.nav-link.active');
        if(activeItem) {
            this.smoothScrollToElement(activeItem);
        }
    }

    /**
     * Handle active item scroll on page load
     */
    handleActiveItemScroll() {
        // Delay to ensure DOM is fully rendered
        setTimeout(() => {
            this.scrollToActiveItem();
        }, 100);
    }

    /**
     * Smooth scroll to element
     */
    smoothScrollToElement(element) {
        if(!element) return;

        const elementTop = element.offsetTop;
        const elementHeight = element.offsetHeight;
        const containerHeight = this.sidebarNav.clientHeight;
        const scrollTop = this.sidebarNav.scrollTop;

        // Calculate optimal scroll position (center the element)
        const targetScrollTop = elementTop - (containerHeight / 2) + (elementHeight / 2);

        // Only scroll if element is not fully visible
        const elementBottom = elementTop + elementHeight;
        const visibleTop = scrollTop;
        const visibleBottom = scrollTop + containerHeight;

        if(elementTop < visibleTop || elementBottom > visibleBottom) {
            this.sidebarNav.scrollTo({
                top: Math.max(0, targetScrollTop),
                behavior: 'smooth'
            });
        }
    }

    /**
     * Handle submenu toggle
     */
    handleSubmenuToggle(event) {
        const toggle = event.currentTarget;
        const targetId = toggle.getAttribute('data-bs-target');
        const submenu = document.querySelector(targetId);

        if(submenu) {
            // Delay scroll to allow collapse animation
            setTimeout(() => {
                if(submenu.classList.contains('show')) {
                    this.smoothScrollToElement(toggle);
                }
            }, 350);
        }
    }

    /**
     * Handle keyboard navigation
     */
    handleKeyboardNavigation(event) {
        const focusedElement = document.activeElement;
        if(!focusedElement.classList.contains('nav-link')) return;

        let nextElement = null;

        switch(event.key) {
            case 'ArrowDown':
                nextElement = this.getNextNavLink(focusedElement);
                break;
            case 'ArrowUp':
                nextElement = this.getPreviousNavLink(focusedElement);
                break;
            case 'Home':
                nextElement = this.sidebarNav.querySelector('.nav-link');
                break;
            case 'End':
                const navLinks = this.sidebarNav.querySelectorAll('.nav-link');
                nextElement = navLinks[navLinks.length - 1];
                break;
            default:
                return;
        }

        if(nextElement) {
            event.preventDefault();
            nextElement.focus();
            this.smoothScrollToElement(nextElement);
        }
    }

    /**
     * Get next navigation link
     */
    getNextNavLink(currentElement) {
        const navLinks = Array.from(this.sidebarNav.querySelectorAll('.nav-link'));
        const currentIndex = navLinks.indexOf(currentElement);
        return navLinks[currentIndex + 1] || navLinks[0];
    }

    /**
     * Get previous navigation link
     */
    getPreviousNavLink(currentElement) {
        const navLinks = Array.from(this.sidebarNav.querySelectorAll('.nav-link'));
        const currentIndex = navLinks.indexOf(currentElement);
        return navLinks[currentIndex - 1] || navLinks[navLinks.length - 1];
    }

    /**
     * Handle window resize
     */
    handleResize() {
        this.updateScrollIndicator();
    }

    /**
     * Scroll to top
     */
    scrollToTop() {
        this.sidebarNav.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    }

    /**
     * Scroll to bottom
     */
    scrollToBottom() {
        this.sidebarNav.scrollTo({
            top: this.sidebarNav.scrollHeight,
            behavior: 'smooth'
        });
    }

    /**
     * Add scroll to top button
     */
    addScrollToTopButton() {
        const button = document.createElement('button');
        button.className = 'sidebar-scroll-top';
        button.innerHTML = '<i class="bi bi-arrow-up"></i>';
        button.title = 'Scroll to top';
        button.addEventListener('click', this.scrollToTop.bind(this));

        this.sidebar.appendChild(button);

        // Show/hide based on scroll position
        this.sidebarNav.addEventListener('scroll', () => {
            if(this.sidebarNav.scrollTop > 200) {
                button.style.opacity = '1';
            } else {
                button.style.opacity = '0';
            }
        });
    }
}

// Auto-initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    new SidebarScroll();
});

// Export for manual initialization if needed
window.SidebarScroll = SidebarScroll;
