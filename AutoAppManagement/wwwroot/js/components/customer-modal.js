/**
 * Customer Modal Component
 * Handles customer account creation, editing, and viewing
 */

class CustomerModal {
    constructor() {
        this.modal = null;
        this.form = null;
        this.isEditMode = false;
        this.currentCustomerId = null;
        
        this.init();
    }

    init() {
        console.log('🔧 Initializing Customer Modal...');
        
        this.modal = document.getElementById('customerModal');
        this.form = document.getElementById('customerForm');
        
        if (!this.modal || !this.form) {
            console.error('❌ Customer modal or form not found');
            return;
        }

        this.bindEvents();
        console.log('✅ Customer Modal initialized');
    }

    bindEvents() {
        // Save button click
        const saveButton = document.getElementById('saveCustomer');
        if (saveButton) {
            saveButton.addEventListener('click', () => this.handleSave());
        }

        // Modal events
        this.modal.addEventListener('show.bs.modal', () => this.onModalShow());
        this.modal.addEventListener('hidden.bs.modal', () => this.onModalHidden());

        // Form validation
        this.form.addEventListener('submit', (e) => {
            e.preventDefault();
            this.handleSave();
        });

        // Password confirmation validation
        const confirmPassword = document.getElementById('customerConfirmPassword');
        const password = document.getElementById('customerPassword');
        
        if (confirmPassword && password) {
            confirmPassword.addEventListener('input', () => {
                this.validatePasswordMatch();
            });
            
            password.addEventListener('input', () => {
                this.validatePasswordMatch();
            });
        }

        // Level change handler
        const levelSelect = document.getElementById('customerLevel');
        if (levelSelect) {
            levelSelect.addEventListener('change', () => this.handleLevelChange());
        }
    }

    // Show modal for creating new customer
    showCreateModal() {
        console.log('📝 Opening create customer modal...');
        
        this.isEditMode = false;
        this.currentCustomerId = null;
        
        // Update modal title
        const modalTitle = document.getElementById('customerModalLabel');
        if (modalTitle) {
            modalTitle.innerHTML = '<i class="bi bi-person-plus me-2"></i>Thêm khách hàng mới';
        }

        // Show password fields
        this.togglePasswordFields(true);
        
        // Set default values
        this.setDefaultValues();
        
        // Show modal
        const bsModal = new bootstrap.Modal(this.modal);
        bsModal.show();
    }

    // Show modal for editing existing customer
    showEditModal(customerId) {
        console.log('✏️ Opening edit customer modal for ID:', customerId);
        
        this.isEditMode = true;
        this.currentCustomerId = customerId;
        
        // Update modal title
        const modalTitle = document.getElementById('customerModalLabel');
        if (modalTitle) {
            modalTitle.innerHTML = '<i class="bi bi-pencil me-2"></i>Chỉnh sửa khách hàng';
        }

        // Hide password fields in edit mode
        this.togglePasswordFields(false);
        
        // Load customer data
        this.loadCustomerData(customerId);
        
        // Show modal
        const bsModal = new bootstrap.Modal(this.modal);
        bsModal.show();
    }

    // Show modal for viewing customer details
    showViewModal(customerId) {
        console.log('👁️ Opening view customer modal for ID:', customerId);
        
        this.isEditMode = false;
        this.currentCustomerId = customerId;
        
        // Update modal title
        const modalTitle = document.getElementById('customerModalLabel');
        if (modalTitle) {
            modalTitle.innerHTML = '<i class="bi bi-eye me-2"></i>Chi tiết khách hàng';
        }

        // Hide password fields
        this.togglePasswordFields(false);
        
        // Load customer data
        this.loadCustomerData(customerId);
        
        // Disable all form fields
        this.setFormReadonly(true);
        
        // Hide save button
        const saveButton = document.getElementById('saveCustomer');
        if (saveButton) {
            saveButton.style.display = 'none';
        }
        
        // Show modal
        const bsModal = new bootstrap.Modal(this.modal);
        bsModal.show();
    }

