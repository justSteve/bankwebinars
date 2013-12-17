/// <reference path="../typings/jquery/jquery.d.ts"/>
/// <reference path="../typings/bootstrap/bootstrap.d.ts"/>
//http://visualstudiomagazine.com/articles/2013/06/01/test-driven-development-with-typescript.aspx

// Interface
interface IOrder {
    getIdOrder();
    setIdOrder(IdOrder: number);
    getIdAffiliate();
    setIdAffiliate(IdAffiliate: number);
    getIdUser();
    setIdUser(IdUser: number);
    getFirstName();
    getLastName();
    setFirstName(FirstName: string);
    setLastName(LastName: string);
    getOrigin();
    setOrigin(Origin: string);
    getEmail();
    setEmail(Email: string);
    getShipping2();
    setShipping2(Shipping2: string);
    getBilling2();
    setBilling2(Billing2: string);
    getZip();
    setZip(Zip: string);
    getState();
    setState(State: string);
    getCity();
    setCity(City: string);
    getPhone();
    setPhone(Phone: string);
    getShipping();
    setShipping(Shipping: string);
    getBilling();
    setBilling(Billing: string);
    getInstitution();
    setInstitution(Institution: string);
}
interface IWebUser {

    getIdUser();
    setIdUser(IdUser: number);
    getFirstName();
    setFirstName(FirstName: string);
    getLastName();
    setLastName(LastName: string);
    getuType();
    setuType(uType: string);
    getEmail();
    setEmail(Email: string);
    getShipping2();
    setShipping2(Shipping2: string);
    getBilling2();
    setBilling2(Billing2: string);
    getZip();
    setZip(Zip: string);
    getState();
    setState(State: string);
    getCity();
    setCity(City: string);
    getPhone();
    setPhone(Phone: string);
    getShipping();
    setShipping(Shipping: string);
    getBilling();
    setBilling(Billing: string);
    getInstitution();
    setInstitution(Institution: string);
}

// Module
module cuwebinars {

    // Class
    export class wUser implements IWebUser {
        // Constructor
        constructor(
            public IdUser: number,
            public FirstName: string,
            public LastName: string,
            public Institution: string,
            public uType: string,
            public Email: string,
            public Shipping: string,
            public Shipping2: string,
            public Billing: string,
            public Billing2: string,
            public Zip: string,
            public State: string,
            public City: string,
            public Phone: string
            ) { }

        public getIdUser() { return this.IdUser; }
        public setIdUser(IdUser: number) { this.IdUser = IdUser; }
        public getFirstName() { return this.FirstName; }
        public setFirstName(FirstName: string) { this.FirstName = FirstName; }
        public getLastName() { return this.LastName; }
        public setLastName(LastName: string) { this.LastName = LastName; }
        public getuType() { return this.uType; }
        public setuType(uType: string) { this.uType = uType; }
        public getEmail() { return this.Email; }
        public setEmail(Email: string) { this.Email = Email; }
        public getShipping2() { return this.Shipping2; }
        public setShipping2(Shipping2: string) { this.Shipping2 = Shipping2; }
        public getBilling2() { return this.Billing2; }
        public setBilling2(Billing2: string) { this.Billing2 = Billing2; }
        public getZip() { return this.Zip; }
        public setZip(Zip: string) { this.Zip = Zip; }
        public getState() { return this.State; }
        public setState(State: string) { this.State = State; }
        public getCity() { return this.City; }
        public setCity(City: string) { this.City = City; }
        public getPhone() { return this.Phone; }
        public setPhone(Phone: string) { this.Phone = Phone; }
        public getShipping() { return this.Shipping; }
        public setShipping(Shipping: string) { this.Shipping = Shipping; }
        public getBilling() { return this.Billing; }
        public setBilling(Billing: string) { this.Billing = Billing; }
        public getInstitution() { return this.Institution; }
        public setInstitution(Institution: string) { this.Institution = Institution; }
    }


    // Class
    export class Order implements IOrder {
        // Constructor
        constructor(
            public IdOrder: number,
            public IdAffiliate: number,
            public IdUser: number,
            public FirstName: string,
            public LastName: string,
            public Institution: string,
            public Origin: string,
            public Email: string,
            public Shipping: string,
            public Shipping2: string,
            public Billing: string,
            public Billing2: string,
            public Zip: string,
            public State: string,
            public City: string,
            public Phone: string
            ) { }

        public getIdOrder() { return this.IdOrder; }
        public setIdOrder(IdOrder: number) { this.IdOrder = IdOrder; }
        public getIdAffiliate() { return this.IdAffiliate; }
        public setIdAffiliate(IdAffiliate: number) { this.IdAffiliate = IdAffiliate; }
        public getIdUser() { return this.IdUser; }
        public setIdUser(IdUser: number) { this.IdUser = IdUser; }
        public getFirstName() { return this.FirstName; }
        public getLastName() { return this.LastName; }
        public setFirstName(FirstName: string) { this.FirstName = FirstName; }
        public setLastName(LastName: string) { this.LastName = LastName; }
        public getOrigin() { return this.Origin; }
        public setOrigin(Origin: string) { this.Origin = Origin; }
        public getEmail() { return this.Email; }
        public setEmail(Email: string) { this.Email = Email; }
        public getShipping2() { return this.Shipping2; }
        public setShipping2(Shipping2: string) { this.Shipping2 = Shipping2; }
        public getBilling2() { return this.Billing2; }
        public setBilling2(Billing2: string) { this.Billing2 = Billing2; }
        public getZip() { return this.Zip; }
        public setZip(Zip: string) { this.Zip = Zip; }
        public getState() { return this.State; }
        public setState(State: string) { this.State = State; }
        public getCity() { return this.City; }
        public setCity(City: string) { this.City = City; }
        public getPhone() { return this.Phone; }
        public setPhone(Phone: string) { this.Phone = Phone; }
        public getShipping() { return this.Shipping; }
        public setShipping(Shipping: string) { this.Shipping = Shipping; }
        public getBilling() { return this.Billing; }
        public setBilling(Billing: string) { this.Billing = Billing; }
        public getInstitution() { return this.Institution; }
        public setInstitution(Institution: string) { this.Institution = Institution; }
    }
}

// Local variables..
//var Id: IPoint = new Shapes.Point(3, 4);
//var dist = p.getDist();