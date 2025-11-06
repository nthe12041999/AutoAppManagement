/**
 * Base Detail Handler
 * Generic save handler for modal/detail forms
 */
(function () {
    // Ensure binder is available (lazy-load if missing) using a single script include per page
    function ensureBinderLoaded() {
        return new Promise((resolve, reject) => {
            if (window.formControlBinder) return resolve();
            const existing = document.querySelector('script[data-autoload="form-control-binder"]');
            if (existing) {
                existing.addEventListener('load', () => resolve());
                existing.addEventListener('error', () => reject(new Error('Failed to load form-control-binder.js')));
                return;
            }
            const script = document.createElement('script');
            script.src = '/js/form-control-binder.js';
            script.async = true;
            script.defer = true;
            script.setAttribute('data-autoload', 'form-control-binder');
            script.onload = () => resolve();
            script.onerror = () => reject(new Error('Failed to load form-control-binder.js'));
            document.head.appendChild(script);
        });
    }

    // Core BaseDetail and DetailRegistry setup
    if (!window.BaseDetail) {
        class BaseDetail {
            constructor(formElement) { 
                this.form = formElement; 
                this.isViewMode = false;
            }
            onInit() {
                // Auto-detect view mode from modal or form attributes
                this.detectAndSetViewMode();
                
                // Also set up a periodic check for view mode detection
                const checkViewMode = () => {
                    if (!this.form) return;
                    
                    const $form = $(this.form);
                    const $modal = $form.closest('.modal');
                    
                    if ($modal.length && $modal.hasClass('show')) {
                        const modalTitle = $modal.find('.modal-title').text().toLowerCase();
                        console.log('🔄 Periodic check - Modal title:', modalTitle);
                        
                        if ((modalTitle.includes('xem chi tiết') || modalTitle.includes('xem') || modalTitle.includes('view') || modalTitle.includes('chi tiết')) && !$form.hasClass('view-mode')) {
                            console.log('🔍 Periodic check detected view mode, activating...');
                            this.setViewMode();
                        }
                    }
                };
                
                // Check after delays
                setTimeout(checkViewMode, 500);
                setTimeout(checkViewMode, 1000);
                setTimeout(checkViewMode, 2000);
            }
            detectAndSetViewMode() {
                if (!this.form) return;
                
                const $form = $(this.form);
                const $modal = $form.closest('.modal');
                
                // Check various indicators for view mode with delay to ensure DOM is ready
                setTimeout(() => {
                    const modalTitle = $modal.find('.modal-title').text().toLowerCase();
                    console.log('🔍 Checking modal title for view mode:', modalTitle);
                    
                    const isViewMode = 
                        modalTitle.includes('xem chi tiết') ||
                        modalTitle.includes('xem') ||
                        modalTitle.includes('view') ||
                        modalTitle.includes('chi tiết') ||
                        $modal.attr('data-mode') === 'view' ||
                        $form.attr('data-mode') === 'view' ||
                        $form.hasClass('view-mode') ||
                        window.location.search.includes('mode=view');
                    
                    console.log('🔍 View mode detected:', isViewMode);
                    
                    if (isViewMode) {
                        console.log('🔒 Activating view mode for form');
                        this.setViewMode();
                    }
                }, 100);
            }
            setViewMode(modalTitle) {
                this.isViewMode = true;
                window.setFormViewMode($(this.form), modalTitle);
            }
            beforeValidate() { return true; }
            afterValidate(isValid) { return isValid; }
            transformData(data) { return data; }
            beforeSubmit(data) { return data; }
            onSuccess(response) {}
            onError(error) {}
        }
        window.BaseDetail = BaseDetail;
    }

    if (!window.DetailRegistry) {
        window.DetailRegistry = {
            map: {},
            register: function(formId, ctor) { this.map[formId] = ctor; },
            resolve: function(formEl) {
                const id = formEl && formEl.id;
                const Ctor = (id && this.map[id]) || window.BaseDetail;
                return new Ctor(formEl);
            }
        };
    }

    // Note: Auto-load per-form detail script was removed. Each page should include its own <form>-detail.js.
    // ===== Minimal built-in binder (fallback when form-control-binder.js is not present) =====
    function basicIsEmpty(val) {
        return val == null || String(val).trim() === '';
    }

    function basicGetFieldLabel(field) {
        const $label = $(field).closest('.mb-3, .form-group').find('label');
        return $label.length > 0 ? $label.text().replace('*', '').trim() : 'Trường này';
    }

    function basicValidateField(field) {
        const $el = $(field);
        const label = basicGetFieldLabel(field);
        const value = field.type === 'checkbox' || field.type === 'radio' ? (field.checked ? field.value : '') : (field.value || '');

        // required
        const isRequired = field.hasAttribute('required') || $el.is('[data-required]');
        if (isRequired && basicIsEmpty(value)) {
            return { ok: false, msg: `${label} là bắt buộc` };
        }
        if (basicIsEmpty(value)) return { ok: true, msg: '' };

        // min/max length
        const minLen = parseInt($el.attr('data-min-length') || field.minLength || 0, 10);
        const maxLen = parseInt($el.attr('data-max-length') || field.maxLength || 0, 10);
        if (minLen > 0 && value.length < minLen) return { ok: false, msg: `${label} phải có ít nhất ${minLen} ký tự` };
        if (maxLen > 0 && value.length > maxLen) return { ok: false, msg: `${label} không được quá ${maxLen} ký tự` };

        // regex
        const regexStr = $el.attr('data-regex');
        if (regexStr) {
            try {
                const rx = new RegExp(regexStr);
                if (!rx.test(value)) return { ok: false, msg: $el.attr('data-regex-message') || `${label} không đúng định dạng` };
            } catch {}
        }

        // valid values
        const validValues = $el.attr('data-valid-values');
        if (validValues) {
            const arr = validValues.split(',').map(s => s.trim());
            if (!arr.includes(String(value))) return { ok: false, msg: `${label} không hợp lệ` };
        }

        // match (e.g., confirm password)
        const matchField = $el.attr('data-match');
        if (matchField) {
            const $match = $(field.form).find(`[name="${matchField}"], [data-name="${matchField}"]`);
            if ($match.length && value !== $match.val()) return { ok: false, msg: $el.attr('data-match-message') || `${label} không khớp` };
        }

        // max-date (support data-max-today and auto message)
        const formatDateVN = (dStr) => {
            if (!dStr || typeof dStr !== 'string' || dStr.length < 10) return dStr;
            const [y, m, d] = dStr.substring(0,10).split('-');
            return `${d}/${m}/${y}`;
        };
        let maxDate = $el.attr('data-max-date');
        if (!maxDate && $el.is('[data-max-today]')) {
            maxDate = new Date().toISOString().slice(0, 10);
        }
        if (field.type === 'date' && maxDate) {
            if (value && value > maxDate) {
                const msgOverride = $el.attr('data-max-date-message');
                const msg = msgOverride || ($el.is('[data-max-today]')
                    ? `${label} không được lớn hơn ngày hiện tại`
                    : `${label} không được lớn hơn ${formatDateVN(maxDate)}`);
                return { ok: false, msg };
            }
        }

        return { ok: true, msg: '' };
    }

    function basicApplyValidation(field, isOk, msg) {
        const $f = $(field);
        $f.removeClass('is-valid is-invalid');
        $f.parent().find('.invalid-feedback').remove();
        if (isOk) {
            $f.addClass('is-valid');
        } else {
            $f.addClass('is-invalid');
            $f.parent().append($('<div class="invalid-feedback"></div>').text(msg));
        }
    }

    function basicValidateForm(form) {
        let ok = true;
        $(form).find('input,select,textarea').each((_, field) => {
            const v = basicValidateField(field);
            basicApplyValidation(field, v.ok, v.msg);
            if (!v.ok) ok = false;
        });
        return ok;
    }

    function basicGetFormData(form) {
        const fd = new FormData(form);
        const data = Object.fromEntries(fd.entries());
        // handle multi-select
        $(form).find('select[multiple]').each((_, s) => {
            const name = s.name;
            const vals = Array.from(s.selectedOptions).map(o => o.value);
            data[name] = vals;
        });
        // checkbox arrays
        $(form).find('input[type="checkbox"][name$="[]"]').each((_, c) => {
            const base = c.name.replace('[]', '');
            if (!Array.isArray(data[base])) data[base] = [];
            if (c.checked) data[base].push(c.value);
        });
        return data;
    }

    function basicNotify(message, type = 'info') {
        // fallback toast
        const cls = type === 'error' ? 'alert-danger' : type === 'success' ? 'alert-success' : 'alert-info';
        const el = document.createElement('div');
        el.className = `alert ${cls} alert-dismissible fade show position-fixed`;
        el.style.cssText = 'top:20px;right:20px;z-index:9999;min-width:320px;max-width:480px;';
        el.innerHTML = `<button type="button" class="btn-close" data-bs-dismiss="alert"></button><div>${message}</div>`;
        document.body.appendChild(el);
        setTimeout(() => el.remove(), 3500);
    }
    function mapModeToState(mode) {
        const stateMap = { add: 1, create: 1, new: 1, edit: 2, update: 2, modify: 2, delete: 3, remove: 3 };
        const key = (mode || '').toLowerCase();
        return stateMap[key];
    }

    function ensureTypes(payload) {
        if (payload.Status !== undefined && payload.Status !== null && typeof payload.Status === 'string' && !isNaN(payload.Status)) {
            payload.Status = parseInt(payload.Status, 10);
        }
        if (payload.State !== undefined && payload.State !== null && typeof payload.State === 'string' && !isNaN(payload.State)) {
            payload.State = parseInt(payload.State, 10);
        }
        if (payload.ID !== undefined && payload.ID !== null && typeof payload.ID === 'string' && !isNaN(payload.ID)) {
            payload.ID = parseInt(payload.ID, 10);
        }
        return payload;
    }

    function removeEmptyStrings(payload) {
        if (!payload || typeof payload !== 'object') return payload;
        const cleaned = Array.isArray(payload) ? [] : {};
        Object.keys(payload).forEach((key) => {
            const value = payload[key];
            if (value === '') {
                // drop empty strings to avoid model binding errors (e.g., nullable DateTime)
                return;
            }
            if (value && typeof value === 'object' && !Array.isArray(value)) {
                cleaned[key] = removeEmptyStrings(value);
            } else {
                cleaned[key] = value;
            }
        });
        return cleaned;
    }

    window.saveDetailForm = function saveDetailForm(actionUrl, formMethod, mode) {
        const $modal = $('.modal.show');
        if ($modal.length === 0) {
            console.error('No active modal found');
            return;
        }

        // Prefer inner form if exists; otherwise use modal content as container
        const $form = $modal.find('form').length ? $modal.find('form') : $modal.find('.modal-content');
        if ($form.length === 0) {
            console.error('No form found in modal');
            return;
        }

        const $saveBtn = $modal.find('#saveBtn');
        if ($saveBtn.length > 0) {
            $saveBtn.prop('disabled', true);
            $saveBtn.html('<i class="bi bi-hourglass-split me-1"></i>Đang lưu...');
        }

        // Resolve controller for this form and call hooks
        const controller = (function () {
            const ctrl = $form.data('detailCtrl') || window.DetailRegistry.resolve($form[0]);
            $form.data('detailCtrl', ctrl);
            try { if (typeof ctrl.onInit === 'function') ctrl.onInit(); } catch {}
            return ctrl;
        })();

        // Validate using binder if available (lazy-load), else fallback
        ensureBinderLoaded().then(() => {
            if (controller && typeof controller.beforeValidate === 'function') {
                const ok = controller.beforeValidate();
                if (!ok) {
                    if ($saveBtn.length > 0) {
                        $saveBtn.prop('disabled', false);
                        $saveBtn.html('<i class="bi bi-check-circle me-1"></i>Lưu');
                    }
                    return;
                }
            }

            const validateOk = window.formControlBinder
                ? window.formControlBinder.validateForm($form[0])
                : basicValidateForm($form[0]);

            const finalOk = controller && typeof controller.afterValidate === 'function'
                ? controller.afterValidate(!!validateOk)
                : !!validateOk;
            if (!finalOk) {
                if ($saveBtn.length > 0) {
                    $saveBtn.prop('disabled', false);
                    $saveBtn.html('<i class="bi bi-check-circle me-1"></i>Lưu');
                }
                if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') {
                    window.formControlBinder.showNotification('❌ Vui lòng kiểm tra lại thông tin!', 'error');
                } else {
                    basicNotify('❌ Vui lòng kiểm tra lại thông tin!', 'error');
                }
                return;
            }

            // Build payload
            let data = window.formControlBinder ? window.formControlBinder.getFormData($form[0]) : basicGetFormData($form[0]);
            data = removeEmptyStrings(data);
            if (controller && typeof controller.transformData === 'function') {
                try { const t = controller.transformData({ ...data }); if (t && typeof t === 'object') data = t; } catch {}
            }

        // Attach Mode/State
        const resolvedMode = (mode || $modal.find('.modal-footer').attr('data-mode') || '').toLowerCase();
        if (resolvedMode) {
            data.Mode = resolvedMode;
            const mapped = mapModeToState(resolvedMode);
            if (mapped && !data.State) data.State = mapped;
        } else {
            if (data.ID && parseInt(data.ID) > 0 && !data.State) {
                data.State = 2; // Edit
                data.Mode = 'edit';
            } else if (!data.State) {
                data.State = 1; // Add
                data.Mode = 'add';
            }
        }

        // Allow page-specific customization before submit
        if (typeof window.customerDetailCustomize === 'function') {
            try {
                const customized = window.customerDetailCustomize({ ...data });
                if (customized && typeof customized === 'object') {
                    data = customized;
                }
            } catch (e) {
                console.warn('customerDetailCustomize error:', e);
            }
        }

        // Ensure numeric types where needed
        data = ensureTypes(removeEmptyStrings(data));
        if (controller && typeof controller.beforeSubmit === 'function') {
            try { const t = controller.beforeSubmit({ ...data }); if (t && typeof t === 'object') data = t; } catch {}
        }

            // Submit via AJAX JSON (no external dependency)
            $.ajax({
                url: actionUrl,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data)
            }).done((resp) => {
                if (resp && (resp.Success === true || resp.IsSuccess === true)) {
                    const successMsg = (resp && (resp.Message || resp.message)) || 'Lưu thành công!';
                    if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') {
                        window.formControlBinder.showNotification('✅ ' + successMsg, 'success');
                    } else {
                        basicNotify('✅ ' + successMsg, 'success');
                    }
                    try { if (controller && typeof controller.onSuccess === 'function') controller.onSuccess(resp); } catch {}
                    $modal.closest('.modal-container').remove();
                    if (window.dataGridInstance && typeof window.dataGridInstance.refreshData === 'function') {
                        // Try refresh all grids
                        const grids = window.dataGridInstance.getAllGrids ? window.dataGridInstance.getAllGrids() : null;
                        if (grids && grids.forEach) {
                            grids.forEach((cfg) => window.dataGridInstance.refreshData(cfg));
                        }
                    }
                } else {
                    const msg = (resp && (resp.Message || resp.message)) || 'Có lỗi xảy ra';
                    if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') {
                        window.formControlBinder.showNotification('❌ ' + msg, 'error');
                    } else {
                        basicNotify('❌ ' + msg, 'error');
                    }
                    try { if (controller && typeof controller.onError === 'function') controller.onError(resp); } catch {}
                }
                if ($saveBtn.length > 0) {
                    $saveBtn.prop('disabled', false);
                    $saveBtn.html('<i class="bi bi-check-circle me-1"></i>Lưu');
                }
            }).fail((xhr, status, error) => {
                console.error('Detail form submission error:', error);
                
                // Parse error response để lấy message
                let errorMessage = 'Có lỗi xảy ra khi lưu';
                try {
                    if (xhr.responseJSON) {
                        errorMessage = xhr.responseJSON.Message || xhr.responseJSON.message || errorMessage;
                    } else if (xhr.responseText) {
                        const parsed = JSON.parse(xhr.responseText);
                        errorMessage = parsed.Message || parsed.message || errorMessage;
                    }
                } catch (e) {
                    // Keep default message
                }
                
                if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') {
                    window.formControlBinder.showNotification('❌ ' + errorMessage, 'error');
                } else {
                    basicNotify('❌ ' + errorMessage, 'error');
                }
                try { if (controller && typeof controller.onError === 'function') controller.onError(error); } catch {}
                if ($saveBtn.length > 0) {
                    $saveBtn.prop('disabled', false);
                    $saveBtn.html('<i class="bi bi-check-circle me-1"></i>Lưu');
                }
            });
        }).catch(() => {
            // Binder failed to load; fallback to basic path
            if (controller && typeof controller.beforeValidate === 'function') {
                const ok = controller.beforeValidate();
                if (!ok) {
                    if ($saveBtn.length > 0) {
                        $saveBtn.prop('disabled', false);
                        $saveBtn.html('<i class=\"bi bi-check-circle me-1\"></i>Lưu');
                    }
                    return;
                }
            }
            const validateOk = basicValidateForm($form[0]);
            const finalOk = controller && typeof controller.afterValidate === 'function'
                ? controller.afterValidate(!!validateOk)
                : !!validateOk;
            if (!finalOk) {
                if ($saveBtn.length > 0) {
                    $saveBtn.prop('disabled', false);
                    $saveBtn.html('<i class="bi bi-check-circle me-1"></i>Lưu');
                }
                basicNotify('❌ Vui lòng kiểm tra lại thông tin!', 'error');
                return;
            }
            let data = removeEmptyStrings(basicGetFormData($form[0]));
            if (controller && typeof controller.transformData === 'function') {
                try { const t = controller.transformData({ ...data }); if (t && typeof t === 'object') data = t; } catch {}
            }
            const resolvedMode = (mode || $modal.find('.modal-footer').attr('data-mode') || '').toLowerCase();
            if (resolvedMode) {
                data.Mode = resolvedMode;
                const mapped = mapModeToState(resolvedMode);
                if (mapped && !data.State) data.State = mapped;
            } else {
                if (data.ID && parseInt(data.ID) > 0 && !data.State) {
                    data.State = 2;
                    data.Mode = 'edit';
                } else if (!data.State) {
                    data.State = 1;
                    data.Mode = 'add';
                }
            }
            if (typeof window.customerDetailCustomize === 'function') {
                try { const customized = window.customerDetailCustomize({ ...data }); if (customized && typeof customized === 'object') data = customized; } catch {}
            }
            data = ensureTypes(removeEmptyStrings(data));
            if (controller && typeof controller.beforeSubmit === 'function') {
                try { const t = controller.beforeSubmit({ ...data }); if (t && typeof t === 'object') data = t; } catch {}
            }
            $.ajax({
                url: actionUrl,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data)
            }).done((resp) => {
                basicNotify('✅ Lưu thành công!', 'success');
                try { if (controller && typeof controller.onSuccess === 'function') controller.onSuccess(resp); } catch {}
                $modal.closest('.modal-container').remove();
            }).fail((err) => {
                basicNotify('❌ Có lỗi xảy ra khi lưu', 'error');
                try { if (controller && typeof controller.onError === 'function') controller.onError(err); } catch {}
                if ($saveBtn.length > 0) {
                    $saveBtn.prop('disabled', false);
                    $saveBtn.html('<i class="bi bi-check-circle me-1"></i>Lưu');
                }
            });
        });
    };

    /**
     * Generic function to set any form to view mode (read-only)
     * Can be used by any form across the application
     * @param {jQuery|string} formSelector - Form selector or jQuery object
     * @param {string} modalTitle - Optional custom modal title
     */
    window.setFormViewMode = function(formSelector, modalTitle = 'Xem Chi Tiết') {
        const $form = typeof formSelector === 'string' ? $(formSelector) : formSelector;
        
        if (!$form.length) {
            console.warn('Form not found for view mode:', formSelector);
            return;
        }
        
        console.log('🔒 Setting form to view mode:', $form.attr('id') || 'unknown form');
        
        // Disable all form controls
        $form.find('input, select, textarea, button[type="submit"]').each(function() {
            const $element = $(this);
            
            // Skip hidden inputs and already processed elements
            if ($element.attr('type') === 'hidden' || $element.hasClass('view-mode-processed')) {
                return;
            }
            
            // Mark as processed to avoid double processing
            $element.addClass('view-mode-processed');
            
            // Store original state for potential restoration
            $element.data('original-disabled', $element.prop('disabled'));
            $element.data('original-readonly', $element.prop('readonly'));
            
            // Disable the element but keep it visible with visual indication
            $element.prop('disabled', true);
            $element.prop('readonly', true);
            $element.addClass('view-mode-disabled');
            
            // Add visual styling to indicate disabled state
            $element.css({
                'background-color': '#f8f9fa',
                'color': '#6c757d',
                'cursor': 'not-allowed',
                'opacity': '0.8'
            });
        });
        
        // Handle switch controls - disable but keep visible
        $form.find('.form-switch input[type="checkbox"]').each(function() {
            const $switch = $(this);
            if ($switch.hasClass('view-mode-processed')) return;
            
            $switch.addClass('view-mode-processed');
            $switch.prop('disabled', true);
            $switch.css('cursor', 'not-allowed');
            
            // Disable the entire switch container
            $switch.closest('.form-switch').css({
                'opacity': '0.6',
                'cursor': 'not-allowed'
            });
        });
        
        // Handle file upload controls - disable but keep visible
        $form.find('input[type="file"]').each(function() {
            const $fileInput = $(this);
            $fileInput.prop('disabled', true);
            $fileInput.css({
                'background-color': '#f8f9fa',
                'cursor': 'not-allowed',
                'opacity': '0.6'
            });
        });
        
        // Hide all submit/save buttons - more comprehensive search
        $form.find('button[type="submit"]').hide();
        $form.find('button:contains("Lưu")').hide();
        $form.find('.btn-primary:contains("Lưu")').hide();
        $form.find('.btn-success:contains("Lưu")').hide();
        $form.find('#saveBtn').hide();
        $form.find('.btn[onclick*="save"]').hide();
        
        // Update modal title if in modal
        const $modal = $form.closest('.modal');
        if ($modal.length) {
            const currentTitle = $modal.find('.modal-title').text();
            if (!currentTitle.includes('Xem')) {
                $modal.find('.modal-title').html(`<i class="bi bi-eye me-2"></i>${modalTitle}`);
            }
            
            // Hide footer save buttons more comprehensively
            $modal.find('.modal-footer button[type="submit"]').hide();
            $modal.find('.modal-footer button:contains("Lưu")').hide();
            $modal.find('.modal-footer .btn-primary:contains("Lưu")').hide();
            $modal.find('.modal-footer .btn-success:contains("Lưu")').hide();
            $modal.find('.modal-footer #saveBtn').hide();
            $modal.find('.modal-footer .btn[onclick*="save"]').hide();
            
            // Update close button
            let $closeBtn = $modal.find('.modal-footer .btn-secondary');
            if ($closeBtn.length === 0) {
                // If no secondary button, look for any close button
                $closeBtn = $modal.find('.modal-footer button:contains("Đóng"), .modal-footer button:contains("Hủy"), .modal-footer button[data-bs-dismiss="modal"]');
            }
            if ($closeBtn.length > 0) {
                $closeBtn.text('Đóng').removeClass('btn-secondary btn-outline-secondary').addClass('btn-outline-primary');
            } else {
                // Add close button if not found
                $modal.find('.modal-footer').append('<button type="button" class="btn btn-outline-primary" data-bs-dismiss="modal">Đóng</button>');
            }
            
            // Add Edit button to switch to edit mode
            if (!$modal.find('.modal-footer .btn-edit-mode').length) {
                const $editBtn = $('<button type="button" class="btn btn-warning btn-edit-mode me-2"><i class="bi bi-pencil me-1"></i>Sửa</button>');
                $editBtn.on('click', function() {
                    window.restoreFormFromViewMode($form);
                });
                $modal.find('.modal-footer').prepend($editBtn);
            }
        }
        
        // Add view-mode class to form
        $form.addClass('view-mode');
        
        // Add CSS styles for better view mode appearance
        if (!$('head').find('#view-mode-styles').length) {
            $('head').append(`
                <style id="view-mode-styles">
                    .view-mode .view-mode-display {
                        padding: 0.375rem 0;
                        font-weight: 500;
                        color: #495057;
                        border-bottom: 1px solid #e9ecef;
                        margin-bottom: 0.5rem;
                    }
                    .view-mode .view-mode-display:empty:before {
                        content: "Chưa có thông tin";
                        color: #6c757d;
                        font-style: italic;
                    }
                    .view-mode .form-label {
                        font-weight: 600;
                        color: #343a40;
                    }
                    .view-mode-indicator {
                        animation: fadeIn 0.3s ease-in;
                    }
                    @keyframes fadeIn {
                        from { opacity: 0; transform: translateY(-10px); }
                        to { opacity: 1; transform: translateY(0); }
                    }
                </style>
            `);
        }
        
        console.log('✅ Form successfully set to view mode');
    };
    
    /**
     * Function to restore form from view mode to edit mode
     * @param {jQuery|string} formSelector - Form selector or jQuery object
     */
    window.restoreFormFromViewMode = function(formSelector) {
        const $form = typeof formSelector === 'string' ? $(formSelector) : formSelector;
        
        if (!$form.length || !$form.hasClass('view-mode')) {
            console.warn('Form not found or not in view mode:', formSelector);
            return;
        }
        
        console.log('🔓 Restoring form from view mode:', $form.attr('id') || 'unknown form');
        
        // Remove view mode indicator
        $form.find('.view-mode-indicator').remove();
        
        // Restore all form controls
        $form.find('.view-mode-processed').each(function() {
            const $element = $(this);
            
            // Restore original disabled/readonly state
            const originalDisabled = $element.data('original-disabled') || false;
            const originalReadonly = $element.data('original-readonly') || false;
            
            $element.prop('disabled', originalDisabled);
            $element.prop('readonly', originalReadonly);
            $element.removeClass('view-mode-disabled view-mode-processed');
            
            // Remove data attributes
            $element.removeData('original-disabled original-readonly');
            
            // Restore original styling
            $element.css({
                'background-color': '',
                'color': '',
                'cursor': '',
                'opacity': ''
            });
        });
        
        // Restore switch controls
        $form.find('.form-switch').css({
            'opacity': '',
            'cursor': ''
        });
        
        // Restore file inputs
        $form.find('input[type="file"]').css({
            'background-color': '',
            'cursor': '',
            'opacity': ''
        });
        
        // Show buttons
        $form.find('button[type="submit"], .btn-primary, .btn-success, #saveBtn').show();
        
        // Update modal if in modal
        const $modal = $form.closest('.modal');
        if ($modal.length) {
            // Show save buttons
            $modal.find('.modal-footer .btn-primary, .modal-footer .btn-success, .modal-footer #saveBtn').show();
            $modal.find('.modal-footer button[type="submit"]').show();
            $modal.find('.modal-footer button:contains("Lưu")').show();
            
            // Update close button back to "Hủy"
            const $closeBtn = $modal.find('.modal-footer .btn-outline-primary');
            if ($closeBtn.length) {
                $closeBtn.text('Hủy').removeClass('btn-outline-primary').addClass('btn-secondary');
            }
            
            // Hide edit button
            $modal.find('.modal-footer .btn-edit-mode').hide();
            
            // Update modal title back to edit
            const currentTitle = $modal.find('.modal-title').text();
            if (currentTitle.includes('Xem Chi Tiết')) {
                $modal.find('.modal-title').html(`<i class="bi bi-pencil me-2"></i>${currentTitle.replace('Xem Chi Tiết', 'Chỉnh Sửa')}`);
            }
        }
        
        // Remove view-mode class
        $form.removeClass('view-mode');
        
        console.log('✅ Form successfully restored from view mode');
    };

    // Global modal event handler to auto-detect view mode
    $(document).on('shown.bs.modal', '.modal', function() {
        const $modal = $(this);
        const modalTitle = $modal.find('.modal-title').text().toLowerCase();
        
        console.log('📋 Modal shown with title:', modalTitle);
        
        // Check if this is a view modal - more comprehensive check
        const isViewModal = 
            modalTitle.includes('xem chi tiết') || 
            modalTitle.includes('xem') || 
            modalTitle.includes('view') || 
            modalTitle.includes('chi tiết') ||
            modalTitle.includes('detail') ||
            $modal.attr('data-mode') === 'view' ||
            $modal.find('form').attr('data-mode') === 'view';
        
        if (isViewModal) {
            console.log('🔍 View modal detected, setting view mode');
            
            // Find form in modal and set view mode
            const $form = $modal.find('form').first();
            if ($form.length) {
                // Use longer delay to ensure everything is rendered
                setTimeout(() => {
                    console.log('🔒 Setting view mode with delay');
                    window.setFormViewMode($form, modalTitle);
                }, 300); // Increased delay
            } else {
                console.warn('⚠️ No form found in view modal');
            }
        } else {
            console.log('📝 Edit modal detected, keeping edit mode');
        }
    });

    // Also handle when modal is about to show
    $(document).on('show.bs.modal', '.modal', function() {
        const $modal = $(this);
        const modalTitle = $modal.find('.modal-title').text().toLowerCase();
        
        console.log('📋 Modal showing with title:', modalTitle);
        
        // Pre-check for view mode to prepare
        if (modalTitle.includes('xem chi tiết') || modalTitle.includes('xem') || modalTitle.includes('view') || modalTitle.includes('chi tiết')) {
            console.log('🔍 Pre-detected view modal');
            $modal.attr('data-detected-view-mode', 'true');
        }
    });

    // Global function to check and set view mode for any modal
    window.checkAndSetViewMode = function($modal) {
        if (!$modal || !$modal.length) {
            $modal = $('.modal.show');
        }
        
        if ($modal.length) {
            const modalTitle = $modal.find('.modal-title').text().toLowerCase();
            console.log('🔍 Checking modal for view mode:', modalTitle);
            
            if (modalTitle.includes('xem chi tiết') || modalTitle.includes('xem') || modalTitle.includes('view') || modalTitle.includes('chi tiết')) {
                const $form = $modal.find('form').first();
                if ($form.length && !$form.hasClass('view-mode')) {
                    console.log('🔒 Setting view mode via checkAndSetViewMode');
                    window.setFormViewMode($form, modalTitle);
                    return true;
                }
            }
        }
        return false;
    };

    // Fallback: Check for view mode on DOM ready and periodically
    $(document).ready(function() {
        // Check immediately
        setTimeout(function() {
            window.checkAndSetViewMode();
        }, 100);
        
        // Set up periodic check for view modals
        setInterval(function() {
            const $modal = $('.modal.show');
            if ($modal.length) {
                const modalTitle = $modal.find('.modal-title').text().toLowerCase();
                if ((modalTitle.includes('xem chi tiết') || modalTitle.includes('xem') || modalTitle.includes('view') || modalTitle.includes('chi tiết'))) {
                    const $form = $modal.find('form').first();
                    if ($form.length && !$form.hasClass('view-mode')) {
                        console.log('🔄 Periodic check activating view mode');
                        window.setFormViewMode($form, modalTitle);
                    }
                }
            }
        }, 1000);
    });

    // Debug functions for manual testing
    window.debugForceViewMode = function() {
        console.log('🔧 DEBUG: Forcing view mode on current modal');
        const $modal = $('.modal.show');
        if ($modal.length) {
            const $form = $modal.find('form').first();
            if ($form.length) {
                window.setFormViewMode($form, 'Xem Chi Tiết (Debug)');
                console.log('✅ DEBUG: View mode activated');
            } else {
                console.warn('❌ DEBUG: No form found in current modal');
            }
        } else {
            console.warn('❌ DEBUG: No active modal found');
        }
    };

    window.debugRestoreEditMode = function() {
        console.log('🔧 DEBUG: Restoring edit mode on current modal');
        const $modal = $('.modal.show');
        if ($modal.length) {
            const $form = $modal.find('form').first();
            if ($form.length) {
                window.restoreFormFromViewMode($form);
                console.log('✅ DEBUG: Edit mode restored');
            } else {
                console.warn('❌ DEBUG: No form found in current modal');
            }
        } else {
            console.warn('❌ DEBUG: No active modal found');
        }
    };

    window.debugCheckModal = function() {
        console.log('🔍 DEBUG: Checking current modal state');
        const $modal = $('.modal.show');
        if ($modal.length) {
            const modalTitle = $modal.find('.modal-title').text();
            const $form = $modal.find('form').first();
            const isViewMode = $form.hasClass('view-mode');
            
            console.log('📋 Modal Title:', modalTitle);
            console.log('📝 Form ID:', $form.attr('id') || 'no-id');
            console.log('🔒 Is View Mode:', isViewMode);
            console.log('🎯 Form Controls Count:', $form.find('input, select, textarea').length);
            console.log('👁️ View Display Count:', $form.find('.view-mode-display').length);
            console.log('🔘 Buttons Visible:', $modal.find('button:visible').map((i, btn) => $(btn).text()).get());
        } else {
            console.warn('❌ DEBUG: No active modal found');
        }
    };
})();

// No auto-load of per-form detail scripts; include the correct <form>-detail.js in each view


