

$(function () {

	$("#showLoginBox").dialog({
		autoOpen: false,
		height: 300,
		width: 350,
		modal: true
	});

	$("#login-user")
			.button()
			.click(function () {
				$("#showLoginBox").dialog("open");

			});

	// define function that opens the overlay

	var form = $("#loginForm");
	form.submit(function () {

		var data = form.serialize();
		$.post(form.attr("action"), data, function (result, status) {
			if (result.Success) {
				$("#showLoginBox").dialog("close");

				//Show user name
				$.ajax({
					url: "/Account/ShowLoginStatus",
					cache: false,
					success: function (html) {
						$(".showLoggedUser").html(html);
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

	$("#showLoginBox .lostPasswordButton").click(function () {
		$("#showLoginBox .container").hide();
		$("#showLoginBox .lostPasswordContainer").show();
		return false;
	});

	$("#showLoginBox .loginButton").click(function () {
		
		$("#showLoginBox .container").hide();
		$("#showLoginBox .loginContainer").show();
		return false;
	});

	var lostPasswordForm = $("#lostPasswordForm");
	lostPasswordForm.submit(function () {
		var data = lostPasswordForm.serialize();
		$.post(lostPasswordForm.attr("action"), data, function (result, status) {
			if (result.Success) {
				$("#showLoginBox .container").hide();
				$("#showLoginBox .passwordSentContainer").show();
			} else {
				$("#showLoginBox .container").hide();
				$("#showLoginBox .noAccountFoundContainer").show();
			}
		}, "json");
		return false;
	});
});
