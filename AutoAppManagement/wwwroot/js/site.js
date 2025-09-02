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
