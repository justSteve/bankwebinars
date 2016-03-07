using System.Security.Cryptography;

namespace CUWebinars.Business.Core
{
    public enum OrderGenesis
    {
        ImportedForNewUser = 0,
        ImportedForExistingUser = 1,
        CreatedViaCartByNewUser = 2,
        CreatedViaCartByExistingUser = 3,
        CreatedViaExpressCheckout = 4,
        Resend = 5,
                ImportedForACSNewUser = 6,
        ImportedForACSExistingUser = 7,
    }
}