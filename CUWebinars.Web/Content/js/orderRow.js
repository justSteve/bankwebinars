

$(function () {

	$("#showOrderRow").dialog({
		autoOpen: false,
		height: 300,
		width: 350,
		modal: true
	});

	$("#editOrderRow")
			.button()
			.click(function () {
				$("#showOrderRow").dialog("open");

			});

	// define function that opens the overlay

	var form = $("#loginForm");
	form.submit(function () {

		var data = form.serialize();
		$.post(form.attr("action"), data, function (result, status) {
			if (result.Success) {
				$("#showOrderRow").dialog("close");

				//
				$.ajax({
					url: "/Account/ShowLoginStatus",
					cache: false,
					success: function (html) {
						$(".showOrderDetails").html(html);
					}
				});

				//Show user menu if navigation block is present
				if ($("#main_navigation").length > 0) {
					$.ajax({
						url: "/Account/ShowUserMenu",
						cache: false,
						success: function (html) {
							$("#main_navigation").append(html);
						}
					});
					if (result.UserType == "Admin") {

						window.location = "\Admin";
					}

					if (result.UserType == "Affiliate") {
						//alert("is affiliate");                        
						window.location = "\Affiliate";
					}
				}

			} else {
				$('.loginErrors').html(result.ErrorMessage);
			}
		}, "json");
		return false;
	});

	$("#showOrderRow .lostPasswordButton").click(function () {
		$("#showOrderRow .container").hide();
		$("#showOrderRow .lostPasswordContainer").show();
		return false;
	});

	$("#showOrderRow .loginButton").click(function () {
		
		$("#showOrderRow .container").hide();
		$("#showOrderRow .loginContainer").show();
		return false;
	});

	var lostPasswordForm = $("#lostPasswordForm");
	lostPasswordForm.submit(function () {
		var data = lostPasswordForm.serialize();
		$.post(lostPasswordForm.attr("action"), data, function (result, status) {
			if (result.Success) {
				$("#showOrderRow .container").hide();
				$("#showOrderRow .passwordSentContainer").show();
			} else {
				$("#showOrderRow .container").hide();
				$("#showOrderRow .noAccountFoundContainer").show();
			}
		}, "json");
		return false;
	});
});
