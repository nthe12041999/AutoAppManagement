// Application initialization for ES6/jQuery based components
(function(){
    // Initialize core components when DOM is ready
    $(document).ready(function() {
        // Initialize DataGrid if available
        if (window.DataGrid && !window.dataGridInstance) {
            window.dataGridInstance = new DataGrid();
        }
        
        // Initialize other core components
        console.log('✅ Application components initialized');
    });
})();





