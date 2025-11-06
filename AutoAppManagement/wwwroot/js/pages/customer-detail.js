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
        // Register ES6 class with DetailRegistry
        function registerWithId(ctorOrClassName) {
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
        }

        // ES6 CustomerDetail class
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
                    if (!c.length) { 
                        c = $('<small class="character-counter text-muted"></small>'); 
                        $(this).parent().append(c); 
                    }
                    c.text(`${len}/${max}`);
                    if (len > max * 0.9) c.addClass('text-warning'); 
                    else c.removeClass('text-warning');
                });
                
                // Try to load licenses with retry mechanism
                this.loadLicensesWithRetry();
                
                // Also load when modal shown
                const $modal = $('#customerForm').closest('.modal');
                if ($modal.length) {
                    $modal.on('shown.bs.modal', () => {
                        console.log('🔄 Modal shown - reloading licenses');
                        setTimeout(() => this.loadLicensesWithRetry(), 100);
                    });
                }
            }
            
            loadLicensesWithRetry() {
                // Simple license loading for this basic version
                console.log('🔄 Loading licenses (basic version)');
                const $sel = $('#LicenseId');
                if (!$sel.length) return;
                
                $sel.html('<option value="">Đang tải...</option>');
                
                // Try to load licenses via API
                if (typeof calGetAPIAuthen === 'function') {
                    calGetAPIAuthen('/License/GetAll', {}, function(res) {
                        const opts = ['<option value="">-- Chọn license --</option>'];
                        const list = (res && res.Data) || res || [];
                        
                        if (Array.isArray(list)) {
                            list.forEach(item => {
                                const id = item.ID || item.Id || item.id;
                                const name = item.LicenseName || item.licenseName || item.Name || item.name;
                                if (id && name) {
                                    opts.push(`<option value="${id}">${name}</option>`);
                                }
                            });
                        }
                        
                        $sel.html(opts.join(''));
                        console.log(`📋 Basic: Loaded ${opts.length - 1} licenses`);
                    }, function() {
                        $sel.html('<option value="">Lỗi khi tải license</option>');
                    });
                } else {
                    // Fallback: Add some dummy options for testing
                    $sel.html(`
                        <option value="">-- Chọn license --</option>
                        <option value="1">License Basic</option>
                        <option value="2">License Premium</option>
                        <option value="3">License Enterprise</option>
                    `);
                    console.log('📋 Basic: Loaded fallback licenses');
                }
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
                if (typeof data.Gender === 'string') {
                    const g = data.Gender.toLowerCase();
                    if (g === 'male') data.Gender = 1; 
                    else if (g === 'female') data.Gender = 2; 
                    else if (g === 'other') data.Gender = 3;
                }
                
                // Chuyển Status sang số
                if (data.Status && typeof data.Status === 'string' && !isNaN(data.Status)) {
                    data.Status = parseInt(data.Status, 10);
                }
                if (!data.Status) data.Status = 1; // default Active
                
                // Normalize date fields
                if (data.DateOfBirth) data.DateOfBirth = new Date(data.DateOfBirth).toISOString();
                if (!data.RegisterDate) data.RegisterDate = new Date().toISOString();
                
                // Normalize boolean fields
                if (data.IsLocked == null) data.IsLocked = false;
                if (data.IsAutoRenewal == null) data.IsAutoRenewal = false;
                
                // Set default LicenseId if needed
                if (!data.LicenseId) data.LicenseId = 1;
                
                return data;
            }
        }
        
        // Register ES6 class with registry
        registerWithId(CustomerDetail);
        
        // Global helper function for debugging license issues
        window.debugLicenseLoading = function() {
            console.log('🔍 Debugging license loading...');
            const $sel = $('#LicenseId');
            console.log('License select element:', $sel.length ? 'Found' : 'Not found');
            if ($sel.length) {
                console.log('Current options:', $sel.find('option').length);
                console.log('Current value:', $sel.val());
                console.log('Stored value:', $sel.data('current-value'));
            }
            
            // Try manual reload
            const form = document.getElementById('customerForm');
            if (form) {
                const ctrl = $(form).data('detailCtrl');
                if (ctrl && typeof ctrl.loadLicensesWithRetry === 'function') {
                    console.log('🔄 Manually triggering license reload...');
                    ctrl.loadLicensesWithRetry();
                }
            }
        };
    });
})();

// Extended customer form behaviors
(function () {
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
            if (FirstName && LastName && !$('#DisplayName').val()) {
                $('#DisplayName').val(`${FirstName} ${LastName}`);
            }
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
            if (length > maxLength * 0.9) {
                counter.addClass('text-warning');
            } else {
                counter.removeClass('text-warning');
            }
        });
    });
})();



