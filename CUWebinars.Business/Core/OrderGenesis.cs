namespace CUWebinars.Business.Core
{
    public enum OrderGenesis
    {
        ImportedForNewUser = 0,
        ImportedForExistingUser = 1,
        CreatedViaCartByNewUser = 2,
        CreatedViaCartByExistingUser = 3
    }
}