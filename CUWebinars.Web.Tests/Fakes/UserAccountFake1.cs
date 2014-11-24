using System;
using System.Collections.Generic;
using BrockAllen.MembershipReboot;

namespace CUWebinars.Web.Tests.Fakes
{
    /// <summary>
    /// Provides stub for Email getter.
    /// </summary>
    public class UserAccountFake1 : UserAccount
    {
        public override string Email
        {
            get { return "testuser1@cuwebinars.com"; }
        }

        protected override void AddClaim(UserClaim item)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveClaim(UserClaim item)
        {
            throw new NotImplementedException();
        }

        protected override void AddLinkedAccount(LinkedAccount item)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveLinkedAccount(LinkedAccount item)
        {
            throw new NotImplementedException();
        }

        protected override void AddLinkedAccountClaim(LinkedAccountClaim item)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveLinkedAccountClaim(LinkedAccountClaim item)
        {
            throw new NotImplementedException();
        }

        protected override void AddCertificate(UserCertificate item)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveCertificate(UserCertificate item)
        {
            throw new NotImplementedException();
        }

        protected override void AddTwoFactorAuthToken(TwoFactorAuthToken item)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveTwoFactorAuthToken(TwoFactorAuthToken item)
        {
            throw new NotImplementedException();
        }

        protected override void AddPasswordResetSecret(PasswordResetSecret item)
        {
            throw new NotImplementedException();
        }

        protected override void RemovePasswordResetSecret(PasswordResetSecret item)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<UserClaim> Claims
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<LinkedAccount> LinkedAccounts
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<LinkedAccountClaim> LinkedAccountClaims
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<UserCertificate> Certificates
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<TwoFactorAuthToken> TwoFactorAuthTokens
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<PasswordResetSecret> PasswordResetSecrets
        {
            get { throw new NotImplementedException(); }
        }
    }
}
