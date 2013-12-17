/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="../typings/bootstrap/bootstrap.d.ts"/>
//http://visualstudiomagazine.com/articles/2013/06/01/test-driven-development-with-typescript.aspx
// Module
var cuwebinars;
(function (cuwebinars) {
    // Class
    var wUser = (function () {
        // Constructor
        function wUser(IdUser, FirstName, LastName, Institution, uType, Email, Shipping, Shipping2, Billing, Billing2, Zip, State, City, Phone) {
            this.IdUser = IdUser;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Institution = Institution;
            this.uType = uType;
            this.Email = Email;
            this.Shipping = Shipping;
            this.Shipping2 = Shipping2;
            this.Billing = Billing;
            this.Billing2 = Billing2;
            this.Zip = Zip;
            this.State = State;
            this.City = City;
            this.Phone = Phone;
        }
        wUser.prototype.getIdUser = function () {
            return this.IdUser;
        };
        wUser.prototype.setIdUser = function (IdUser) {
            this.IdUser = IdUser;
        };
        wUser.prototype.getFirstName = function () {
            return this.FirstName;
        };
        wUser.prototype.setFirstName = function (FirstName) {
            this.FirstName = FirstName;
        };
        wUser.prototype.getLastName = function () {
            return this.LastName;
        };
        wUser.prototype.setLastName = function (LastName) {
            this.LastName = LastName;
        };
        wUser.prototype.getuType = function () {
            return this.uType;
        };
        wUser.prototype.setuType = function (uType) {
            this.uType = uType;
        };
        wUser.prototype.getEmail = function () {
            return this.Email;
        };
        wUser.prototype.setEmail = function (Email) {
            this.Email = Email;
        };
        wUser.prototype.getShipping2 = function () {
            return this.Shipping2;
        };
        wUser.prototype.setShipping2 = function (Shipping2) {
            this.Shipping2 = Shipping2;
        };
        wUser.prototype.getBilling2 = function () {
            return this.Billing2;
        };
        wUser.prototype.setBilling2 = function (Billing2) {
            this.Billing2 = Billing2;
        };
        wUser.prototype.getZip = function () {
            return this.Zip;
        };
        wUser.prototype.setZip = function (Zip) {
            this.Zip = Zip;
        };
        wUser.prototype.getState = function () {
            return this.State;
        };
        wUser.prototype.setState = function (State) {
            this.State = State;
        };
        wUser.prototype.getCity = function () {
            return this.City;
        };
        wUser.prototype.setCity = function (City) {
            this.City = City;
        };
        wUser.prototype.getPhone = function () {
            return this.Phone;
        };
        wUser.prototype.setPhone = function (Phone) {
            this.Phone = Phone;
        };
        wUser.prototype.getShipping = function () {
            return this.Shipping;
        };
        wUser.prototype.setShipping = function (Shipping) {
            this.Shipping = Shipping;
        };
        wUser.prototype.getBilling = function () {
            return this.Billing;
        };
        wUser.prototype.setBilling = function (Billing) {
            this.Billing = Billing;
        };
        wUser.prototype.getInstitution = function () {
            return this.Institution;
        };
        wUser.prototype.setInstitution = function (Institution) {
            this.Institution = Institution;
        };
        return wUser;
    })();
    cuwebinars.wUser = wUser;

    // Class
    var Order = (function () {
        // Constructor
        function Order(IdOrder, IdAffiliate, IdUser, FirstName, LastName, Institution, Origin, Email, Shipping, Shipping2, Billing, Billing2, Zip, State, City, Phone) {
            this.IdOrder = IdOrder;
            this.IdAffiliate = IdAffiliate;
            this.IdUser = IdUser;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Institution = Institution;
            this.Origin = Origin;
            this.Email = Email;
            this.Shipping = Shipping;
            this.Shipping2 = Shipping2;
            this.Billing = Billing;
            this.Billing2 = Billing2;
            this.Zip = Zip;
            this.State = State;
            this.City = City;
            this.Phone = Phone;
        }
        Order.prototype.getIdOrder = function () {
            return this.IdOrder;
        };
        Order.prototype.setIdOrder = function (IdOrder) {
            this.IdOrder = IdOrder;
        };
        Order.prototype.getIdAffiliate = function () {
            return this.IdAffiliate;
        };
        Order.prototype.setIdAffiliate = function (IdAffiliate) {
            this.IdAffiliate = IdAffiliate;
        };
        Order.prototype.getIdUser = function () {
            return this.IdUser;
        };
        Order.prototype.setIdUser = function (IdUser) {
            this.IdUser = IdUser;
        };
        Order.prototype.getFirstName = function () {
            return this.FirstName;
        };
        Order.prototype.getLastName = function () {
            return this.LastName;
        };
        Order.prototype.setFirstName = function (FirstName) {
            this.FirstName = FirstName;
        };
        Order.prototype.setLastName = function (LastName) {
            this.LastName = LastName;
        };
        Order.prototype.getOrigin = function () {
            return this.Origin;
        };
        Order.prototype.setOrigin = function (Origin) {
            this.Origin = Origin;
        };
        Order.prototype.getEmail = function () {
            return this.Email;
        };
        Order.prototype.setEmail = function (Email) {
            this.Email = Email;
        };
        Order.prototype.getShipping2 = function () {
            return this.Shipping2;
        };
        Order.prototype.setShipping2 = function (Shipping2) {
            this.Shipping2 = Shipping2;
        };
        Order.prototype.getBilling2 = function () {
            return this.Billing2;
        };
        Order.prototype.setBilling2 = function (Billing2) {
            this.Billing2 = Billing2;
        };
        Order.prototype.getZip = function () {
            return this.Zip;
        };
        Order.prototype.setZip = function (Zip) {
            this.Zip = Zip;
        };
        Order.prototype.getState = function () {
            return this.State;
        };
        Order.prototype.setState = function (State) {
            this.State = State;
        };
        Order.prototype.getCity = function () {
            return this.City;
        };
        Order.prototype.setCity = function (City) {
            this.City = City;
        };
        Order.prototype.getPhone = function () {
            return this.Phone;
        };
        Order.prototype.setPhone = function (Phone) {
            this.Phone = Phone;
        };
        Order.prototype.getShipping = function () {
            return this.Shipping;
        };
        Order.prototype.setShipping = function (Shipping) {
            this.Shipping = Shipping;
        };
        Order.prototype.getBilling = function () {
            return this.Billing;
        };
        Order.prototype.setBilling = function (Billing) {
            this.Billing = Billing;
        };
        Order.prototype.getInstitution = function () {
            return this.Institution;
        };
        Order.prototype.setInstitution = function (Institution) {
            this.Institution = Institution;
        };
        return Order;
    })();
    cuwebinars.Order = Order;
})(cuwebinars || (cuwebinars = {}));
//# sourceMappingURL=Main.js.map
