// Customer Detail - ES6 class extends BaseDetail
(function () {
    function ensureBaseDetailReady(callback) {
        if (window.BaseDetail && window.DetailRegistry) { 
            callback(); 
            return; 
        }
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
                    if (typeof ctrl.onInit === 'function') {
                        ctrl.onInit();
                    }
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
                
                // Load danh sách License
                this.loadLicenses();
                },
                loadLicenses: function() {
                    const $sel = $('#LicenseId');
                    if (!$sel.length) return;
                    
                    // Get current value to restore after loading
                    const currentValue = $sel.val() || $sel.data('current-value');
                    
                    calGetAPIAuthen('/License/GetAll', {}, function(res){
                        const list = (res && (res.Data || res.data)) || res || [];
                        const opts = ['<option value="">-- Chọn license --</option>'];
                        (list || []).forEach(function(item){
                            const id = item.ID || item.Id || item.id;
                            const name = item.LicenseName || item.licenseName || '';
                            const status = item.Status || item.status;
                            
                            // Chỉ hiển thị license đang Active
                            if (status === 1 && name) {
                                opts.push(`<option value="${id}">${name}</option>`);
                            }
                        });
                        $sel.html(opts.join(''));
                        
                        // Restore value if exists
                        if (currentValue) {
                            $sel.val(currentValue).trigger('change');
                            console.log('✅ License value restored:', currentValue);
                        }
                    }, function(){ 
                        $sel.html('<option value="">Không thể tải danh sách license</option>');
                    });
                },
                loadData: function(data) {
                    console.log('🎯 CustomerDetail.loadData called with:', data);
                    
                    // Store LicenseId to restore after licenses are loaded
                    if (data.LicenseId) {
                        $('#LicenseId').data('current-value', data.LicenseId);
                    }
                    
                    // Reload licenses to ensure options are available
                    this.loadLicenses();
                },
                transformData: function(data) {
                if (data.Email && !data.UserName) data.UserName = String(data.Email).split('@')[0];
                if (!data.Name) {
                    const first = data.FirstName || '';
                    const last = data.LastName || '';
                    const full = `${first} ${last}`.trim();
                    if (full) data.Name = full;
                }
                
                // Chuyển Gender sang số
                if (data.Gender !== undefined && data.Gender !== null && data.Gender !== '') {
                    if (typeof data.Gender === 'string') {
                        const g = data.Gender.toLowerCase();
                        if (g === 'male') data.Gender = 1;
                        else if (g === 'female') data.Gender = 2;
                        else if (g === 'other') data.Gender = 3;
                        else if (!isNaN(data.Gender)) data.Gender = parseInt(data.Gender, 10);
                    }
                }
                
                // Chuyển Status sang số
                if (data.Status !== undefined && data.Status !== null && data.Status !== '') {
                    if (typeof data.Status === 'string' && !isNaN(data.Status)) {
                        data.Status = parseInt(data.Status, 10);
                    }
                }
                if (!data.Status) data.Status = 1; // default Active
                
                // Chuyển LicenseId sang số
                if (data.LicenseId !== undefined && data.LicenseId !== null && data.LicenseId !== '') {
                    if (typeof data.LicenseId === 'string' && !isNaN(data.LicenseId)) {
                        data.LicenseId = parseInt(data.LicenseId, 10);
                    }
                }
                // Không set default LicenseId nữa - để user tự chọn
                
                // normalize SendWelcomeEmail
                if (data.SendWelcomeEmail !== undefined) {
                    data.SendWelcomeEmail = (data.SendWelcomeEmail === true || data.SendWelcomeEmail === 'true');
                }
                if (data.DateOfBirth) data.DateOfBirth = new Date(data.DateOfBirth).toISOString();
                if (!data.RegisterDate) data.RegisterDate = new Date().toISOString();
                if (data.IsLocked == null) data.IsLocked = false;
                if (data.IsAutoRenewal == null) data.IsAutoRenewal = false;
                
                return data;
                }
            });
            registerWithId('App.detail.CustomerController');
        } else {
            class CustomerDetail extends window.BaseDetail {
                onInit() {
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
                    const $sel = $('#LicenseId');
                    if (!$sel.length) return;
                    
                    // Get current value to restore after loading
                    const currentValue = $sel.val() || $sel.data('current-value');
                    
                    calGetAPIAuthen('/License/GetAll', {}, function(res){
                        const list = (res && (res.Data || res.data)) || res || [];
                        const opts = ['<option value="">-- Chọn license --</option>'];
                        (list || []).forEach(function(item){
                            const id = item.ID || item.Id || item.id;
                            const name = item.LicenseName || item.licenseName || '';
                            const status = item.Status || item.status;
                            
                            // Chỉ hiển thị license đang Active
                            if (status === 1 && name) {
                                opts.push(`<option value="${id}">${name}</option>`);
                            }
                        });
                        $sel.html(opts.join(''));
                        
                        // Restore value if exists
                        if (currentValue) {
                            $sel.val(currentValue).trigger('change');
                            console.log('✅ License value restored:', currentValue);
                        }
                    }, function(){ 
                        $sel.html('<option value="">Không thể tải danh sách license</option>');
                    });
                }
                loadData(data) {
                    console.log('🎯 CustomerDetail.loadData called with:', data);
                    
                    // Store LicenseId to restore after licenses are loaded
                    if (data.LicenseId) {
                        $('#LicenseId').data('current-value', data.LicenseId);
                    }
                    
                    // Reload licenses to ensure options are available
                    this.loadLicenses();
                }
                transformData(data) {
                    if (data.Email && !data.UserName) data.UserName = String(data.Email).split('@')[0];
                    if (!data.Name) {
                        const first = data.FirstName || '';
                        const last = data.LastName || '';
                        const full = `${first} ${last}`.trim();
                        if (full) data.Name = full;
                    }
                    
                    // Chuyển Gender sang số
                    if (data.Gender !== undefined && data.Gender !== null && data.Gender !== '') {
                        if (typeof data.Gender === 'string') {
                            const g = data.Gender.toLowerCase();
                            if (g === 'male') data.Gender = 1;
                            else if (g === 'female') data.Gender = 2;
                            else if (g === 'other') data.Gender = 3;
                            else if (!isNaN(data.Gender)) data.Gender = parseInt(data.Gender, 10);
                        }
                    }
                    
                    // Chuyển Status sang số
                    if (data.Status !== undefined && data.Status !== null && data.Status !== '') {
                        if (typeof data.Status === 'string' && !isNaN(data.Status)) {
                            data.Status = parseInt(data.Status, 10);
                        }
                    }
                    if (!data.Status) data.Status = 1; // default Active
                    
                    // Chuyển LicenseId sang số
                    if (data.LicenseId !== undefined && data.LicenseId !== null && data.LicenseId !== '') {
                        if (typeof data.LicenseId === 'string' && !isNaN(data.LicenseId)) {
                            data.LicenseId = parseInt(data.LicenseId, 10);
                        }
                    }
                    if (!data.LicenseId) data.LicenseId = 1;
                    
                    // normalize SendWelcomeEmail
                    if (data.SendWelcomeEmail !== undefined) {
                        data.SendWelcomeEmail = (data.SendWelcomeEmail === true || data.SendWelcomeEmail === 'true');
                    }
                    if (data.DateOfBirth) data.DateOfBirth = new Date(data.DateOfBirth).toISOString();
                    if (!data.RegisterDate) data.RegisterDate = new Date().toISOString();
                    if (data.IsLocked == null) data.IsLocked = false;
                    if (data.IsAutoRenewal == null) data.IsAutoRenewal = false;
                    
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
