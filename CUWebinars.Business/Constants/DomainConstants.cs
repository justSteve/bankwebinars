
using System;

namespace CUWebinars.Business.Constants
{
    public sealed class DomainConstants
    {
        public const string AdminEmailedPrefix = "AdminEmailed-";
        public const string Active = "A";
        public const string BillingAddress = "Billing";
        public const string Blank = "blank";
        public const string Cart = "Cart";
        public const string CartCreatedUserPasswordCreate = "CartCreatedUserPasswordCreate";
        public const string CheckoutInProcess = "CheckoutInProcess"; 
        public const string ConfirmChangeEmailLink = "ConfirmChangeEmailLink";
        public const string DateTimeLongFormat = "yyyy-MM-dd-hh-mm-ss-fff-tt";
        public const string CareOfString = "c/o ";
        public const string JoinUrl = "joinUrl";
        public const string LoggerConnectionStringName = "LoggerConnection";
        public const string New = "N";

        public const string CartByAffiliate = "CartByAffiliate";
        public const string OriginImported = "Imported";
        public const string OriginImportedACS = "ImportedByACS";
        public const string OriginExpress = "Express";
        public const string OriginMigrated = "Migrator";
        public const string OriginResume = "Resume";

        public const string RazorExtension = ".cshtml";
        public const string RegistrantKey = "registrantKey";
        public const string ResetPasswordRequested = "ResetPasswordRequested";
        public const string ResourcePathTemplate = @"Notification\Templates";
        public const string ResourcePathPreviewTemplate = @"Notification\PreviewTemplates";
        public const string ShippingAddress = "Shipping";
        public const string TempPassword = "TempPassword";
        public const string UserCreatedDuringCartCheckout = "UserCreatedDuringCartCheckout";
        public const string UserCreatedViaNewOrder = "UserCreatedViaNewOrder";
        public const string UserCreatedViaMigrator = "UserCreatedViaMigrator";
        public const string UserNotFound = "User not found";
        public const string UsLocale = "en-US";
        public const string VerificationKey = "VerificationKey";
        public const string VerificationKeyForBatchChangePwd = "VerificationKeyForBatchChangePwd";
        public const string VerifyEmailLink = "VerifyEmailLink";
        public const string BankWebinars = "BankWebinars";
        // ReSharper disable once InconsistentNaming
        public const string CUWebinars = "CUWebinars";
        public const string ClaimDateFormatText = "yyyy-MM-dd";
        public const string UtcNowAsCts = "yyyy-MM-dd";

        public static DateTime BuildUtcNowAsCts
        {
            get
            {
                DateTime timeUtc = DateTime.UtcNow;
                return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            }
        }


        public const string CreationModeImported = "imported";
        public const string CreationModeMigrated = "migrated";
        public const string CreationModeExpress = "express";
    }
}
