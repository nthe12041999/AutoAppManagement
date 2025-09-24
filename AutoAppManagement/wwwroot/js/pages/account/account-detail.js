// AccountDetail class: idempotent initializer for account detail partials
class AccountDetail {
    constructor(container) {
        // container can be jQuery object or DOM element
        this.$root = (typeof $ !== 'undefined' && container && container.jquery) ? container : (container ? $(container) : $(document));
        this._inited = false;
        this.init();
    }

    init() {
        window.formControlBinder.init($('#CustomerForm'));
    }
}

// Expose class and helper initializer
window.AccountDetail = AccountDetail;
window.initAccountDetail = function(container) {
    try {
        return new AccountDetail(container);
    } catch (e) {
        console.error('initAccountDetail error', e);
    }
};