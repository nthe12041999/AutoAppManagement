/**
 * Create Admin Modal Component
 * Handles form validation, submission and modal interactions
 */

// Load required dependencies
function loadScript(src) {
    return new Promise((resolve, reject) => {
        // Check if script already exists
        if (document.querySelector(`script[src="${src}"]`)) {
            resolve();
            return;
        }

        const script = document.createElement('script');
        script.src = src;
        script.onload = resolve;
        script.onerror = reject;
        document.head.appendChild(script);
    });
}

// Load dependencies
async function loadDependencies() {
    try {
        console.log('📦 Loading CreateAdminModal dependencies...');

        // Load validation mixin first
        await loadScript('/js/mixins/validation-mixin.js');
        console.log('✅ ValidationMixin loaded');

        // Then load form mixin
        await loadScript('/js/mixins/form-mixin.js');
        console.log('✅ FormMixin loaded');

        console.log('✅ All dependencies loaded successfully');
        return true;
    } catch (error) {
        console.error('❌ Failed to load dependencies:', error);
        return false;
    }
}

class CreateAdminModal {
    constructor(modalSelector = '#createAdminModal', formSelector = '#adminCreateForm') {
        this.modalSelector = modalSelector;
        this.formSelector = formSelector;
        this.modal = null;
        this.form = null;
        this.formMixin = null;
        
        this.init();
    }
    
    async init() {
        console.log('🚀 Initializing Create Admin Modal...');

        // Load dependencies first
        const dependenciesLoaded = await loadDependencies();
        if (!dependenciesLoaded) {
            console.error('❌ Failed to load dependencies, modal may not work properly');
            return;
        }

        // Wait for DOM to be ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.setup());
        } else {
            this.setup();
        }
    }
    
    setup() {
        // Get modal and form elements
        this.modal = document.querySelector(this.modalSelector);
        this.form = document.querySelector(this.formSelector);
        
        if (!this.modal || !this.form) {
            console.error('❌ Modal or form not found:', {
                modal: this.modal,
                form: this.form
            });
            return;
        }
        
        // Initialize FormMixin
        this.initFormMixin();
        
        // Setup modal event handlers
        this.setupModalEvents();
        
        // Setup fallback button handlers
        this.setupFallbackHandlers();
        
        console.log('✅ Create Admin Modal initialized successfully');
    }
    
    initFormMixin() {
        this.formMixin = new FormMixin(this.formSelector, {
            autoValidate: true,
            showValidationSummary: true,
            trackChanges: true,
            confirmBeforeSubmit: true,
            apiUrl: '/AdminAccount/CreateAdmin',
            method: 'POST',
            onSubmitSuccess: (response) => this.handleSubmitSuccess(response),
            onSubmitError: (error) => this.handleSubmitError(error),
            onValidationChange: (isValid, errors) => this.handleValidationChange(isValid, errors),
            onFormChange: (hasChanges) => this.handleFormChange(hasChanges)
        });
    }
    
    setupModalEvents() {
        // Reset form when modal is hidden
        this.modal.addEventListener('hidden.bs.modal', () => {
            this.formMixin.reset();
        });
        
        // Focus first input when modal is shown
        this.modal.addEventListener('shown.bs.modal', () => {
            const firstInput = this.form.querySelector('input:not([type="hidden"])');
            if (firstInput) {
                firstInput.focus();
            }
        });
    }
    
    show() {
        if (this.modal) {
            const bootstrapModal = new bootstrap.Modal(this.modal);
            bootstrapModal.show();
        }
    }
    
    hide() {
        if (this.modal) {
            const bootstrapModal = bootstrap.Modal.getInstance(this.modal);
            if (bootstrapModal) {
                bootstrapModal.hide();
            }
        }
    }
    
    handleSubmitSuccess(response) {
        // Close modal
        this.hide();
        
        // Show success message
        Swal.fire({
            icon: 'success',
            title: 'Thành công!',
            text: 'Tạo admin thành công!',
            timer: 2000,
            showConfirmButton: false
        }).then(() => {
            // Refresh DataGrid
            if (window.adminGrid) {
                window.adminGrid.refresh();
            } else {
                location.reload();
            }
        });
    }
    
    handleSubmitError(error) {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: error.message || 'Có lỗi xảy ra khi tạo admin',
            confirmButtonText: 'OK'
        });
    }
    
    handleValidationChange(isValid, errors) {
        const summaryCard = document.getElementById('validationSummaryCard');
        const summaryList = document.getElementById('validationSummary');
        
        if (summaryCard && summaryList) {
            if (errors.length > 0) {
                summaryList.innerHTML = errors.map(error => 
                    `<li class="text-danger"><i class="bi bi-x-circle me-1"></i>${error}</li>`
                ).join('');
                summaryCard.style.display = 'block';
            } else {
                summaryCard.style.display = 'none';
            }
        }
    }
    
    handleFormChange(hasChanges) {
        const statusDiv = document.getElementById('formStatus');
        if (statusDiv) {
            if (hasChanges) {
                statusDiv.innerHTML = `
                    <div class="d-flex align-items-center text-warning">
                        <i class="bi bi-pencil me-2"></i>
                        <span>Có thay đổi chưa lưu</span>
                    </div>
                `;
            } else {
                statusDiv.innerHTML = `
                    <div class="d-flex align-items-center text-muted">
                        <i class="bi bi-circle me-2"></i>
                        <span>Chưa có thay đổi</span>
                    </div>
                `;
            }
        }
    }
} 
// Export for global access
window.CreateAdminModal = CreateAdminModal;
