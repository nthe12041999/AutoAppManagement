// App utilities: notification and API wrappers (Promise-based)
(function(){
    if (!window.App) window.App = {};
    if (!App.util) App.util = {};

    App.util.notify = function(message, type){
        var t = type || 'info';
        if (typeof Swal !== 'undefined') {
            var icon = t === 'error' ? 'error' : t === 'success' ? 'success' : t === 'warning' ? 'warning' : 'info';
            Swal.fire({ title: '', text: message, icon: icon, timer: 2500, showConfirmButton: false });
            return;
        }
        // Fallback Bootstrap alert
        var cls = t === 'error' ? 'alert-danger' : t === 'success' ? 'alert-success' : t === 'warning' ? 'alert-warning' : 'alert-info';
        var el = document.createElement('div');
        el.className = 'alert ' + cls + ' alert-dismissible fade show position-fixed';
        el.style.cssText = 'top:20px;right:20px;z-index:9999;min-width:320px;max-width:480px;';
        el.innerHTML = '<button type="button" class="btn-close" data-bs-dismiss="alert"></button><div>' + (message || '') + '</div>';
        document.body.appendChild(el);
        setTimeout(function(){ if (el && el.parentNode) el.parentNode.removeChild(el); }, 3000);
    };

    App.util.api = {
        get: function(url, params){
            return new Promise(function(resolve, reject){
                try {
                    if (typeof calGetAPIAuthen === 'function') {
                        calGetAPIAuthen(url, params || {}, resolve, reject);
                    } else if (window.$ && $.ajax) {
                        $.ajax({ url: url, type: 'GET', dataType: 'json', data: params || {} })
                            .done(resolve).fail(function(xhr){ reject({ message: xhr.responseText || 'Request failed', status: xhr.status }); });
                    } else {
                        var qs = params ? ('?' + new URLSearchParams(params).toString()) : '';
                        fetch(url + qs).then(function(r){ return r.json(); }).then(resolve).catch(reject);
                    }
                } catch(e) { reject(e); }
            });
        },
        post: function(url, body){
            return new Promise(function(resolve, reject){
                try {
                    if (typeof callPostAPIAuthen === 'function') {
                        callPostAPIAuthen(url, body || {}, resolve, reject);
                    } else if (window.$ && $.ajax) {
                        $.ajax({ url: url, type: 'POST', dataType: 'json', contentType: 'application/json; charset=utf-8', data: JSON.stringify(body || {}) })
                            .done(resolve).fail(function(xhr){ reject({ message: xhr.responseText || 'Request failed', status: xhr.status }); });
                    } else {
                        fetch(url, { method: 'POST', headers: { 'Content-Type':'application/json' }, body: JSON.stringify(body || {}) })
                            .then(function(r){ return r.json(); }).then(resolve).catch(reject);
                    }
                } catch(e) { reject(e); }
            });
        }
    };
})();
















