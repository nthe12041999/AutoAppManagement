function getCookie(name) {
	// Tách các cookie thành từng phần tử trong một mảng
	var cookies = document.cookie.split(";");

	// Duyệt qua mảng cookie và tìm cookie có tên cần lấy
	for (var i = 0; i < cookies.length; i++) {
		var cookie = cookies[i].trim();
		// Kiểm tra xem cookie có bắt đầu bằng tên cần tìm không
		if (cookie.indexOf(name + "=") === 0) {
			// Trả về giá trị của cookie
			return cookie.substring(name.length + 1, cookie.length);
		}
	}
	// Trả về null nếu không tìm thấy cookie với tên đã cho
	return null;
}

function callAPIAjax(url, data, type, hasAuthen, successMethod, errorMethod, customResponseMethod) {
	objectAjax = {
		url: url,
		type: type,
		dataType: 'json',
		success: function (response) {

			if (customResponseMethod && typeof customResponseMethod == 'function') {
				customResponseMethod(response);
			}
			else {
				// Xử lý dữ liệu trả về từ máy chủ
				if (response && response.isSuccess && successMethod && typeof successMethod == 'function') {
					successMethod(response);
				}
				else if (errorMethod && typeof errorMethod == 'function') {
					errorMethod(response);
				}
			}
		},
		error: function (xhr, status, error) {
			// Xử lý lỗi nếu có
			console.log('AJAX Error:', xhr, status, error);
			if (errorMethod && typeof errorMethod == 'function') {
				errorMethod({
					message: xhr.responseText || error || 'Có lỗi xảy ra khi gọi API',
					status: xhr.status,
					statusText: xhr.statusText
				});
			}
		}
	};

	// Thiết lập Content-Type cho POST request
	if (type === 'POST') {
		objectAjax.contentType = 'application/json; charset=utf-8';
		objectAjax.data = JSON.stringify(data);
	} else {
		objectAjax.data = data;
	}

	$.ajax(objectAjax);
}

function callPostAPIAuthen(url, data, successMethod, errorMethod, customResponseMethod) {
	callAPIAjax(url, data, 'POST', true, successMethod, errorMethod, customResponseMethod)
}

function calGetAPIAuthen(url, data, successMethod, errorMethod, customResponseMethod) {
	callAPIAjax(url, data, 'GET', true, successMethod, errorMethod, customResponseMethod)
}

function callPostAPIPublic(url, data, successMethod, errorMethod, customResponseMethod) {
	callAPIAjax(url, data, 'POST', false, successMethod, errorMethod, customResponseMethod)
}

function callGetAPIPublic(url, data, successMethod, errorMethod, customResponseMethod) {
	callAPIAjax(url, data, 'GET', false, successMethod, errorMethod, customResponseMethod)
}