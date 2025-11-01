// Customer Detail - ES6 class extends BaseDetail
(function () {
    function ensureBaseDetailReady(callback) {
        if (window.BaseDetail && window.DetailRegistry) { callback(); return; }
        setTimeout(function () { ensureBaseDetailReady(callback); }, 50);
    }

    ensureBaseDetailReady(function () {
        // If Ext JS is available, define as Ext class; otherwise use ES6 class
        var registerWithId = function(ctorOrClassName) {
            window.DetailRegistry.register('customerForm', ctorOrClassName);
            $(document).ready(function () {
                const form = document.getElementById('customerForm');
                if (form) {
                    const ctrl = window.DetailRegistry.resolve(form);
                    $(form).data('detailCtrl', ctrl);
                    if (typeof ctrl.onInit === 'function') ctrl.onInit();
                }
            });
        };

        if (window.Ext && Ext.define) {
            if (!window.App) window.App = {}; if (!App.detail) App.detail = {};
            Ext.define('App.detail.CustomerController', {
                extend: 'App.detail.BaseDetail',
                onInit: function() {
                const sync = () => {
                    const first = $('#FirstName').val() || '';
                    const last = $('#LastName').val() || '';
                    const email = $('#Email').val() || '';
                    const full = (first + ' ' + last).trim();
                    if (full) $('#Name').val(full);
                    if (email && !$('#UserName').val()) $('#UserName').val(email.split('@')[0]);
                };
                $('#FirstName, #LastName, #Email').on('input', sync);
                $('#Notes').on('input', function () {
                    const len = this.value.length, max = 500;
                    let c = $(this).parent().find('.character-counter');
                    if (!c.length) { c = $('<small class="character-counter text-muted"></small>'); $(this).parent().append(c); }
                    c.text(`${len}/${max}`);
                    if (len > max * 0.9) c.addClass('text-warning'); else c.removeClass('text-warning');
                });
                },
                transformData: function(data) {
                if (data.Email && !data.UserName) data.UserName = String(data.Email).split('@')[0];
                if (!data.Name) {
                    const first = data.FirstName || '';
                    const last = data.LastName || '';
                    const full = `${first} ${last}`.trim();
                    if (full) data.Name = full;
                }
                if (typeof data.Gender === 'string') {
                    const g = data.Gender.toLowerCase();
                    if (g === 'male') data.Gender = 1; else if (g === 'female') data.Gender = 2; else if (g === 'other') data.Gender = 3;
                }
                if (!data.Status) data.Status = 1; // default Active
                if (data.Status && typeof data.Status === 'string' && !isNaN(data.Status)) data.Status = parseInt(data.Status, 10);
                // normalize SendWelcomeEmail
                if (data.SendWelcomeEmail !== undefined) {
                    data.SendWelcomeEmail = (data.SendWelcomeEmail === true || data.SendWelcomeEmail === 'true');
                }
                if (data.DateOfBirth) data.DateOfBirth = new Date(data.DateOfBirth).toISOString();
                if (!data.RegisterDate) data.RegisterDate = new Date().toISOString();
                if (data.IsLocked == null) data.IsLocked = false;
                if (data.IsAutoRenewal == null) data.IsAutoRenewal = false;
                if (!data.LicenseId) data.LicenseId = 1;
                return data;
                }
            });
            registerWithId('App.detail.CustomerController');
        } else {
            class CustomerDetail extends window.BaseDetail {
                onInit() {
                    console.log('CustomerDetail.onInit called');
                    const sync = () => {
                        const first = $('#FirstName').val() || '';
                        const last = $('#LastName').val() || '';
                        const email = $('#Email').val() || '';
                        const full = (first + ' ' + last).trim();
                        if (full) $('#Name').val(full);
                        if (email && !$('#UserName').val()) $('#UserName').val(email.split('@')[0]);
                    };
                    $('#FirstName, #LastName, #Email').on('input', sync);
                    $('#Notes').on('input', function () {
                        const len = this.value.length, max = 500;
                        let c = $(this).parent().find('.character-counter');
                        if (!c.length) { c = $('<small class="character-counter text-muted"></small>'); $(this).parent().append(c); }
                        c.text(`${len}/${max}`);
                        if (len > max * 0.9) c.addClass('text-warning'); else c.removeClass('text-warning');
                    });

                    this.loadLicenses();
                }
                loadLicenses() {
                    console.log('CustomerDetail.loadLicenses called');
                    const $sel = $('#LicenseId');
                    if (!$sel.length) return;
                    try {
                        calGetAPIAuthen('/api/License/GetAll', {}, function(res){
                            const list = (res && (res.Data || res.data)) || res || [];
                            const opts = ['<option value="">-- Chọn license --</option>'];
                            (list || []).forEach(function(item){
                                const id = item.Id || item.id;
                                const text = item.LicenseName || item.licenseName || item.LicenseKey || item.licenseKey || ('#' + id);
                                opts.push(`<option value="${id}">${text}</option>`);
                            });
                            $sel.html(opts.join(''));
                        }, function(){ /* silent */ });
                    } catch(e) { /* ignore */ }
                }
                transformData(data) {
                    if (data.Email && !data.UserName) data.UserName = String(data.Email).split('@')[0];
                    if (!data.Name) {
                        const first = data.FirstName || '';
                        const last = data.LastName || '';
                        const full = `${first} ${last}`.trim();
                        if (full) data.Name = full;
                    }
                    if (typeof data.Gender === 'string') {
                        const g = data.Gender.toLowerCase();
                        if (g === 'male') data.Gender = 1; else if (g === 'female') data.Gender = 2; else if (g === 'other') data.Gender = 3;
                    }
                    if (!data.Status) data.Status = 1;
                    if (data.Status && typeof data.Status === 'string' && !isNaN(data.Status)) data.Status = parseInt(data.Status, 10);
                    if (data.SendWelcomeEmail !== undefined) {
                        data.SendWelcomeEmail = (data.SendWelcomeEmail === true || data.SendWelcomeEmail === 'true');
                    }
                    if (data.DateOfBirth) data.DateOfBirth = new Date(data.DateOfBirth).toISOString();
                    if (!data.RegisterDate) data.RegisterDate = new Date().toISOString();
                    if (data.IsLocked == null) data.IsLocked = false;
                    if (data.IsAutoRenewal == null) data.IsAutoRenewal = false;
                    if (!data.LicenseId) data.LicenseId = 1;
                    return data;
                }
            }
            registerWithId(CustomerDetail);
        }
    });
})();

