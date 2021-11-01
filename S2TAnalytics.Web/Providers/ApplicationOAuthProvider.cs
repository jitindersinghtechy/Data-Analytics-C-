using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using S2TAnalytics.Infrastructure.Services;
using S2TAnalytics.DAL.UnitOfWork;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Models;

namespace S2TAnalytics.Web.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }
            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            if (allowedOrigin == null) allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            UnitOfWork u = new UnitOfWork(ReadConfiguration.ConnectionString, ReadConfiguration.DataBaseName);
            AuthenticateService authenticateService = new AuthenticateService(u);

            // Validate your user and base on validation return claim identity or invalid_grant error
            string email = context.UserName;
            string password = context.Password;

            var requestParameters = await context.Request.ReadFormAsync();
            var OTP = requestParameters["OTP"];
            var loginType = requestParameters["loginType"];


            bool IsExpired = false;

            if (loginType == "user")
            {
                var authenticateResponse = authenticateService.AuthenticateUser(email, password, ref IsExpired);

                if (authenticateResponse.Success)
                {
                    UserModel userModel = (UserModel)authenticateResponse.Data;
                    string userGroups = "";
                    if (userModel.UserGroups != null && userModel.UserGroups.Count > 0)
                    {
                        foreach (string group in userModel.UserGroups)
                            userGroups += group + ",";
                        userGroups = userGroups.Substring(0, userGroups.Length - 1);
                    }

                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                    identity.AddClaim(new Claim("sub", context.UserName));
                    identity.AddClaim(new Claim("Email", context.UserName));
                    identity.AddClaim(new Claim("OrganizationID", userModel.OrganizationID.ToString()));
                    identity.AddClaim(new Claim("UserID", userModel.UserID));
                    identity.AddClaim(new Claim("RoleID", userModel.RoleID));
                    identity.AddClaim(new Claim("DatasourceIds", userModel.CommaSeperatedDatasourceIds));
                    identity.AddClaim(new Claim("fullName", userModel.FirstName + " " + userModel.LastName));
                    identity.AddClaim(new Claim("UserGroups", userGroups));
                    identity.AddClaim(new Claim("PlanPermissionIds", userModel.CommaSeperatedPlanPermissionIds));

                    var props = new AuthenticationProperties(new Dictionary<string, string> {
                                        {"userName", context.UserName},
                                        {"fullName", userModel.FirstName + " " + userModel.LastName},
                                        {"RoleID", userModel.RoleID},
                                        {"IsExpired", IsExpired.ToString()},
                                        {"PlanPermissionIds", userModel.CommaSeperatedPlanPermissionIds},
                                    });

                    var ticket = new AuthenticationTicket(identity, props);
                    context.Validated(ticket);
                }
                else
                {
                    context.SetError("invalid_grant", authenticateResponse.Message);
                    // context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }
            else
            {
                if (OTP != null)
                {
                    var authenticateResponse = authenticateService.AuthenticateSuperAdmin(email, password, OTP);
                    
                    // if (((email == "admin") && (password == "admin")) || ((email == "jignesh") && (password == "abc")))
                    if (authenticateResponse.Success)
                    {
                        UserModel userModel = (UserModel)authenticateResponse.Data;
                        var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                        identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                        identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                        identity.AddClaim(new Claim("sub", context.UserName));
                        identity.AddClaim(new Claim("Email", context.UserName));
                        identity.AddClaim(new Claim("OrganizationID", userModel.OrganizationID.ToString()));
                        identity.AddClaim(new Claim("UserID", userModel.UserID));
                        identity.AddClaim(new Claim("RoleID", userModel.RoleID));
                        //identity.AddClaim(new Claim("DatasourceIds", userModel.CommaSeperatedDatasourceIds));
                        identity.AddClaim(new Claim("fullName", userModel.FirstName + " " + userModel.LastName));

                        var props = new AuthenticationProperties(new Dictionary<string, string> {
                                             {"userName", context.UserName},
                                             {"fullName", userModel.FirstName + " " + userModel.LastName},
                                             {"RoleID", userModel.RoleID},

                                         });
                        var ticket = new AuthenticationTicket(identity, props);
                        context.Validated(ticket);
                        authenticateService.EmptyOTP(email, password);
                    }
                    else
                    {
                        context.SetError("invalid_grant", "The user name or password or OTP is incorrect.");
                        return;
                    }
                }
                else
                {
                    context.SetError("invalid_grant", "Please Enter OTP");
                    return;
                }
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

        //public async Task UpdateClaim(OAuthGrantResourceOwnerCredentialsContext context)
        //{

        //    var isValidated = context.Validated();

        //    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
        //    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
        //    identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
        //    identity.AddClaim(new Claim("sub", context.UserName));
        //    identity.AddClaim(new Claim("Email", context.UserName));
        //    identity.AddClaim(new Claim("OrganizationID", ""));
        //    identity.AddClaim(new Claim("UserID", ""));
        //    identity.AddClaim(new Claim("RoleID", ""));
        //    identity.AddClaim(new Claim("DatasourceIds", ""));
        //    identity.AddClaim(new Claim("fullName", "AA"));

        //    context.Validated(identity);
        //    return;
        //}
    }
}