    // Load customer data from server
    async loadCustomerData(customerId) {
        try {
            console.log('📡 Loading customer data for ID:', customerId);

            // Show loading state
            this.setLoadingState(true);

            const response = await fetch(`/CustomerAccount/GetCustomerAccount?id=${customerId}`);
            const result = await response.json();

            if (result.success && result.data) {
                this.populateForm(result.data);
            } else {
                throw new Error(result.message || 'Failed to load customer data');
            }
        } catch (error) {
            console.error('❌ Error loading customer data:', error);
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: 'Không thể tải thông tin khách hàng'
                });
            } else {
                alert('Không thể tải thông tin khách hàng');
            }
        } finally {
            this.setLoadingState(false);
        }
    }

    // Populate form with customer data
    populateForm(customerData) {
        console.log('📋 Populating form with customer data:', customerData);
        
        const fields = [
            'Name', 'UserName', 'Email', 'Phone', 'Gender', 
            'DateOfBirth', 'Level', 'MaxAccountFb', 'Language',
            'RegisterDate', 'ExpiredDate', 'IsLocked'
        ];

        fields.forEach(field => {
            const element = document.getElementById(`customer${field}`);
            if (element && customerData[field] !== undefined) {
                if (element.type === 'checkbox') {
                    element.checked = customerData[field];
                } else if (element.type === 'date' && customerData[field]) {
                    // Format date for input
                    const date = new Date(customerData[field]);
                    element.value = date.toISOString().split('T')[0];
                } else {
                    element.value = customerData[field] || '';
                }
            }
        });
    }

    // Handle save action
    async handleSave() {
        console.log('💾 Handling save action...');

        if (!this.validateForm()) {
            return;
        }

        try {
            const formData = this.getFormData();
            const url = this.isEditMode
                ? `/CustomerAccount/UpdateCustomerAccount`
                : `/CustomerAccount/CreateCustomerAccount`;

            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData)
            });

            const result = await response.json();

            if (result.success) {
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        icon: 'success',
                        title: 'Thành công!',
                        text: result.message || (this.isEditMode ? 'Đã cập nhật khách hàng' : 'Đã tạo khách hàng mới'),
                        timer: 2000,
                        showConfirmButton: false
                    });
                } else {
                    alert(result.message || (this.isEditMode ? 'Đã cập nhật khách hàng' : 'Đã tạo khách hàng mới'));
                }

                // Close modal
                bootstrap.Modal.getInstance(this.modal).hide();

                // Refresh grid
                if (window.customerDataGrid) {
                    window.customerDataGrid.refresh();
                }
            } else {
                throw new Error(result.message || 'Save failed');
            }
        } catch (error) {
            console.error('❌ Error saving customer:', error);
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: error.message || 'Có lỗi xảy ra khi lưu thông tin khách hàng'
                });
            } else {
                alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi lưu thông tin khách hàng'));
            }
        }
    }

    // Get form data
    getFormData() {
        const formData = new FormData(this.form);
        const data = {};
        
        for (let [key, value] of formData.entries()) {
            data[key] = value;
        }
        
        // Add ID for edit mode
        if (this.isEditMode && this.currentCustomerId) {
            data.Id = this.currentCustomerId;
        }
        
        // Convert checkbox values
        data.IsLocked = document.getElementById('customerIsLocked').checked;
        
        return data;
    }

    // Validate form
    validateForm() {
        let isValid = true;
        
        // Clear previous validation
        this.form.classList.remove('was-validated');
        
        // Check required fields
        const requiredFields = this.form.querySelectorAll('[required]');
        requiredFields.forEach(field => {
            if (!field.value.trim()) {
                field.classList.add('is-invalid');
                isValid = false;
            } else {
                field.classList.remove('is-invalid');
            }
        });
        
        // Validate password match
        if (!this.isEditMode && !this.validatePasswordMatch()) {
            isValid = false;
        }
        
        this.form.classList.add('was-validated');
        return isValid;
    }

    // Validate password match
    validatePasswordMatch() {
        const password = document.getElementById('customerPassword');
        const confirmPassword = document.getElementById('customerConfirmPassword');
        
        if (!password || !confirmPassword) return true;
        
        const isMatch = password.value === confirmPassword.value;
        
        if (isMatch) {
            confirmPassword.classList.remove('is-invalid');
        } else {
            confirmPassword.classList.add('is-invalid');
        }
        
        return isMatch;
    }

    // Handle level change
    handleLevelChange() {
        const levelSelect = document.getElementById('customerLevel');
        const maxAccountFb = document.getElementById('customerMaxAccountFb');
        
        if (!levelSelect || !maxAccountFb) return;
        
        const level = parseInt(levelSelect.value);
        
        // Set default max accounts based on level
        switch (level) {
            case 1: // Customer
                maxAccountFb.value = 10;
                break;
            case 2: // Premium
                maxAccountFb.value = 25;
                break;
            case 3: // VIP
                maxAccountFb.value = 50;
                break;
        }
    }

    // Toggle password fields visibility
    togglePasswordFields(show) {
        const passwordSection = document.getElementById('passwordSection');
        const confirmPasswordSection = document.getElementById('confirmPasswordSection');
        const passwordField = document.getElementById('customerPassword');
        const confirmPasswordField = document.getElementById('customerConfirmPassword');
        
        if (passwordSection) passwordSection.style.display = show ? 'block' : 'none';
        if (confirmPasswordSection) confirmPasswordSection.style.display = show ? 'block' : 'none';
        
        if (passwordField) passwordField.required = show;
        if (confirmPasswordField) confirmPasswordField.required = show;
    }

    // Set form readonly state
    setFormReadonly(readonly) {
        const inputs = this.form.querySelectorAll('input, select, textarea');
        inputs.forEach(input => {
            input.disabled = readonly;
        });
    }

    // Set default values for new customer
    setDefaultValues() {
        document.getElementById('customerGender').value = 'Male';
        document.getElementById('customerLevel').value = '1';
        document.getElementById('customerMaxAccountFb').value = '10';
        document.getElementById('customerLanguage').value = 'vi';
        document.getElementById('customerIsLocked').checked = false;
        
        // Set register date to today
        const today = new Date().toISOString().split('T')[0];
        document.getElementById('customerRegisterDate').value = today;
        
        // Set expired date to 1 year from now
        const nextYear = new Date();
        nextYear.setFullYear(nextYear.getFullYear() + 1);
        document.getElementById('customerExpiredDate').value = nextYear.toISOString().split('T')[0];
    }

    // Set loading state
    setLoadingState(loading) {
        const saveButton = document.getElementById('saveCustomer');
        if (saveButton) {
            saveButton.disabled = loading;
            saveButton.innerHTML = loading 
                ? '<i class="bi bi-hourglass-split me-1"></i>Đang tải...'
                : '<i class="bi bi-check-lg me-1"></i>Lưu';
        }
    }

    // Modal show event
    onModalShow() {
        console.log('📖 Customer modal shown');
    }

    // Modal hidden event
    onModalHidden() {
        console.log('📕 Customer modal hidden');
        
        // Reset form
        this.form.reset();
        this.form.classList.remove('was-validated');
        
        // Clear validation states
        const invalidFields = this.form.querySelectorAll('.is-invalid');
        invalidFields.forEach(field => field.classList.remove('is-invalid'));
        
        // Reset modal state
        this.isEditMode = false;
        this.currentCustomerId = null;
        
        // Show save button
        const saveButton = document.getElementById('saveCustomer');
        if (saveButton) {
            saveButton.style.display = 'inline-block';
        }
        
        // Enable form fields
        this.setFormReadonly(false);
        
        // Show password fields
        this.togglePasswordFields(true);
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('🚀 Initializing Customer Modal...');
    window.customerModal = new CustomerModal();
    console.log('✅ Customer Modal initialized');
});

// Export for global access
window.CustomerModal = CustomerModal;
