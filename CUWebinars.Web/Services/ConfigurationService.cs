using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Web.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public ConfigurationService()
        {

        }

        private static async Task Initialize( )
        {
            try
            {
                if (config != null)
                {
                    return;
                }

                var builder = new ConfigurationBuilder( );

                builder.AddAzureKeyVault(
                    ConfigurationManager.AppSettings[ "Vault" ],
                    ConfigurationManager.AppSettings[ "ClientId" ],
                    ConfigurationManager.AppSettings[ "ClientSecret" ] );

                await Task.Run( ( ) => config = builder.Build( ) );
            }
            catch (Exception ex)
            {
                var t = ex;
            }


        }
    }
}
