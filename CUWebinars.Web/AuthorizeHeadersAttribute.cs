using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CUWebinars.Web
{
    public class AuthorizeHeadersAttribute : AuthorizationFilterAttribute
    {
        private bool isAuthorized = false;
        private string usernameKey = "username";
        private string passwordKey = "password";
        private string username = null;
        private string role = null;

        public string UsernameKey
        {
            get
            {
                return this.usernameKey;
            }
        }

        public string PasswordKey
        {
            get
            {
                return this.passwordKey;
            }
        }

        public string Username
        {
            get
            {
                return this.username;
            }
        }

        public string Role
        {
            get
            {
                return this.role;
            }
        }

        public AuthorizeHeadersAttribute()
        {

        }

        public AuthorizeHeadersAttribute(string username, string role, string usernameKey = null, string passwordKey = null)
        {
            this.usernameKey = string.IsNullOrWhiteSpace(usernameKey) ? this.usernameKey : usernameKey;
            this.passwordKey = string.IsNullOrWhiteSpace(passwordKey) ? this.passwordKey : passwordKey; ;
            this.username = username;
            this.role = role;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            if (actionContext.Request.RequestUri.Scheme == Uri.UriSchemeHttps && request.Headers.Contains(UsernameKey) && request.Headers.Contains(PasswordKey))
            {
                var usernameHeader = request.Headers.Where(val => val.Key == UsernameKey).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(this.Username) && this.username != usernameHeader.Value.FirstOrDefault())
                {
                    isAuthorized = false;
                }
                else
                {
                    //TODO: Authorization logic here  
                    this.isAuthorized = true;
                    //Authorization logic end  
                }
            }


            if (!this.isAuthorized)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            }
        }

        protected virtual bool IsAuthorized(HttpActionContext actionContext)
        {
            return this.isAuthorized;
        }
    }  
         
}