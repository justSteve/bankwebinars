/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/jquery/jquery.validation.d.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />


module OrderRegistration {

    declare var $;

    export class PageObject {
        private discount: number;
        private wait4Emails: string;
        private orderRowID: number = 0;
        private CheckoutInProcess: boolean = false;
        private isUserLogged: boolean = false;
        private whichStep: string = "Step0";

        getOrderRowId(): number {
            return this.orderRowID;
        }

        getWhichStep(): string {
            return this.whichStep;
        }

        setWhichStep(step: string): void {
            this.whichStep = step;
        }

    };

    export class StateManager {

        private pageObject: OrderRegistration.PageObject;

        constructor(public incomingPageObject: OrderRegistration.PageObject) {
            this.pageObject = incomingPageObject;
        }

        SetCartState(): void {
            switch (this.pageObject.getWhichStep()) {
            case "Step0":
                //console.log("Step0");
                $("#connectionsCount").val(0);
                $('#collectAdditionalLocation').html('');
                $("#confirmationTab").hide();
                $("#signUpTab").hide();
                $("#contactInfoTab").hide();
                $('#AddToCart').attr({ disabled: false, value: 'SignUp' });
                $('#AddToCart1').attr({ disabled: false, value: 'SignUp' });

                break;

            case "Step1":
                //console.log("Step1");

                $("#confirmationTab").hide();
                $("#signUpTab").hide();
                $("#contactInfoTab").show();
                break;

            case "Step2":
                $("#signUpTab").hide();
                $("#signUp").hide();

                $("#confirmationTab").tab('show');
                $("#confirmationTab").addClass('active');
                $("#confirmation").addClass('active');
                $("#confirmation").show();
                //$.get("/cart/checkoutConfirm/" + orderRowID)
                //    .success(function (dataConfirm) {
                //        $('#confirmation').html(dataConfirm);
                //    })
                //.done($("#confirmation").show())
                $("#confirmationTab").trigger('click');

                //console.log("Step2");

                break;

            case "Registered":
                //console.log("state is registered");
                $("#confirmationTab a").text('Order Summary');
                $("#signUpTab a").text('Connection Info');

                $.get("/cart/checkoutConfirm/" + this.pageObject.getOrderRowId())
                    .success(function(dataConfirm) {
                        $('#confirmation').html(dataConfirm);
                    });
                $('#AddToCart').hide();
                $('#AddToCart1').hide();
                break;
            }
        }

    };
}

//function setPath() {
//    var indexOfHome = location.href.indexOf('Account');
//    var path = '';

//    if (indexOfHome > -1)
//        path = location.href.substr(0, location.href.indexOf('Account') - 1);
//    else
//        path = location.href;

//    //  IE is a rubbish browser!
//    if (path === '')
//        path = $(location).attr('href');
//    return path;
//}

//function isValidEmailAddress(emailAddress : string) {
//    var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
//    return pattern.test(emailAddress);
//};