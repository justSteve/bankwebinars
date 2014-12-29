using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrockAllen.MembershipReboot;

namespace CUWebinars.Business.Tests.Fakes
{
    internal class UserAccountRepositoryFake : IUserAccountRepository
    {
        public UserAccount Create()
        {
            throw new NotImplementedException();
        }

        public void Add(UserAccount item)
        {
            throw new NotImplementedException();
        }

        public void Remove(UserAccount item)
        {
            throw new NotImplementedException();
        }

        public void Update(UserAccount item)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetByID(Guid id)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetByUsername(string tenant, string username)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetByEmail(string tenant, string email)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetByMobilePhone(string tenant, string phone)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetByVerificationKey(string key)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetByLinkedAccount(string tenant, string provider, string id)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetByCertificate(string tenant, string thumbprint)
        {
            throw new NotImplementedException();
        }
    }
}
