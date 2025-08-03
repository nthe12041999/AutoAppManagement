// Sidebar Submenu JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Initialize submenu functionality
    initializeSubmenu();
    
    // Set active states
    setActiveStates();
});

function initializeSubmenu() {
    // Handle submenu toggle clicks
    const submenuToggles = document.querySelectorAll('[data-bs-toggle="collapse"]');
    
    submenuToggles.forEach(toggle => {
        toggle.addEventListener('click', function(e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('data-bs-target');
            const target = document.querySelector(targetId);
            const isExpanded = this.getAttribute('aria-expanded') === 'true';
            
            if (target) {
                if (isExpanded) {
                    // Collapse
                    target.classList.remove('show');
                    this.setAttribute('aria-expanded', 'false');
                } else {
                    // Expand
                    target.classList.add('show');
                    this.setAttribute('aria-expanded', 'true');
                }
            }
        });
    });
    
    // Handle submenu item clicks
    const submenuLinks = document.querySelectorAll('.nav-submenu .nav-link');
    submenuLinks.forEach(link => {
        link.addEventListener('click', function() {
            // Remove active class from all submenu links
            submenuLinks.forEach(l => l.classList.remove('active'));
            
            // Add active class to clicked link
            this.classList.add('active');
        });
    });
}

function setActiveStates() {
    const currentPath = window.location.pathname;
    const currentController = getControllerFromPath(currentPath);
    const currentAction = getActionFromPath(currentPath);
    
    // Set active state for main menu items
    const mainMenuLinks = document.querySelectorAll('.admin-sidebar .nav-link:not(.nav-submenu .nav-link)');
    mainMenuLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href && href === currentPath) {
            link.classList.add('active');
        }
    });
    
    // Set active state for submenu items and expand parent
    const submenuLinks = document.querySelectorAll('.nav-submenu .nav-link');
    submenuLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href && href === currentPath) {
            link.classList.add('active');
            
            // Expand parent submenu
            const submenu = link.closest('.collapse');
            if (submenu) {
                submenu.classList.add('show');
                
                // Set parent toggle as expanded
                const toggle = document.querySelector(`[data-bs-target="#${submenu.id}"]`);
                if (toggle) {
                    toggle.setAttribute('aria-expanded', 'true');
                    toggle.classList.add('active');
                }
            }
        }
    });
    
    // Special handling for Elements controller
    if (currentController === 'Elements') {
        const elementsToggle = document.querySelector('[data-bs-target="#elementsSubmenu"]');
        const elementsSubmenu = document.querySelector('#elementsSubmenu');
        
        if (elementsToggle && elementsSubmenu) {
            elementsSubmenu.classList.add('show');
            elementsToggle.setAttribute('aria-expanded', 'true');
            elementsToggle.classList.add('active');
        }
    }
}

function getControllerFromPath(path) {
    const parts = path.split('/').filter(p => p);
    return parts.length > 0 ? parts[0] : '';
}

function getActionFromPath(path) {
    const parts = path.split('/').filter(p => p);
    return parts.length > 1 ? parts[1] : 'Index';
}

// Handle page navigation
window.addEventListener('popstate', function() {
    setActiveStates();
});

// Handle AJAX navigation if using SPA features
document.addEventListener('click', function(e) {
    const link = e.target.closest('a[href]');
    if (link && link.href && !link.href.startsWith('javascript:') && !link.href.includes('#')) {
        // Update active states after navigation
        setTimeout(() => {
            setActiveStates();
        }, 100);
    }
});
