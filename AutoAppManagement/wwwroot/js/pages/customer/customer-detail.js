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
                    // Always create new controller instance to avoid stale state
                    const ctrl = window.DetailRegistry.resolve(form);
                    $(form).data('detailCtrl', ctrl);
                    
                    // Clear any existing event handlers to prevent duplicates
                    $(form).off('.customerDetail');
                    
                    if (typeof ctrl.onInit === 'function') {
                        ctrl.onInit();
                    }
                }
            });
            
            // Also handle modal events globally
            $(document).on('show.bs.modal', '.modal', function() {
                const $modal = $(this);
                const customerForm = $modal.find('#customerForm')[0];
                if (customerForm) {
                    console.log('🔄 Modal opening - preparing customer form');
                    const ctrl = $(customerForm).data('detailCtrl');
                    if (ctrl && typeof ctrl.loadLicenses === 'function') {
                        // Clear any cached license data
                        $('#LicenseId').removeData('current-value');
                        console.log('🔄 Clearing license cache for fresh load');
                    }
                }
            });
            
            $(document).on('shown.bs.modal', '.modal', function() {
                const $modal = $(this);
                const customerForm = $modal.find('#customerForm')[0];
                if (customerForm) {
                    console.log('🔄 Modal shown - checking for view mode and license reload');
                    
                    // Check for view mode first
                    const modalTitle = $modal.find('.modal-title').text().toLowerCase();
                    console.log('📋 Customer modal title:', modalTitle);
                    
                    if (modalTitle.includes('xem chi tiết') || modalTitle.includes('xem') || modalTitle.includes('view') || modalTitle.includes('chi tiết')) {
                        console.log('🔍 Customer view modal detected, setting view mode');
                        setTimeout(() => {
                            window.setFormViewMode($(customerForm), modalTitle);
                        }, 200);
                        return; // Don't load licenses in view mode
                    }
                    
                    // Only load licenses if not in view mode
                    const ctrl = $(customerForm).data('detailCtrl');
                    if (ctrl && typeof ctrl.loadLicenses === 'function') {
                        // Force reload licenses every time modal is shown
                        setTimeout(() => {
                            ctrl.loadLicenses();
                        }, 100);
                    }
                }
            });
        }

        // ES6 CustomerDetail class
        class CustomerDetail extends window.BaseDetail {
            onInit() {
                console.log('CustomerDetail (ES6) onInit called');
                
                // First check for view mode
                const $form = $(this.form);
                const $modal = $form.closest('.modal');
                if ($modal.length) {
                    const modalTitle = $modal.find('.modal-title').text().toLowerCase();
                    console.log('🔍 CustomerDetail checking modal title:', modalTitle);
                    
                    if (modalTitle.includes('xem chi tiết') || modalTitle.includes('xem') || modalTitle.includes('view') || modalTitle.includes('chi tiết')) {
                        console.log('🔒 CustomerDetail detected view mode, setting up view mode');
                        setTimeout(() => {
                            this.setViewMode(modalTitle);
                        }, 100);
                        return; // Don't set up edit mode functionality
                    }
                }
                
                // Only set up edit mode functionality if not in view mode
                console.log('📝 CustomerDetail setting up edit mode');
                
                const sync = () => {
                    const first = $('#FirstName').val() || '';
                    const last = $('#LastName').val() || '';
                    const email = $('#Email').val() || '';
                    const full = (first + ' ' + last).trim();
                    if (full) $('#Name').val(full);
                    if (email && !$('#UserName').val()) $('#UserName').val(email.split('@')[0]);
                };
                $('#FirstName, #LastName, #Email').on('input.customerDetail', sync);
                $('#Notes').on('input.customerDetail', function () {
                    const len = this.value.length, max = 500;
                    let c = $(this).parent().find('.character-counter');
                    if (!c.length) { c = $('<small class="character-counter text-muted"></small>'); $(this).parent().append(c); }
                    c.text(`${len}/${max}`);
                    if (len > max * 0.9) c.addClass('text-warning'); else c.removeClass('text-warning');
                });

                console.log('CustomerDetail (ES6) calling loadLicenses on init');
                
                // Always load licenses on init (only in edit mode)
                this.loadLicenses();
            }
            loadLicenses() {
                console.log('CustomerDetail (ES6) loadLicenses invoked');
                const $sel = $('#LicenseId');
                if (!$sel.length) {
                    console.warn('⚠️ LicenseId element not found');
                    return;
                }
                
                // Get current value to restore after loading
                const currentValue = $sel.val() || $sel.data('current-value');
                console.log('🔍 Current license value to restore:', currentValue);
                
                // Always show loading state
                $sel.html('<option value="">Đang tải...</option>');
                
                // Clear any existing request to avoid race conditions
                if (this.licenseLoadingRequest) {
                    console.log('🚫 Aborting previous license request');
                    this.licenseLoadingRequest.abort();
                }
                
                // Add retry mechanism with timeout
                const loadWithRetry = (retryCount = 0) => {
                    const maxRetries = 3;
                    
                    console.log(`🔄 Loading licenses (attempt ${retryCount + 1}/${maxRetries + 1})`);
                    
                    this.licenseLoadingRequest = $.ajax({
                        url: '/License/GetAll',
                        type: 'GET',
                        dataType: 'json',
                        headers: {
                            'Authorization': localStorage.getItem('authToken') ? `Bearer ${localStorage.getItem('authToken')}` : undefined
                        },
                        timeout: 10000 // 10 second timeout
                    });
                    
                    this.licenseLoadingRequest.done((res) => {
                        console.log('🔍 License API response:', res);
                        
                        // Handle different response formats
                        let list = [];
                        if (res && res.success && res.data) {
                            list = res.data;
                        } else if (res && res.Data) {
                            list = res.Data;
                        } else if (Array.isArray(res)) {
                            list = res;
                        } else {
                            console.warn('⚠️ Unexpected license response format:', res);
                        }
                        
                        const opts = ['<option value="">-- Chọn license --</option>'];
                        
                        if (Array.isArray(list) && list.length > 0) {
                            list.forEach(function(item){
                                const id = item.ID || item.Id || item.id;
                                const name = item.LicenseName || item.licenseName || item.Name || item.name;
                                const key = item.LicenseKey || item.licenseKey || '';
                                const status = item.Status || item.status;
                                
                                // Chỉ hiển thị license đang Active (status = 1 hoặc 'active')
                                if ((status === 1 || status === 'active') && (name || key)) {
                                    const displayName = name || key || `License ${id}`;
                                    opts.push(`<option value="${id}">${displayName}</option>`);
                                }
                            });
                        } else {
                            console.warn('⚠️ No licenses found or invalid data format');
                        }
                        
                        $sel.html(opts.join(''));
                        
                        // Restore value if exists - with delay to ensure DOM is updated
                        if (currentValue) {
                            setTimeout(() => {
                                $sel.val(currentValue);
                                if ($sel.val() === currentValue) {
                                    $sel.trigger('change');
                                    console.log('✅ License value restored successfully:', currentValue);
                                } else {
                                    console.warn('⚠️ Failed to restore license value:', currentValue);
                                    // Try to find license by value
                                    const option = $sel.find(`option[value="${currentValue}"]`);
                                    if (option.length === 0) {
                                        console.warn('⚠️ License option not found for value:', currentValue);
                                    }
                                }
                            }, 50);
                        }
                        
                        console.log(`📋 Loaded ${opts.length - 1} licenses successfully`);
                        this.licenseLoadingRequest = null;
                    }).fail((xhr, status, error) => {
                        console.error('❌ License API error:', { xhr, status, error, retryCount });
                        
                        if (status === 'abort') {
                            console.log('🚫 Request was aborted');
                            return;
                        }
                        
                        if (retryCount < maxRetries) {
                            console.log(`🔄 Retrying license load (${retryCount + 1}/${maxRetries})`);
                            setTimeout(() => loadWithRetry(retryCount + 1), 1000 * (retryCount + 1));
                            return;
                        }
                        
                        $sel.html('<option value="">Không thể tải danh sách license</option>');
                        
                        // Try alternative endpoint if main fails after all retries
                        if (xhr.status === 404) {
                            console.log('🔄 Trying alternative License API...');
                            $.ajax({
                                url: '/License/GetPaging?pageSize=1000',
                                type: 'GET',
                                dataType: 'json',
                                headers: {
                                    'Authorization': localStorage.getItem('authToken') ? `Bearer ${localStorage.getItem('authToken')}` : undefined
                                }
                            }).done((res) => {
                                console.log('🔍 Alternative License API response:', res);
                                
                                let list = [];
                                if (res && res.success && res.data && res.data.Items) {
                                    list = res.data.Items;
                                } else if (res && res.Data && res.Data.Items) {
                                    list = res.Data.Items;
                                }
                                
                                const opts = ['<option value="">-- Chọn license --</option>'];
                                
                                if (Array.isArray(list) && list.length > 0) {
                                    list.forEach(function(item){
                                        const id = item.ID || item.Id || item.id;
                                        const name = item.LicenseName || item.licenseName || item.Name || item.name;
                                        const key = item.LicenseKey || item.licenseKey || '';
                                        const status = item.Status || item.status;
                                        
                                        if ((status === 1 || status === 'active') && (name || key)) {
                                            const displayName = name || key || `License ${id}`;
                                            opts.push(`<option value="${id}">${displayName}</option>`);
                                        }
                                    });
                                }
                                
                                $sel.html(opts.join(''));
                                
                                if (currentValue) {
                                    setTimeout(() => {
                                        $sel.val(currentValue).trigger('change');
                                    }, 50);
                                }
                            }).fail(() => {
                                $sel.html('<option value="">Lỗi khi tải license</option>');
                            });
                        }
                        
                        this.licenseLoadingRequest = null;
                    });
                };
                
                // Start loading with retry
                loadWithRetry();
            }
            loadData(data) {
                console.log('CustomerDetail (ES6) loadData called with', data);
                console.log('🎯 CustomerDetail.loadData called with:', data);
                
                // Store LicenseId to restore after licenses are loaded
                if (data && data.LicenseId) {
                    console.log('💾 Storing LicenseId for restoration:', data.LicenseId);
                    $('#LicenseId').data('current-value', data.LicenseId);
                }
                
                // Always reload licenses to ensure fresh data
                console.log('🔄 Reloading licenses from loadData');
                this.loadLicenses();
                
                // Also try to set value after a delay in case licenses are already loaded
                if (data && data.LicenseId) {
                    setTimeout(() => {
                        const $sel = $('#LicenseId');
                        if ($sel.find('option').length > 1) { // More than just the default option
                            console.log('🔄 Attempting immediate license restoration:', data.LicenseId);
                            $sel.val(data.LicenseId);
                            if ($sel.val() === data.LicenseId.toString()) {
                                $sel.trigger('change');
                                console.log('✅ Immediate license restoration successful');
                            } else {
                                console.log('⚠️ Immediate restoration failed, will retry after load');
                            }
                        }
                    }, 200);
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

        // Register ES6 class with registry
        registerWithId(CustomerDetail);
        
        // Global function to force reload licenses (for debugging/manual trigger)
        window.forceReloadLicenses = function(clearCache = true) {
            console.log('🔄 Manual license reload triggered');
            
            if (clearCache) {
                $('#LicenseId').removeData('current-value');
                console.log('🧹 Cleared license cache');
            }
            
            const form = document.getElementById('customerForm');
            if (form) {
                const ctrl = $(form).data('detailCtrl');
                if (ctrl && typeof ctrl.loadLicenses === 'function') {
                    // Abort any existing request
                    if (ctrl.licenseLoadingRequest) {
                        ctrl.licenseLoadingRequest.abort();
                        ctrl.licenseLoadingRequest = null;
                    }
                    ctrl.loadLicenses();
                } else {
                    console.warn('⚠️ Controller not found or loadLicenses method missing');
                }
            } else {
                console.warn('⚠️ customerForm not found');
            }
        };
        
        // Enhanced debug function
        window.debugLicenseState = function() {
            console.log('🔍 ==> License Debug Info <==');
            const $sel = $('#LicenseId');
            console.log('License select found:', $sel.length > 0);
            if ($sel.length) {
                console.log('Options count:', $sel.find('option').length);
                console.log('Current value:', $sel.val());
                console.log('Cached value:', $sel.data('current-value'));
                console.log('Options:', $sel.find('option').map((i, opt) => `${opt.value}: ${opt.text}`).get());
            }
            
            const form = document.getElementById('customerForm');
            if (form) {
                const ctrl = $(form).data('detailCtrl');
                console.log('Controller found:', !!ctrl);
                console.log('LoadLicenses method:', typeof ctrl?.loadLicenses);
                console.log('Active request:', !!ctrl?.licenseLoadingRequest);
            }
            console.log('==> End Debug Info <==');
        };
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
