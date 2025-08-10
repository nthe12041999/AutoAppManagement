// Demo Modal Component JavaScript
console.log('🚀 Loading Demo Modal Component...');

const DemoModal = {
    // Configuration
    config: {
        modalId: 'demoModal',
        formId: 'demoForm',
        apiUrl: '/Demo'
    },

    // Current mode: 'create', 'edit', 'view'
    currentMode: 'create',
    currentData: null,

    // Initialize
    init() {
        console.log('🎯 Initializing Demo Modal...');
        
        this.setupEventListeners();
        this.setupValidation();
        
        console.log('✅ Demo Modal initialized');
    },

    // Setup event listeners
    setupEventListeners() {
        // Save button
        const saveBtn = document.getElementById('saveDemoItem');
        if (saveBtn) {
            saveBtn.addEventListener('click', () => this.save());
        }

        // Modal events
        const modal = document.getElementById(this.config.modalId);
        if (modal) {
            modal.addEventListener('hidden.bs.modal', () => this.onModalHidden());
            modal.addEventListener('shown.bs.modal', () => this.onModalShown());
        }

        // Form validation on input
        const form = document.getElementById(this.config.formId);
        if (form) {
            const inputs = form.querySelectorAll('input, select');
            inputs.forEach(input => {
                input.addEventListener('blur', () => this.validateField(input));
                input.addEventListener('input', () => this.clearFieldError(input));
            });
        }
    },

    // Setup form validation
    setupValidation() {
        const form = document.getElementById(this.config.formId);
        if (form) {
            form.addEventListener('submit', (e) => {
                e.preventDefault();
                this.save();
            });
        }
    },

    // Show modal
    show(mode = 'create', data = null) {
        console.log('📝 Showing demo modal in mode:', mode);
        
        this.currentMode = mode;
        this.currentData = data;

        // Update modal title and button text
        this.updateModalUI();

        // Populate form if editing
        if (mode === 'edit' && data) {
            this.populateForm(data);
        } else {
            this.clearForm();
        }

        // Show modal
        const modal = new bootstrap.Modal(document.getElementById(this.config.modalId));
        modal.show();
    },

    // Update modal UI based on mode
    updateModalUI() {
        const modalTitle = document.getElementById('demoModalTitle');
        const saveText = document.getElementById('saveDemoText');
        const saveBtn = document.getElementById('saveDemoItem');

        switch (this.currentMode) {
            case 'create':
                if (modalTitle) modalTitle.textContent = 'Thêm demo item';
                if (saveText) saveText.textContent = 'Tạo mới';
                if (saveBtn) {
                    saveBtn.className = 'btn btn-primary';
                    saveBtn.disabled = false;
                }
                break;
            case 'edit':
                if (modalTitle) modalTitle.textContent = 'Sửa demo item';
                if (saveText) saveText.textContent = 'Cập nhật';
                if (saveBtn) {
                    saveBtn.className = 'btn btn-warning';
                    saveBtn.disabled = false;
                }
                break;
            case 'view':
                if (modalTitle) modalTitle.textContent = 'Xem demo item';
                if (saveText) saveText.textContent = 'Đóng';
                if (saveBtn) {
                    saveBtn.className = 'btn btn-secondary';
                    saveBtn.disabled = true;
                }
                break;
        }
    },

    // Populate form with data
    populateForm(data) {
        console.log('📋 Populating form with data:', data);

        // Map data to form fields
        const fieldMappings = {
            'demoId': data.id,
            'demoName': data.name,
            'demoEmail': data.email,
            'demoPhone': data.phone,
            'demoDepartment': data.department,
            'demoPosition': data.position,
            'demoStatus': data.status,
            'demoSalary': data.salary,
            'demoScore': data.score,
            'demoJoinDate': data.joinDate ? new Date(data.joinDate).toISOString().split('T')[0] : '',
            'demoIsActive': data.isActive
        };

        // Populate fields
        Object.entries(fieldMappings).forEach(([fieldId, value]) => {
            const field = document.getElementById(fieldId);
            if (field) {
                if (field.type === 'checkbox') {
                    field.checked = Boolean(value);
                } else {
                    field.value = value || '';
                }
            }
        });

        // Disable fields if in view mode
        if (this.currentMode === 'view') {
            this.setFormReadonly(true);
        } else {
            this.setFormReadonly(false);
        }
    },

    // Clear form
    clearForm() {
        console.log('🧹 Clearing form...');

        const form = document.getElementById(this.config.formId);
        if (form) {
            form.reset();
            form.classList.remove('was-validated');
            
            // Clear validation states
            const fields = form.querySelectorAll('.form-control, .form-select');
            fields.forEach(field => {
                field.classList.remove('is-valid', 'is-invalid');
            });
        }

        // Set default values
        const isActiveField = document.getElementById('demoIsActive');
        if (isActiveField) {
            isActiveField.checked = true;
        }

        this.setFormReadonly(false);
    },

    // Set form readonly
    setFormReadonly(readonly) {
        const form = document.getElementById(this.config.formId);
        if (form) {
            const fields = form.querySelectorAll('input, select, textarea');
            fields.forEach(field => {
                if (readonly) {
                    field.setAttribute('readonly', 'readonly');
                    field.setAttribute('disabled', 'disabled');
                } else {
                    field.removeAttribute('readonly');
                    field.removeAttribute('disabled');
                }
            });
        }
    },

    // Validate form
    validateForm() {
        const form = document.getElementById(this.config.formId);
        if (!form) return false;

        // Add validation class
        form.classList.add('was-validated');

        // Check HTML5 validation
        const isValid = form.checkValidity();

        // Custom validations
        const email = document.getElementById('demoEmail');
        if (email && email.value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email.value)) {
                this.setFieldError(email, 'Email không hợp lệ');
                return false;
            }
        }

        const score = document.getElementById('demoScore');
        if (score && score.value) {
            const scoreValue = parseInt(score.value);
            if (scoreValue < 0 || scoreValue > 100) {
                this.setFieldError(score, 'Điểm phải từ 0 đến 100');
                return false;
            }
        }

        return isValid;
    },

    // Validate single field
    validateField(field) {
        if (!field.checkValidity()) {
            this.setFieldError(field, field.validationMessage);
        } else {
            this.clearFieldError(field);
        }
    },

    // Set field error
    setFieldError(field, message) {
        field.classList.add('is-invalid');
        field.classList.remove('is-valid');
        
        const feedback = field.parentNode.querySelector('.invalid-feedback');
        if (feedback) {
            feedback.textContent = message;
        }
    },

    // Clear field error
    clearFieldError(field) {
        field.classList.remove('is-invalid');
        if (field.checkValidity()) {
            field.classList.add('is-valid');
        }
    },

    // Save form
    save() {
        console.log('💾 Saving demo item...');

        // Skip validation in view mode
        if (this.currentMode === 'view') {
            this.hide();
            return;
        }

        // Validate form
        if (!this.validateForm()) {
            console.log('❌ Form validation failed');
            this.showNotification('Vui lòng kiểm tra lại thông tin', 'error');
            return;
        }

        // Get form data
        const formData = this.getFormData();
        console.log('📤 Form data:', formData);

        // Show loading state
        this.setLoadingState(true);

        // Simulate API call
        setTimeout(() => {
            this.setLoadingState(false);
            
            if (this.currentMode === 'create') {
                this.showNotification('Tạo mới thành công!', 'success');
            } else {
                this.showNotification('Cập nhật thành công!', 'success');
            }
            
            this.hide();
            
            // Refresh grid if available
            if (window.DemoDataGrid) {
                window.DemoDataGrid.applyFilters();
            }
        }, 1000);
    },

    // Get form data
    getFormData() {
        const form = document.getElementById(this.config.formId);
        const formData = new FormData(form);
        const data = {};

        for (let [key, value] of formData.entries()) {
            data[key] = value;
        }

        // Handle checkbox
        data.isActive = document.getElementById('demoIsActive')?.checked || false;

        return data;
    },

    // Set loading state
    setLoadingState(loading) {
        const saveBtn = document.getElementById('saveDemoItem');
        const saveText = document.getElementById('saveDemoText');

        if (loading) {
            if (saveBtn) saveBtn.disabled = true;
            if (saveText) saveText.innerHTML = '<i class="bi bi-hourglass-split me-1"></i>Đang lưu...';
        } else {
            if (saveBtn) saveBtn.disabled = false;
            if (saveText) {
                const text = this.currentMode === 'create' ? 'Tạo mới' : 'Cập nhật';
                saveText.innerHTML = `<i class="bi bi-check-lg me-1"></i>${text}`;
            }
        }
    },

    // Hide modal
    hide() {
        const modal = bootstrap.Modal.getInstance(document.getElementById(this.config.modalId));
        if (modal) {
            modal.hide();
        }
    },

    // Modal hidden event
    onModalHidden() {
        console.log('🚪 Modal hidden');
        this.clearForm();
        this.currentMode = 'create';
        this.currentData = null;
    },

    // Modal shown event
    onModalShown() {
        console.log('👁️ Modal shown');
        
        // Focus first input
        const firstInput = document.querySelector(`#${this.config.modalId} input:not([type="hidden"]):not([readonly])`);
        if (firstInput) {
            firstInput.focus();
        }
    },

    // Show notification
    showNotification(message, type = 'info') {
        if (window.DemoDataGrid && window.DemoDataGrid.showNotification) {
            window.DemoDataGrid.showNotification(message, type);
        } else {
            alert(message);
        }
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    DemoModal.init();
});

// Export for global access
window.DemoModal = DemoModal;
