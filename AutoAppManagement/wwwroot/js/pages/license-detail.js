/**
 * License Detail (Ext/ES6 controller)
 */

function validateLicenseElementWithDataAttributes($element, value, isRequired) {
    const result = { IsValid: true, Message: '' };
    let fieldName = $element.attr('data-label') || $element.attr('placeholder') || $element.closest('.mb-3, .form-group').find('label').text() || $element.attr('name') || 'Trường này';
    fieldName = fieldName.replace('*', '').trim();
    if (isRequired && (value === null || value === undefined || value === '')) {
        result.IsValid = false; result.Message = `${fieldName} là bắt buộc`; return result;
    }
    if (!value) { return result; }
    if ($element.attr('name') === 'licenseKey' && value) {
        if (!value.match(/^LIC-\d{4}-[A-Z0-9]{3,}$/)) { result.IsValid = false; result.Message = 'License key phải có định dạng LIC-YYYY-XXX'; return result; }
    }
    if ($element.attr('name') === 'maxDevices' && value) {
        const numValue = parseInt(value, 10); if (isNaN(numValue) || numValue < 1 || numValue > 1000) { result.IsValid = false; result.Message = 'Số thiết bị phải từ 1 đến 1000'; return result; }
    }
    if ($element.attr('name') === 'price' && value) {
        const numValue = parseFloat(value); if (isNaN(numValue) || numValue < 0) { result.IsValid = false; result.Message = 'Giá không được âm'; return result; }
    }
    if ($element.attr('type') === 'date' || $element.attr('data-type') === 'date') {
        const startDateField = $('#startDate'); if ($element.attr('name') === 'expiryDate' && startDateField.length > 0) { const startDate = startDateField.val(); if (startDate && value && new Date(value) <= new Date(startDate)) { result.IsValid = false; result.Message = 'Ngày hết hạn phải sau ngày bắt đầu'; return result; } }
    }
    const minLength = $element.attr('data-min-length'); const maxLength = $element.attr('data-max-length');
    if (minLength && value.length < parseInt(minLength)) { result.IsValid = false; result.Message = `${fieldName} phải có ít nhất ${minLength} ký tự`; return result; }
    if (maxLength && value.length > parseInt(maxLength)) { result.IsValid = false; result.Message = `${fieldName} không được vượt quá ${maxLength} ký tự`; return result; }
    const regex = $element.attr('data-regex'); const regexMessage = $element.attr('data-regex-message');
    if (regex && value) { try { const regexPattern = new RegExp(regex); if (!regexPattern.test(value)) { result.IsValid = false; result.Message = regexMessage || `${fieldName} không đúng định dạng`; return result; } } catch (e) { console.warn('Invalid regex pattern:', regex); } }
    const validValues = $element.attr('data-valid-values'); if (validValues && value) { const validArray = validValues.split(',').map(v => v.trim()); if (!validArray.includes(value.toString())) { result.IsValid = false; result.Message = `${fieldName} không hợp lệ`; return result; } }
    return result;
}

function validateLicenseForm(formData) {
    const result = { isValid: true, errors: [], data: {} };
    function addError(field, message) { result.isValid = false; result.errors.push({ field, message }); }
    const $form = $('.modal.show form, #licenseForm');
    if ($form.length > 0) {
        $form.find('input, select, textarea, [data-name]').each(function() {
            const $element = $(this); const name = $element.attr('name') || $element.attr('data-name'); const value = $element.val(); const isRequired = $element.is('[required]') || $element.is('[data-required]');
            if (!name) return; const validation = validateLicenseElementWithDataAttributes($element, value, isRequired);
            if (!validation.IsValid) { addError(name, validation.Message); } else if (value && value.toString().trim() !== '') { result.data[name] = value; }
        });
    }
    $form.find('input[type="checkbox"][name$="[]"]').each(function() { const name = $(this).attr('name').replace('[]', ''); if (!Array.isArray(result.data[name])) { result.data[name] = []; } if ($(this).is(':checked')) { result.data[name].push($(this).val()); } });
    if (!result.data.status) { result.data.status = 'pending'; }
    if (!result.data.licenseType) { result.data.licenseType = 'basic'; }
    if (result.data.maxDevices && typeof result.data.maxDevices === 'string') { result.data.maxDevices = parseInt(result.data.maxDevices, 10); }
    if (result.data.price && typeof result.data.price === 'string') { result.data.price = parseFloat(result.data.price); }
    if (result.data.autoRenewal !== undefined) { result.data.autoRenewal = result.data.autoRenewal === 'true' || result.data.autoRenewal === true; }
    if (result.data.sendNotifications !== undefined) { result.data.sendNotifications = result.data.sendNotifications === 'true' || result.data.sendNotifications === true; }
    return result;
}

