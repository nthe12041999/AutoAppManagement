/**
 * License Modal Component
 * Handles Create/Edit/View modes for License management
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
        console.log('📦 Loading LicenseModal dependencies...');
        await loadScript('/js/mixins/validation-mixin.js');
        await loadScript('/js/mixins/form-mixin.js');
        console.log('✅ All dependencies loaded successfully');
        return true;
    } catch (error) {
        console.error('❌ Failed to load dependencies:', error);
        return false;
    }
}

class LicenseModal {
    constructor(modalSelector = '#licenseModal', formSelector = '#licenseForm') {
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
        console.log('🚀 Initializing License Modal...');
        
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
        
        console.log('✅ License Modal initialized successfully');
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
            if (dataAction === 'add' || text.includes('Thêm License') || text.includes('Thêm')) {
                if (!btn.hasAttribute('data-license-modal-handled')) {
                    btn.setAttribute('data-license-modal-handled', 'true');
                    btn.addEventListener('click', (e) => {
                        e.preventDefault();
                        this.openModal('create');
                    });
                    handlerCount++;
                }
            } else if (dataAction === 'edit' || text.includes('Chỉnh sửa')) {
                if (!btn.hasAttribute('data-license-modal-handled')) {
                    btn.setAttribute('data-license-modal-handled', 'true');
                    btn.addEventListener('click', (e) => {
                        e.preventDefault();
                        const id = btn.getAttribute('data-target') || btn.getAttribute('data-entity');
                        this.openModal('edit', id);
                    });
                    handlerCount++;
                }
            } else if (dataAction === 'view' || text.includes('Xem chi tiết')) {
                if (!btn.hasAttribute('data-license-modal-handled')) {
                    btn.setAttribute('data-license-modal-handled', 'true');
                    btn.addEventListener('click', (e) => {
                        e.preventDefault();
                        const id = btn.getAttribute('data-target') || btn.getAttribute('data-entity');
                        this.openModal('view', id);
                    });
                    handlerCount++;
                }
            }
        });
        
        console.log(`✅ Added license modal handlers to ${handlerCount} buttons`);
    }
    
    openModal(mode, id = null) {
        console.log(`🔘 Opening license modal in ${mode} mode`, id ? `for ID: ${id}` : '');
        
        this.currentMode = mode;
        this.currentId = id;
        
        this.configureModalForMode(mode);
        
        if (id && (mode === 'edit' || mode === 'view')) {
            this.loadLicenseData(id);
        }
        
        this.show();
    }
    
    configureModalForMode(mode) {
        const modalTitle = document.getElementById('licenseModalTitle');
        const modalIcon = document.getElementById('licenseModalIcon');
        const submitButton = document.getElementById('submitButton');
        const submitButtonText = document.getElementById('submitButtonText');
        const modeInput = document.getElementById('licenseMode');
        
        // Update form attributes
        const form = this.form;
        
        switch (mode) {
            case 'create':
                modalTitle.textContent = 'Thêm License mới';
                modalIcon.className = 'bi bi-key me-2';
                submitButtonText.textContent = 'Tạo License';
                submitButton.style.display = 'block';
                
                form.setAttribute('data-api-url', '/License/CreateLicense');
                form.setAttribute('data-method', 'POST');
                form.setAttribute('data-confirm-message', 'Bạn có chắc chắn muốn tạo license này?');
                form.setAttribute('data-success-message', 'Tạo license thành công!');
                
                // Enable all fields
                this.setFieldsReadonly(false);
                break;
                
            case 'edit':
                modalTitle.textContent = 'Chỉnh sửa License';
                modalIcon.className = 'bi bi-pencil me-2';
                submitButtonText.textContent = 'Cập nhật';
                submitButton.style.display = 'block';
                
                form.setAttribute('data-api-url', '/License/UpdateLicense');
                form.setAttribute('data-method', 'PUT');
                form.setAttribute('data-confirm-message', 'Bạn có chắc chắn muốn cập nhật license này?');
                form.setAttribute('data-success-message', 'Cập nhật license thành công!');
                
                // Enable all fields
                this.setFieldsReadonly(false);
                break;
                
            case 'view':
                modalTitle.textContent = 'Xem chi tiết License';
                modalIcon.className = 'bi bi-eye me-2';
                submitButton.style.display = 'none';
                
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
    
    async loadLicenseData(id) {
        try {
            console.log(`📋 Loading license data for ID: ${id}`);
            
            const response = await fetch(`/License/GetLicense/${id}`);
            if (!response.ok) {
                throw new Error('Failed to load license data');
            }
            
            const data = await response.json();
            this.populateForm(data);
            
            console.log('✅ License data loaded successfully');
        } catch (error) {
            console.error('❌ Error loading license data:', error);
            this.handleSubmitError(error);
        }
    }
    
    populateForm(data) {
        // Set hidden ID
        document.getElementById('licenseId').value = data.id || '';
        
        // Populate form fields
        const fields = {
            'licenseName': data.name,
            'licenseKey': data.licenseKey,
            'licenseType': data.type,
            'maxUsers': data.maxUsers,
            'startDate': data.startDate ? data.startDate.split('T')[0] : '',
            'expiryDate': data.expiryDate ? data.expiryDate.split('T')[0] : '',
            'customerName': data.customerName,
            'customerEmail': data.customerEmail,
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
        
        // Set features
        if (data.features && Array.isArray(data.features)) {
            data.features.forEach(feature => {
                const featureCheckbox = document.querySelector(`input[name="Features"][value="${feature}"]`);
                if (featureCheckbox) {
                    featureCheckbox.checked = true;
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
        document.getElementById('licenseId').value = '';
        document.getElementById('licenseMode').value = 'create';
        
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
            create: 'Tạo license thành công!',
            edit: 'Cập nhật license thành công!',
            view: 'Xem thông tin thành công!'
        };
        
        Swal.fire({
            icon: 'success',
            title: 'Thành công!',
            text: messages[mode],
            timer: 2000,
            showConfirmButton: false
        }).then(() => {
            if (window.licenseGrid) {
                window.licenseGrid.refresh();
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

// Auto-initialize when script loads
document.addEventListener('DOMContentLoaded', function() {
    setTimeout(() => {
        window.licenseModal = new LicenseModal();
    }, 500);
});

// Export for global access
window.LicenseModal = LicenseModal;
