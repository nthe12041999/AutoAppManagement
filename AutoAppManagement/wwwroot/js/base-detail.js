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

    // Inheritance-ready base controller and registry
    if (!window.BaseDetail) {
        class BaseDetail {
            constructor(formElement) { this.form = formElement; }
            onInit() {}
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

    // Ext JS integration (optional) - if Ext is available, bridge to Ext classes
    (function setupExtBridge() {
        if (!window.Ext || !Ext.define) return;
        if (!window.App) window.App = {};
        if (!window.App.detail) window.App.detail = {};

        // Define Ext-based Registry (singleton)
        Ext.define('App.detail.Registry', {
            singleton: true,
            map: {},
            register: function(formId, ctor) { this.map[formId] = ctor; },
            resolve: function(formEl) {
                const id = formEl && formEl.id;
                const Ctor = (id && this.map[id]) || App.detail.BaseDetail || window.BaseDetail;
                return new Ctor({ form: formEl });
            }
        });

        // Define BaseDetail as an Ext class that wraps BaseDetail (ES6)
        Ext.define('App.detail.BaseDetail', {
            config: { form: null },
            constructor: function(cfg) {
                this.initConfig(cfg || {});
                // mirror API of BaseDetail
                this.onInit = function(){};
                this.beforeValidate = function(){ return true; };
                this.afterValidate = function(ok){ return ok; };
                this.transformData = function(d){ return d; };
                this.beforeSubmit = function(d){ return d; };
                this.onSuccess = function(){};
                this.onError = function(){};
            }
        });

        // Bridge global DetailRegistry to Ext one
        window.DetailRegistry.register = function(formId, ctor) {
            // Allow passing ES6 classes; wrap to Ext style if needed
            App.detail.Registry.register(formId, function(cfg){
                // If ctor is an Ext class name use Ext.create
                if (typeof ctor === 'string') return Ext.create(ctor, cfg);
                // Else assume ctor is ES6 class
                return new ctor(cfg && cfg.form);
            });
        };
        window.DetailRegistry.resolve = function(formEl) {
            const inst = App.detail.Registry.resolve(formEl);
            // Normalize: if Ext class, expose .form
            if (!inst.form && inst.getForm) inst.form = inst.getForm();
            return inst;
        };
    })();

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
})();

// No auto-load of per-form detail scripts; include the correct <form>-detail.js in each view


