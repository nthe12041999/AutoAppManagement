/**
 * Role Detail (Ext/ES6 controller)
 */

(function () {
    function ensureBaseDetailReady(cb) {
        if (window.BaseDetail && window.DetailRegistry) { cb(); return; }
        setTimeout(function () { ensureBaseDetailReady(cb); }, 50);
    }

    ensureBaseDetailReady(function () {
        var registerWithId = function(ctorOrName) {
            window.DetailRegistry.register('roleForm', ctorOrName);
            $(document).ready(function () {
                const form = document.getElementById('roleForm');
                if (form) {
                    const ctrl = window.DetailRegistry.resolve(form);
                    $(form).data('detailCtrl', ctrl);
                    if (typeof ctrl.onInit === 'function') ctrl.onInit();
                }
            });
        };

        if (window.Ext && Ext.define) {
            if (!window.App) window.App = {}; if (!App.detail) App.detail = {};
            Ext.define('App.detail.RoleController', {
                extend: 'App.detail.BaseDetail',
                onInit: function() {
                    $('#name').on('input', function() {
                        const name = $(this).val();
                        if (name && !$('#code').val()) {
                            const code = name.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '').replace(/[^a-z0-9]/g, '_').substring(0, 50);
                            $('#code').val(code);
                        }
                    });
                    $('#code').on('input', function() { const v = $(this).val().toUpperCase(); if (v !== $(this).val()) $(this).val(v); });
                    $('#description').on('input', function() {
                        const length = this.value.length, maxLength = 500;
                        let counter = $(this).parent().find('.character-counter');
                        if (!counter.length) { counter = $('<small class="character-counter text-muted"></small>'); $(this).parent().append(counter); }
                        counter.text(`${length}/${maxLength}`);
                        if (length > maxLength * 0.9) counter.addClass('text-warning'); else counter.removeClass('text-warning');
                    });
                    window.selectAllPermissions = function() { $('input[name="permissions[]"]').prop('checked', true); if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') window.formControlBinder.showNotification('Đã chọn tất cả quyền hạn', 'success'); };
                    window.clearAllPermissions = function() { $('input[name="permissions[]"]').prop('checked', false); if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') window.formControlBinder.showNotification('Đã bỏ chọn tất cả quyền hạn', 'info'); };
                },
                transformData: function(data) { return data; }
            });
            registerWithId('App.detail.RoleController');
        } else {
            class RoleDetail extends window.BaseDetail {
                onInit() {
                    $('#name').on('input', function() {
                        const name = $(this).val();
                        if (name && !$('#code').val()) {
                            const code = name.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '').replace(/[^a-z0-9]/g, '_').substring(0, 50);
                            $('#code').val(code);
                        }
                    });
                    $('#code').on('input', function() { const v = $(this).val().toUpperCase(); if (v !== $(this).val()) $(this).val(v); });
                    $('#description').on('input', function() { const length = this.value.length, maxLength = 500; let counter = $(this).parent().find('.character-counter'); if (!counter.length) { counter = $('<small class="character-counter text-muted"></small>'); $(this).parent().append(counter); } counter.text(`${length}/${maxLength}`); if (length > maxLength * 0.9) counter.addClass('text-warning'); else counter.removeClass('text-warning'); });
                    window.selectAllPermissions = function() { $('input[name="permissions[]"]').prop('checked', true); if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') window.formControlBinder.showNotification('Đã chọn tất cả quyền hạn', 'success'); };
                    window.clearAllPermissions = function() { $('input[name="permissions[]"]').prop('checked', false); if (window.formControlBinder && typeof window.formControlBinder.showNotification === 'function') window.formControlBinder.showNotification('Đã bỏ chọn tất cả quyền hạn', 'info'); };
                }
            }
            registerWithId(RoleDetail);
        }
    });
})();
