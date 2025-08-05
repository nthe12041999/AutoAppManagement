/**
 * Admin Modal Component
 * Handles Create/Edit/View modes for Admin management
 */

// Load required dependencies
function loadScript(src) {
    return new Promise((resolve, reject) => {
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

async function loadDependencies() {
    try {
        console.log('📦 Loading AdminModal dependencies...');
        await loadScript('/js/mixins/validation-mixin.js');
        await loadScript('/js/mixins/form-mixin.js');
        console.log('✅ All dependencies loaded successfully');
        return true;
    } catch (error) {
        console.error('❌ Failed to load dependencies:', error);
        return false;
    }
}

class AdminModal {
    constructor(modalSelector = '#adminModal', formSelector = '#adminForm') {
        this.modalSelector = modalSelector;
        this.formSelector = formSelector;
        this.modal = null;
        this.form = null;
        this.formMixin = null;
        this.currentMode = 'create';
        this.currentId = null;
        
        this.init();
    }
    
    async init() {
        console.log('🚀 Initializing Admin Modal...');
        
        const dependenciesLoaded = await loadDependencies();
        if (!dependenciesLoaded) {
            console.error('❌ Failed to load dependencies');
            return;
        }
        
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.setup());
        } else {
            this.setup();
        }
    }
    
    setup() {
        this.modal = document.querySelector(this.modalSelector);
        this.form = document.querySelector(this.formSelector);
        
        if (!this.modal || !this.form) {
            console.error('❌ Modal or form not found');
            return;
        }
        
        this.initFormMixin();
        this.setupModalEvents();
        this.setupFallbackHandlers();
        
        console.log('✅ Admin Modal initialized successfully');
    }
    
    initFormMixin() {
        this.formMixin = new FormMixin(this.formSelector, {
            autoValidate: true,
            showValidationSummary: true,
            trackChanges: true,
            confirmBeforeSubmit: true,
            apiUrl: '', // Will be set based on mode
            method: 'POST',
            onSubmitSuccess: (response) => this.handleSubmitSuccess(response),
            onSubmitError: (error) => this.handleSubmitError(error),
            onValidationChange: (isValid, errors) => this.handleValidationChange(isValid, errors),
            onFormChange: (hasChanges) => this.handleFormChange(hasChanges)
        });
    }
    
    setupModalEvents() {
        this.modal.addEventListener('hidden.bs.modal', () => {
            this.reset();
        });
        
        this.modal.addEventListener('shown.bs.modal', () => {
            const firstInput = this.form.querySelector('input:not([type="hidden"]):not([readonly])');
            if (firstInput) {
                firstInput.focus();
            }
        });
    }
    
    setupFallbackHandlers() {
        setTimeout(() => {
            this.setupActionHandlers();
        }, 2000);
    }
    
    setupActionHandlers() {
        const buttons = document.querySelectorAll('button');
        let handlerCount = 0;
        
        buttons.forEach(btn => {
            const text = btn.textContent.trim();
            const dataAction = btn.getAttribute('data-action');
            
            // Handle different actions
            if (dataAction === 'add' || text.includes('Thêm')) {
                if (!btn.hasAttribute('data-modal-handled')) {
                    btn.setAttribute('data-modal-handled', 'true');
                    btn.addEventListener('click', (e) => {
                        e.preventDefault();
                        this.openModal('create');
                    });
                    handlerCount++;
                }
            } else if (dataAction === 'edit' || text.includes('Chỉnh sửa')) {
                if (!btn.hasAttribute('data-modal-handled')) {
                    btn.setAttribute('data-modal-handled', 'true');
                    btn.addEventListener('click', (e) => {
                        e.preventDefault();
                        const id = btn.getAttribute('data-target') || btn.getAttribute('data-entity');
                        this.openModal('edit', id);
                    });
                    handlerCount++;
                }
            } else if (dataAction === 'view' || text.includes('Xem chi tiết')) {
                if (!btn.hasAttribute('data-modal-handled')) {
                    btn.setAttribute('data-modal-handled', 'true');
                    btn.addEventListener('click', (e) => {
                        e.preventDefault();
                        const id = btn.getAttribute('data-target') || btn.getAttribute('data-entity');
                        this.openModal('view', id);
                    });
                    handlerCount++;
                }
            }
        });
        
        console.log(`✅ Added modal handlers to ${handlerCount} buttons`);
    }
    
    openModal(mode, id = null) {
        console.log(`🔘 Opening modal in ${mode} mode`, id ? `for ID: ${id}` : '');
        
        this.currentMode = mode;
        this.currentId = id;
        
        this.configureModalForMode(mode);
        
        if (id && (mode === 'edit' || mode === 'view')) {
            this.loadAdminData(id);
        }
        
        this.show();
    }
    
    configureModalForMode(mode) {
        const modalTitle = document.getElementById('adminModalTitle');
        const modalIcon = document.getElementById('adminModalIcon');
        const submitButton = document.getElementById('submitButton');
        const submitButtonText = document.getElementById('submitButtonText');
        const modeInput = document.getElementById('adminMode');
        const passwordField = document.getElementById('passwordField');
        const confirmPasswordField = document.getElementById('confirmPasswordField');
        
        // Update form attributes
        const form = this.form;
        
        switch (mode) {
            case 'create':
                modalTitle.textContent = 'Thêm tài khoản quản trị';
                modalIcon.className = 'bi bi-person-plus me-2';
                submitButtonText.textContent = 'Tạo Admin';
                submitButton.style.display = 'block';
                
                form.setAttribute('data-api-url', '/AdminAccount/CreateAdmin');
                form.setAttribute('data-method', 'POST');
                form.setAttribute('data-confirm-message', 'Bạn có chắc chắn muốn tạo admin này?');
                form.setAttribute('data-success-message', 'Tạo admin thành công!');
                
                // Show password fields
                passwordField.style.display = 'block';
                confirmPasswordField.style.display = 'block';
                
                // Enable all fields
                this.setFieldsReadonly(false);
                break;
                
            case 'edit':
                modalTitle.textContent = 'Chỉnh sửa tài khoản quản trị';
                modalIcon.className = 'bi bi-pencil me-2';
                submitButtonText.textContent = 'Cập nhật';
                submitButton.style.display = 'block';
                
                form.setAttribute('data-api-url', '/AdminAccount/UpdateAdmin');
                form.setAttribute('data-method', 'PUT');
                form.setAttribute('data-confirm-message', 'Bạn có chắc chắn muốn cập nhật admin này?');
                form.setAttribute('data-success-message', 'Cập nhật admin thành công!');
                
                // Hide password fields
                passwordField.style.display = 'none';
                confirmPasswordField.style.display = 'none';
                
                // Enable all fields
                this.setFieldsReadonly(false);
                break;
                
            case 'view':
                modalTitle.textContent = 'Xem chi tiết tài khoản quản trị';
                modalIcon.className = 'bi bi-eye me-2';
                submitButton.style.display = 'none';
                
                // Hide password fields
                passwordField.style.display = 'none';
                confirmPasswordField.style.display = 'none';
                
                // Make all fields readonly
                this.setFieldsReadonly(true);
                break;
        }
        
        modeInput.value = mode;
        
        // Update FormMixin configuration
        if (this.formMixin) {
            this.formMixin.options.apiUrl = form.getAttribute('data-api-url');
            this.formMixin.options.method = form.getAttribute('data-method');
        }
    }
    
    setFieldsReadonly(readonly) {
        const inputs = this.form.querySelectorAll('input:not([type="hidden"]), textarea, select');
        inputs.forEach(input => {
            if (readonly) {
                input.setAttribute('readonly', 'readonly');
                if (input.type === 'checkbox' || input.type === 'radio') {
                    input.disabled = true;
                }
            } else {
                input.removeAttribute('readonly');
                input.disabled = false;
            }
        });
    }
    
    async loadAdminData(id) {
        try {
            console.log(`📋 Loading admin data for ID: ${id}`);
            
            const response = await fetch(`/AdminAccount/GetAdmin/${id}`);
            if (!response.ok) {
                throw new Error('Failed to load admin data');
            }
            
            const data = await response.json();
            this.populateForm(data);
            
            console.log('✅ Admin data loaded successfully');
        } catch (error) {
            console.error('❌ Error loading admin data:', error);
            this.handleSubmitError(error);
        }
    }
    
    populateForm(data) {
        // Set hidden ID
        document.getElementById('adminId').value = data.id || '';
        
        // Populate form fields
        const fields = {
            'fullName': data.fullName,
            'email': data.email,
            'phoneNumber': data.phoneNumber,
            'userName': data.userName,
            'notes': data.notes,
            'isActive': data.isActive
        };
        
        Object.keys(fields).forEach(fieldName => {
            const field = document.getElementById(fieldName);
            if (field) {
                if (field.type === 'checkbox') {
                    field.checked = fields[fieldName];
                } else {
                    field.value = fields[fieldName] || '';
                }
            }
        });
        
        // Set role
        if (data.role) {
            const roleRadio = document.getElementById(`role${data.role.charAt(0).toUpperCase() + data.role.slice(1)}`);
            if (roleRadio) {
                roleRadio.checked = true;
            }
        }
        
        // Set permissions
        if (data.permissions && Array.isArray(data.permissions)) {
            data.permissions.forEach(permission => {
                const permissionCheckbox = document.querySelector(`input[name="Permissions"][value="${permission}"]`);
                if (permissionCheckbox) {
                    permissionCheckbox.checked = true;
                }
            });
        }
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
    
    reset() {
        this.currentMode = 'create';
        this.currentId = null;
        
        if (this.formMixin) {
            this.formMixin.reset();
        }
        
        // Clear form
        this.form.reset();
        document.getElementById('adminId').value = '';
        document.getElementById('adminMode').value = 'create';
        
        // Reset validation summary
        const summaryCard = document.getElementById('validationSummaryCard');
        if (summaryCard) {
            summaryCard.style.display = 'none';
        }
    }
    
    handleSubmitSuccess(response) {
        this.hide();
        
        const mode = this.currentMode;
        const messages = {
            create: 'Tạo admin thành công!',
            edit: 'Cập nhật admin thành công!',
            view: 'Xem thông tin thành công!'
        };
        
        Swal.fire({
            icon: 'success',
            title: 'Thành công!',
            text: messages[mode],
            timer: 2000,
            showConfirmButton: false
        }).then(() => {
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
            text: error.message || 'Có lỗi xảy ra',
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

// Toggle password visibility utility function
function togglePassword(fieldId) {
    const field = document.getElementById(fieldId);
    const icon = document.getElementById(fieldId + '-icon');
    
    if (field && icon) {
        if (field.type === 'password') {
            field.type = 'text';
            icon.className = 'bi bi-eye-slash';
        } else {
            field.type = 'password';
            icon.className = 'bi bi-eye';
        }
    }
}

// Auto-initialize when script loads
document.addEventListener('DOMContentLoaded', function() {
    setTimeout(() => {
        window.adminModal = new AdminModal();
    }, 500);
});

// Export for global access
window.AdminModal = AdminModal;
window.togglePassword = togglePassword;
