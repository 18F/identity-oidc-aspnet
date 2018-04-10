using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ASPNET_OIDC.Controllers
{
    public class AccountController : Controller
    {
        public const string ClientId = "urn:gov:gsa:openidconnect.profiles:sp:sso:logingov:aspnet_example";
        public const string ClientUrl = "http://localhost:50764";
        public const string IdpUrl = "https://idp.int.identitysandbox.gov";
        public const string AcrValues = "http://idmanagement.gov/ns/assurance/loa/1";

        public ActionResult Index()
        {
            if (TempData["email"] == null)
            {
                ViewBag.Message = "Log in to see your account.";
            }
            else
            {
                ViewBag.Message = $"Welcome back {TempData["email"]}!";
                ViewBag.Content = $"Your user ID is: {TempData["id"]}";
            }
            return View();
        }

        public ActionResult SignIn()
        {
            var state = Guid.NewGuid().ToString("N");
            var nonce = Guid.NewGuid().ToString("N");

            var url = $"{IdpUrl}/openid_connect/authorize?" +
                "&acr_values=" + HttpUtility.HtmlEncode(AcrValues) +
                "&client_id=" + ClientId +
                "&prompt=select_account" +
                "&redirect_uri=" + $"{ClientUrl}/Account/SignInCallback" +
                "&response_type=code" +
                "&scope=openid+email" +
                "&state=" + state +
                "&nonce=" + nonce;
            System.Diagnostics.Debug.WriteLine($"url: {url}");
            return Redirect(url);
        }

        public ActionResult SignInCallback()
        {
            // Retrieve code and state from query string, pring for debugging
            var code = Request.QueryString["code"];
            var state = Request.QueryString["state"];
            System.Diagnostics.Debug.WriteLine($"code: {code}");
            System.Diagnostics.Debug.WriteLine($"state: {state}");

            // Generate JWT for token request
            var cert = new X509Certificate2(Server.MapPath("~/App_Data/cert.pfx"), "1234");
            var signingCredentials = new SigningCredentials(new X509SecurityKey(cert), SecurityAlgorithms.RsaSha256);
            var header = new JwtHeader(signingCredentials);
            var payload = new JwtPayload
            {
                {"iss", ClientId},
                {"sub", ClientId},
                {"aud", $"{IdpUrl}/api/openid_connect/token"},
                {"jti", Guid.NewGuid().ToString("N")},
                {"exp", (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds + 5 * 60}
            };
            var securityToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(securityToken);

            // Send POST to make token request
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                data["client_assertion"] = tokenString;
                data["client_assertion_type"] = HttpUtility.HtmlEncode("urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
                data["code"] = code;
                data["grant_type"] = "authorization_code";

                var response = wb.UploadValues($"{IdpUrl}/api/openid_connect/token", "POST", data);

                var responseString = Encoding.ASCII.GetString(response);
                dynamic tokenResponse = JObject.Parse(responseString);

                var token = handler.ReadToken((String)tokenResponse.id_token) as JwtSecurityToken;
                var userId = token.Claims.First(c => c.Type == "sub").Value;
                var userEmail = token.Claims.First(c => c.Type == "email").Value;

                TempData["id"] = userId;
                TempData["email"] = userEmail;
                return RedirectToAction("Index");
            }
        }
    }
}
