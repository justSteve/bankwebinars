using CUWebinars.Business.CQS.Queries;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface IOrderControllerOrchestrator
    {
        int CreateNewOrder(IncomingOrderModel incomingOrderModel,
            string email,
            OrderManagementQueryResult orderManagementQueryResult,
            string verificationKey,
            string confirmChangeEmailUrl
            );
        int MigrateOrder(MigrateOrderModel migrateOrderModel,
            string email,
            MigratorQueryResult migratorQueryResult,
            string verificationKey,
            string confirmChangeEmailUrl
            );
        int ImportOrder(ImportOrderModel importOrderModel,
            string email,
            ImportQueryResult importQueryResult,
            string verificationKey,
            string confirmChangeEmailUrl
            );
        void FinalizeNewRegistration(IncomingOrderModel incomingOrderModel, string verificationKey);
        void FinalizeMigratedRegistation(MigrateOrderModel migrateOrder, string verificationKey);
        void FinalizeImportedRegistation(ImportOrderModel importOrder, string verificationKey);
        OrderManagementQueryResult GetPreparatoryData(IncomingOrderModel incomingOrderModel, string email);
        MigratorQueryResult GetPreparatoryDataForMigrator(MigrateOrderModel migrateOrder, string email);
        ImportQueryResult GetPreparatoryDataForImporter(ImportOrderModel importOrder, string email);
        string GetConfirmChangeEmailLinkForNewUserAccount();
        string GetVerificationKeyForNewUserAccount();
        WebUser ProcessNewUser(IncomingOrderModel incomingOrderModel, string email);
        WebUser MigrateUser(MigrateOrderModel migrateOrder, string email);
        WebUser ImportUser(ImportOrderModel importedOrder, string email);
    }
}