(function () {
    function ensureBaseDetailReady(cb) {
        if (window.BaseDetail && window.DetailRegistry) { cb(); return; }
        setTimeout(function () { ensureBaseDetailReady(cb); }, 50);
    }

    ensureBaseDetailReady(function () {
        var registerWithId = function(ctorOrName) {
            window.DetailRegistry.register('licenseForm', ctorOrName);
            $(document).ready(function () {
                const form = document.getElementById('licenseForm');
                if (form) {
                    const ctrl = window.DetailRegistry.resolve(form);
                    $(form).data('detailCtrl', ctrl);
                    if (typeof ctrl.onInit === 'function') ctrl.onInit();
                }
            });
        };

        if (window.Ext && Ext.define) {
            if (!window.App) window.App = {}; if (!App.detail) App.detail = {};
            Ext.define('App.detail.LicenseController', {
                extend: 'App.detail.BaseDetail',
                onInit: function() {
                    $('#customer').on('input', function() {
                        const customer = $(this).val();
                        if (customer && !$('#licenseKey').val()) {
                            const year = new Date().getFullYear();
                            const randomCode = Math.random().toString(36).substring(2, 5).toUpperCase();
                            const licenseKey = `LIC-${year}-${randomCode}`;
                            $('#licenseKey').val(licenseKey);
                        }
                    });
                    $('#startDate, #expiryDate').on('change', function() {
                        const startDate = $('#startDate').val();
                        const expiryDate = $('#expiryDate').val();
                        if (startDate && expiryDate && new Date(startDate) >= new Date(expiryDate)) {
                            $('#expiryDate').addClass('is-invalid');
                        } else {
                            $('#expiryDate').removeClass('is-invalid');
                        }
                    });
                    $('#notes').on('input', function() {
                        const length = this.value.length; const maxLength = 500;
                        let counter = $(this).parent().find('.character-counter');
                        if (!counter.length) { counter = $('<small class="character-counter text-muted"></small>'); $(this).parent().append(counter); }
                        counter.text(`${length}/${maxLength}`);
                        if (length > maxLength * 0.9) counter.addClass('text-warning'); else counter.removeClass('text-warning');
                    });
                    window.selectAllFeatures = function() { $('input[name="features[]"]').prop('checked', true); if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') window.formControlBinder.showNotification('Đã chọn tất cả tính năng', 'success'); };
                    window.clearAllFeatures = function() { $('input[name="features[]"]').prop('checked', false); if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') window.formControlBinder.showNotification('Đã bỏ chọn tất cả tính năng', 'info'); };
                },
                transformData: function(data) { return data; }
            });
            registerWithId('App.detail.LicenseController');
        } else {
            class LicenseDetail extends window.BaseDetail {
                onInit() {
                    $('#customer').on('input', function() { const customer = $(this).val(); if (customer && !$('#licenseKey').val()) { const year = new Date().getFullYear(); const randomCode = Math.random().toString(36).substring(2, 5).toUpperCase(); const licenseKey = `LIC-${year}-${randomCode}`; $('#licenseKey').val(licenseKey); } });
                    $('#startDate, #expiryDate').on('change', function() { const startDate = $('#startDate').val(); const expiryDate = $('#expiryDate').val(); if (startDate && expiryDate && new Date(startDate) >= new Date(expiryDate)) { $('#expiryDate').addClass('is-invalid'); } else { $('#expiryDate').removeClass('is-invalid'); } });
                    $('#notes').on('input', function() { const length = this.value.length; const maxLength = 500; let counter = $(this).parent().find('.character-counter'); if (!counter.length) { counter = $('<small class="character-counter text-muted"></small>'); $(this).parent().append(counter); } counter.text(`${length}/${maxLength}`); if (length > maxLength * 0.9) { counter.addClass('text-warning'); } else { counter.removeClass('text-warning'); } });
                    window.selectAllFeatures = function() { $('input[name="features[]"]').prop('checked', true); if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') { window.formControlBinder.showNotification('Đã chọn tất cả tính năng', 'success'); } };
                    window.clearAllFeatures = function() { $('input[name="features[]"]').prop('checked', false); if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') { window.formControlBinder.showNotification('Đã bỏ chọn tất cả tính năng', 'info'); } };
                }
            }
            registerWithId(LicenseDetail);
        }
    });
})();