(function () {
    // Extended customer form behaviors (migrated from customer-extensions.js)
    $(document).ready(function() {
        function syncDerivedFields() {
            const first = $('#FirstName').val() || '';
            const last = $('#LastName').val() || '';
            const email = $('#Email').val() || '';
            const fullName = (first + ' ' + last).trim();
            if (fullName) $('#Name').val(fullName);
            if (email && !$('#UserName').val()) $('#UserName').val(email.split('@')[0]);
        }
        $('#FirstName, #LastName').on('input', syncDerivedFields);
        $('#Email').on('input', syncDerivedFields);

        $('#Email').on('input', function() {
            const Email = $(this).val();
            if (Email && !$('#UserName').val()) $('#UserName').val(Email.split('@')[0]);
        });

        $('#FirstName, #LastName').on('input', function() {
            const FirstName = $('#FirstName').val();
            const LastName = $('#LastName').val();
            if (FirstName && LastName && !$('#DisplayName').val()) $('#DisplayName').val(`${FirstName} ${LastName}`);
        });

        $('#Notes').on('input', function() {
            const length = this.value.length;
            const maxLength = 500;
            let counter = $(this).parent().find('.character-counter');
            if (!counter.length) {
                counter = $('<small class="character-counter text-muted"></small>');
                $(this).parent().append(counter);
            }
            counter.text(`${length}/${maxLength}`);
            if (length > maxLength * 0.9) counter.addClass('text-warning'); else counter.removeClass('text-warning');
        });
    });
})();
