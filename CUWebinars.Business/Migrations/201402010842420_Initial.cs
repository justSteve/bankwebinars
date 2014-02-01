namespace CUWebinars.Business.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddressType = c.String(),
                        Name = c.String(),
                        Phone = c.String(),
                        StreetAddress = c.String(),
                        StreetAddress2 = c.String(),
                        City = c.String(),
                        Zip = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        WebUser_Id = c.Int(),
                        WebUser_idUser = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WebUser", t => t.WebUser_idUser)
                .Index(t => t.WebUser_idUser);
            
            CreateTable(
                "dbo.WebUser",
                c => new
                    {
                        idUser = c.Int(nullable: false),
                        UserType = c.Int(nullable: false),
                        AcctStatus = c.String(nullable: false, maxLength: 1),
                        DateCreated = c.DateTime(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Initial = c.String(),
                        idUserInstitution = c.Int(nullable: false),
                        email = c.String(nullable: false, maxLength: 150),
                        futureMail = c.String(maxLength: 1),
                        generalComments = c.String(maxLength: 1000),
                        taxExempt = c.Boolean(),
                        idSubscriptionDiscount = c.Int(),
                        timeZone = c.Int(nullable: false),
                        Title = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.idUser)
                .ForeignKey("dbo.Institution", t => t.idUserInstitution, cascadeDelete: true)
                .Index(t => t.idUserInstitution);
            
            CreateTable(
                "dbo.Affiliate",
                c => new
                    {
                        idUserAff = c.Int(nullable: false),
                        CommissionModel = c.Byte(nullable: false),
                        URL = c.String(maxLength: 500),
                        WebBanner = c.String(nullable: false, maxLength: 4000),
                        WebFooter = c.String(nullable: false, maxLength: 4000),
                        EmailBanner = c.String(nullable: false, maxLength: 4000),
                        EmailFooter = c.String(nullable: false, maxLength: 4000),
                        ttsDomain = c.String(maxLength: 50),
                        GAPass = c.String(maxLength: 50),
                        supportEmail = c.String(maxLength: 75),
                        DisplayTitle = c.String(maxLength: 150),
                        BillingModel = c.String(maxLength: 150),
                        Logo = c.String(maxLength: 150),
                        ContactPerson = c.String(maxLength: 150),
                        ContactPhone = c.String(maxLength: 150),
                        ContactEmail = c.String(maxLength: 150),
                        ContactFax = c.String(maxLength: 150),
                        ContactAddress = c.String(maxLength: 150),
                        TechEmail = c.String(maxLength: 150),
                        TechPhone = c.String(maxLength: 150),
                        TechName = c.String(maxLength: 150),
                        EmailPromo = c.String(maxLength: 50),
                        WebUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.idUserAff)
                .ForeignKey("dbo.WebUser", t => t.idUserAff)
                .Index(t => t.idUserAff);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        idOrder = c.Int(nullable: false, identity: true),
                        idUser = c.Int(nullable: false),
                        idAffiliate = c.Int(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        Institution = c.String(maxLength: 250),
                        BillingPhone = c.String(maxLength: 30),
                        BillingEmail = c.String(maxLength: 150),
                        BillingAddress = c.String(maxLength: 100),
                        BillingAddress2 = c.String(maxLength: 100),
                        BillingCity = c.String(maxLength: 100),
                        BillingState = c.String(maxLength: 100),
                        BillingZip = c.String(maxLength: 20),
                        ShippingFirstName = c.String(maxLength: 100),
                        ShippingLastName = c.String(maxLength: 100),
                        ShippingPhone = c.String(maxLength: 30),
                        ShippingAddress = c.String(maxLength: 100),
                        ShippingAddress2 = c.String(maxLength: 100),
                        ShippingCity = c.String(maxLength: 100),
                        ShippingState = c.String(maxLength: 100),
                        ShippingZip = c.String(maxLength: 20),
                        PaymentType = c.Byte(nullable: false),
                        UserComments = c.String(maxLength: 2550),
                        AuditInfo = c.String(),
                        AffiliateComments = c.String(),
                        AdminComments = c.String(),
                        TaxExempt = c.Boolean(nullable: false),
                        InitiatedBy = c.Byte(nullable: false),
                        PaidByCCNumber = c.String(maxLength: 40),
                        Origin = c.String(),
                    })
                .PrimaryKey(t => t.idOrder)
                .ForeignKey("dbo.Affiliate", t => t.idAffiliate, cascadeDelete: true)
                .ForeignKey("dbo.WebUser", t => t.idUser, cascadeDelete: true)
                .Index(t => t.idAffiliate)
                .Index(t => t.idUser);
            
            CreateTable(
                "dbo.OrderRow",
                c => new
                    {
                        idOrderRow = c.Int(nullable: false, identity: true),
                        idOrder = c.Int(nullable: false),
                        idWebinar = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RowPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        idDiscount = c.Int(),
                        AlternateEmail = c.String(nullable: false, maxLength: 100),
                        RegistrationType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        ShipmentDate = c.DateTime(),
                        isAdHocRecording = c.Boolean(nullable: false),
                        Royalty = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.idOrderRow)
                .ForeignKey("dbo.Orders", t => t.idOrder, cascadeDelete: true)
                .ForeignKey("dbo.Webinar", t => t.idWebinar, cascadeDelete: true)
                .Index(t => t.idOrder)
                .Index(t => t.idWebinar);
            
            CreateTable(
                "dbo.OrderRowOptions",
                c => new
                    {
                        idOrderRowOption = c.Int(nullable: false, identity: true),
                        idOrderRow = c.Int(nullable: false),
                        idOption = c.Int(nullable: false),
                        OptionPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OptionDescription = c.String(nullable: false, maxLength: 255),
                        TaxExempt = c.Boolean(nullable: false),
                        Type = c.String(nullable: false, maxLength: 32),
                        additional_locations_count = c.Int(),
                        additional_locations_emails = c.String(maxLength: 1024),
                        AdditionalLocationsCount = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.idOrderRowOption)
                .ForeignKey("dbo.Options", t => t.idOption, cascadeDelete: true)
                .ForeignKey("dbo.OrderRow", t => t.idOrderRow, cascadeDelete: true)
                .Index(t => t.idOption)
                .Index(t => t.idOrderRow);
            
            CreateTable(
                "dbo.Options",
                c => new
                    {
                        idOption = c.Int(nullable: false, identity: true),
                        OptionExplain = c.String(),
                        OptionLabel = c.String(maxLength: 150),
                        PriceToAdd = c.Double(),
                        TaxExempt = c.Boolean(),
                        PercToAdd = c.Double(),
                        SortOrder = c.Int(),
                        Type = c.String(nullable: false, maxLength: 32),
                        MsgConfirm = c.String(maxLength: 10),
                        SKU = c.String(maxLength: 150),
                        ShowLiveNotifications = c.String(),
                        ShowRecordingNotifications = c.String(),
                        ShowShippedNotifications = c.String(),
                        Stage1CheckoutConfirmationMsg = c.String(),
                        Stage2CheckoutConfirmationMsg = c.String(),
                        Stage1EmailConfirmationMsg = c.String(),
                        Stage2EmailConfirmationMsg = c.String(),
                        //Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.idOption);
            
            CreateTable(
                "dbo.OptionsXref",
                c => new
                    {
                        idOptionsXref = c.Int(nullable: false, identity: true),
                        idOptionGroup = c.Int(nullable: false),
                        idOption = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idOptionsXref)
                .ForeignKey("dbo.Options", t => t.idOption, cascadeDelete: true)
                .ForeignKey("dbo.OptionsGroups", t => t.idOptionGroup, cascadeDelete: true)
                .Index(t => t.idOption)
                .Index(t => t.idOptionGroup);
            
            CreateTable(
                "dbo.OptionsGroups",
                c => new
                    {
                        idOptionGroup = c.Int(nullable: false, identity: true),
                        OptionGroupDesc = c.String(maxLength: 50),
                        OptionType = c.String(maxLength: 1),
                        SortOrder = c.Int(),
                    })
                .PrimaryKey(t => t.idOptionGroup);
            
            CreateTable(
                "dbo.OptionsGroupsXref",
                c => new
                    {
                        idWebinarOptionGroup = c.Int(nullable: false, identity: true),
                        idWebinar = c.Int(nullable: false),
                        idOptionGroup = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idWebinarOptionGroup)
                .ForeignKey("dbo.OptionsGroups", t => t.idOptionGroup, cascadeDelete: true)
                .ForeignKey("dbo.Webinar", t => t.idWebinar, cascadeDelete: true)
                .Index(t => t.idOptionGroup)
                .Index(t => t.idWebinar);
            
            CreateTable(
                "dbo.Webinar",
                c => new
                    {
                        idWebinar = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        DescriptionLong = c.String(nullable: false),
                        ImageUrl = c.String(maxLength: 50),
                        SmallImageUrl = c.String(maxLength: 50),
                        Status = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 125),
                        Date = c.DateTime(nullable: false),
                        LearnCaption = c.String(nullable: false, maxLength: 255),
                        LearnBody = c.String(nullable: false),
                        WhoAttend = c.String(nullable: false),
                        Duration = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RecordingUrl = c.String(nullable: false, maxLength: 300),
                        idPresenter = c.Int(nullable: false),
                        AdditionalNotifications = c.String(),
                        ceu = c.String(maxLength: 1000),
                        ConnectionInfo = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateChanged = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.idWebinar)
                .ForeignKey("dbo.Presenter", t => t.idPresenter, cascadeDelete: true)
                .Index(t => t.idPresenter);
            
            CreateTable(
                "dbo.Presenter",
                c => new
                    {
                        idUser = c.Int(nullable: false),
                        Biography = c.String(nullable: false, maxLength: 2500),
                        BiographyLong = c.String(nullable: false, maxLength: 2500),
                        PhotoFull = c.String(maxLength: 200),
                        PhotoThumb = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.idUser)
                .ForeignKey("dbo.WebUser", t => t.idUser)
                .Index(t => t.idUser);
            
            CreateTable(
                "dbo.WebinarFile",
                c => new
                    {
                        idWebinarFile = c.Int(nullable: false, identity: true),
                        idWebinar = c.Int(nullable: false),
                        fileLocation = c.String(nullable: false, maxLength: 255),
                        fileDesc = c.String(nullable: false, maxLength: 1000),
                    })
                .PrimaryKey(t => t.idWebinarFile)
                .ForeignKey("dbo.Webinar", t => t.idWebinar, cascadeDelete: true)
                .Index(t => t.idWebinar);
            
            CreateTable(
                "dbo.WebinarTopicXref",
                c => new
                    {
                        idWebinarTopicXref = c.Int(nullable: false, identity: true),
                        idWebinar = c.Int(nullable: false),
                        idTopic = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idWebinarTopicXref)
                .ForeignKey("dbo.Topic", t => t.idTopic, cascadeDelete: true)
                .ForeignKey("dbo.Webinar", t => t.idWebinar, cascadeDelete: true)
                .Index(t => t.idTopic)
                .Index(t => t.idWebinar);
            
            CreateTable(
                "dbo.Topic",
                c => new
                    {
                        idTopic = c.Int(nullable: false, identity: true),
                        topicDesc = c.String(nullable: false, maxLength: 50),
                        idParentTopic = c.Int(),
                        topicHTML = c.String(nullable: false, maxLength: 255),
                        sortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idTopic)
                .ForeignKey("dbo.Topic", t => t.idParentTopic)
                .Index(t => t.idParentTopic);
            
            CreateTable(
                "dbo.Institution",
                c => new
                    {
                        idInstitution = c.Int(nullable: false, identity: true),
                        InstitutionName = c.String(nullable: false, maxLength: 250),
                        InstitutionType = c.String(nullable: false, maxLength: 20),
                        domainName = c.String(maxLength: 75),
                        RegIdentifier = c.String(maxLength: 40),
                        Address = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zip = c.String(),
                    })
                .PrimaryKey(t => t.idInstitution);
            
            CreateTable(
                "dbo.Discounts",
                c => new
                    {
                        idDiscounts = c.Int(nullable: false, identity: true),
                        discountType = c.Byte(nullable: false),
                        code = c.String(nullable: false, maxLength: 50),
                        percentOff = c.Decimal(nullable: false, precision: 18, scale: 2),
                        flatOff = c.Decimal(nullable: false, precision: 18, scale: 2),
                        usesNumber = c.Int(nullable: false),
                        dateValidFrom = c.DateTime(nullable: false),
                        dateValidTo = c.DateTime(nullable: false),
                        status = c.String(nullable: false, maxLength: 50),
                        dateBilled = c.DateTime(),
                        cost = c.Decimal(precision: 18, scale: 2),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.idDiscounts);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "WebUser_idUser", "dbo.WebUser");
            DropForeignKey("dbo.WebUser", "idUserInstitution", "dbo.Institution");
            DropForeignKey("dbo.Affiliate", "idUserAff", "dbo.WebUser");
            DropForeignKey("dbo.Orders", "idUser", "dbo.WebUser");
            DropForeignKey("dbo.OrderRow", "idWebinar", "dbo.Webinar");
            DropForeignKey("dbo.OrderRowOptions", "idOrderRow", "dbo.OrderRow");
            DropForeignKey("dbo.OrderRowOptions", "idOption", "dbo.Options");
            DropForeignKey("dbo.OptionsXref", "idOptionGroup", "dbo.OptionsGroups");
            DropForeignKey("dbo.OptionsGroupsXref", "idWebinar", "dbo.Webinar");
            DropForeignKey("dbo.WebinarTopicXref", "idWebinar", "dbo.Webinar");
            DropForeignKey("dbo.WebinarTopicXref", "idTopic", "dbo.Topic");
            DropForeignKey("dbo.Topic", "idParentTopic", "dbo.Topic");
            DropForeignKey("dbo.WebinarFile", "idWebinar", "dbo.Webinar");
            DropForeignKey("dbo.Webinar", "idPresenter", "dbo.Presenter");
            DropForeignKey("dbo.Presenter", "idUser", "dbo.WebUser");
            DropForeignKey("dbo.OptionsGroupsXref", "idOptionGroup", "dbo.OptionsGroups");
            DropForeignKey("dbo.OptionsXref", "idOption", "dbo.Options");
            DropForeignKey("dbo.OrderRow", "idOrder", "dbo.Orders");
            DropForeignKey("dbo.Orders", "idAffiliate", "dbo.Affiliate");
            DropIndex("dbo.Addresses", new[] { "WebUser_idUser" });
            DropIndex("dbo.WebUser", new[] { "idUserInstitution" });
            DropIndex("dbo.Affiliate", new[] { "idUserAff" });
            DropIndex("dbo.Orders", new[] { "idUser" });
            DropIndex("dbo.OrderRow", new[] { "idWebinar" });
            DropIndex("dbo.OrderRowOptions", new[] { "idOrderRow" });
            DropIndex("dbo.OrderRowOptions", new[] { "idOption" });
            DropIndex("dbo.OptionsXref", new[] { "idOptionGroup" });
            DropIndex("dbo.OptionsGroupsXref", new[] { "idWebinar" });
            DropIndex("dbo.WebinarTopicXref", new[] { "idWebinar" });
            DropIndex("dbo.WebinarTopicXref", new[] { "idTopic" });
            DropIndex("dbo.Topic", new[] { "idParentTopic" });
            DropIndex("dbo.WebinarFile", new[] { "idWebinar" });
            DropIndex("dbo.Webinar", new[] { "idPresenter" });
            DropIndex("dbo.Presenter", new[] { "idUser" });
            DropIndex("dbo.OptionsGroupsXref", new[] { "idOptionGroup" });
            DropIndex("dbo.OptionsXref", new[] { "idOption" });
            DropIndex("dbo.OrderRow", new[] { "idOrder" });
            DropIndex("dbo.Orders", new[] { "idAffiliate" });
            DropTable("dbo.Discounts");
            DropTable("dbo.Institution");
            DropTable("dbo.Topic");
            DropTable("dbo.WebinarTopicXref");
            DropTable("dbo.WebinarFile");
            DropTable("dbo.Presenter");
            DropTable("dbo.Webinar");
            DropTable("dbo.OptionsGroupsXref");
            DropTable("dbo.OptionsGroups");
            DropTable("dbo.OptionsXref");
            DropTable("dbo.Options");
            DropTable("dbo.OrderRowOptions");
            DropTable("dbo.OrderRow");
            DropTable("dbo.Orders");
            DropTable("dbo.Affiliate");
            DropTable("dbo.WebUser");
            DropTable("dbo.Addresses");
        }
    }
}
