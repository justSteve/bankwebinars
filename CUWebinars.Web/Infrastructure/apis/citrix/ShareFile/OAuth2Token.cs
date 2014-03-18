using Newtonsoft.Json.Linq;

/**
 * Copyright (c) 2014 Citrix Systems, Inc.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */

/**
 * The methods in this file will make use of the ShareFile API v3 to show some of the basic
 * operations using GET, POST, PATCH, DELETE HTTP verbs. See api.sharefile.com for more information.
 *
 * Requirements:
 *
 * Json.NET library. see http://json.codeplex.com
 *
 * JSON Deserialization:
 * 
 * The sample methods here simply use the JObject data accessors, rather than deserializing to a ShareFile Class representation.
 *
 * Authentication:
 *
 * OAuth2 password grant is used for authentication. After the token is acquired it is sent an an
 * authorization header with subsequent API requests.
 *
 * Exception / Error Checking:
 *
 * For simplicity, exception handling has not been added.  Code should not be used in a production environment.
 */
namespace CUWebinars.Web.Infrastructure.apis.citrix.ShareFile
{
    class OAuth2Token
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
        public string Appcp { get; set; }
        public string Apicp { get; set; }
        public string Subdomain { get; set; }
        public int ExpiresIn { get; set; }

        public OAuth2Token(JObject json)
        {
            if (json != null)
            {
                AccessToken = (string)json["access_token"];
                RefreshToken = (string)json["refresh_token"];
                TokenType = (string)json["token_type"];
                Appcp = (string)json["appcp"];
                Apicp = (string)json["apicp"];
                Subdomain = (string)json["subdomain"];
                ExpiresIn = (int)json["expires_in"];
            }
            else
            {
                AccessToken = "";
                RefreshToken = "";
                TokenType = "";
                Appcp = "";
                Apicp = "";
                Subdomain = "";
                ExpiresIn = 0;
            }
        }
    }
}