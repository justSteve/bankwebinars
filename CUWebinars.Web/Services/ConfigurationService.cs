using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Web.Services
{
    /// <summary>
    /// This class handles interaction with the Azure Key Vault
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        #region Members

        private static IConfiguration _config;

        #endregion

        #region Constructors

        public ConfigurationService()
        {
            Task.Run( Initialize );
        }

        #endregion

        #region Properties

        public string MailChimpId => _config["MailChimpId"];

        #endregion


        private static async Task Initialize( )
        {
            try
            {
                if (_config != null)
                {
                    return;
                }

                var builder = new ConfigurationBuilder( );

                builder.AddAzureKeyVault(
                    ConfigurationManager.AppSettings[ "AzureVaultUri" ],
                    ConfigurationManager.AppSettings[ "AzureVaultClientId" ],
                    ConfigurationManager.AppSettings[ "AzureVaultClientSecret" ] );

                await Task.Run( ( ) => _config = builder.Build( ) );
            }
            catch (Exception ex)
            {
                var t = ex;
            }


        }
    }
}
