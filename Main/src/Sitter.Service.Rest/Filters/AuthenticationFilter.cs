using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using Amazon.Runtime.Internal.Util;
using MySitterHub.Logic.Repository;
using MySitterHub.Logic.ServiceModels;
using MySitterHub.Model.Common.Authentication;
using MySitterHub.Model.Misc;

namespace MySitterHub.Service.Rest.Filters
{
    public class AuthenticationFilter : System.Web.Http.Filters.IAuthenticationFilter
    {

        private static readonly HashSet<String> excludedRoutes = new HashSet<string>()
        {
            "/api/auth",
            "/api/signup",
            "/api/twilio",
            "/api/forgotpassword/changerequest",
            "/api/forgotpassword/sendcode",
            "/api/forgotpassword/changepassword"
        };

        private const string SimpleTokenAuthSchema = "SimpleToken";
        
        public bool AllowMultiple { get; private set; }

        private AppLogger _log =new AppLogger(); 

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            string logMsg=null;
            LogLevel logLevel = LogLevel.Error;
            try
            {
                // Log request

                if (context.Request == null)
                {
                    logMsg = "context.Request == null";
                    return;
                }

                // Don't authenticate
                if (excludedRoutes.Contains(context.Request.RequestUri.AbsolutePath.ToLower()))
                {
                    logMsg = "EXCLUDED ROUTE";
                    logLevel = LogLevel.Info;
                    return;
                }

                // 1. Look for credentials in the request.
                HttpRequestMessage request = context.Request;
                AuthenticationHeaderValue authorization = request.Headers.Authorization;

                // 2. If there are no credentials, do nothing.
                if (authorization == null)
                {
                    logMsg = "UNAUTHENTICATED REQUEST";
                    context.ErrorResult = new AuthenticationFailureResult("Authorization header is missing", request);
                    return;
                }


                // 3. If there are credentials but the filter does not recognize the 
                //    authentication scheme, do nothing.
                if (authorization.Scheme != SimpleTokenAuthSchema)
                {
                    logMsg = "authorization.Scheme != SimpleTokenAuthSchema";
                    context.ErrorResult = new AuthenticationFailureResult("Invalid auth scheme", request);
                    return;
                }

                // 4. If there are credentials that the filter understands, try to validate them.
                // 5. If the credentials are bad, set the error result.
                if (String.IsNullOrEmpty(authorization.Parameter))
                {
                    logMsg = "Missing credentials";
                    context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                    return;
                }

                var authenticationToken = ExtractUserNameAndToken(authorization.Parameter);
                if (authenticationToken == null || string.IsNullOrWhiteSpace(authenticationToken.UserName) || authenticationToken.UserName == "null")
                {
                    logMsg = "Missing authenticationToken";
                    context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                    return;
                }

                IPrincipal principal = AuthenticateCredentials(authenticationToken);
                if (principal == null)
                {
                    logMsg = "invalid userName";
                    logLevel = LogLevel.Warning;
                    context.ErrorResult = new AuthenticationFailureResult("Invalid username or token", request);
                }
                else
                {
                    logLevel = LogLevel.Info;

                    // 6. If the credentials are valid, set principal.
                    context.Principal = principal;
                }

            }
            finally
            {
                string meth = (context.Request == null || context.Request.Method == null) ? null : context.Request.Method.ToString();
                string userId = (context.Principal == null || context.Principal.Identity == null) ? null : context.Principal.Identity.Name;
                string pathQuery = (context.Request == null || context.Request.RequestUri == null) ? null : context.Request.RequestUri.PathAndQuery;

                _log.Log(logLevel, userId, string.Format("--> {0} {1} {2}", meth, pathQuery, logMsg));
            }

        }

        private AuthenticationToken ExtractUserNameAndToken(string parameter)
        {
            if (!parameter.Contains(":"))
                return null;

            string[] unp = parameter.Split(':');
            return new AuthenticationToken(unp[0], unp[1]);
        }

        private GenericPrincipal AuthenticateCredentials(AuthenticationToken authToken)
        {
            if (authToken == null || authToken.UserName == null)
            {
                throw new Exception("authToken or authToken.username is null");
            }
            var authResult = AuthManager.Instance.AuthenticateToken(authToken);
            if (!authResult.IsSuccess)
                return null;
            
            var identity = new GenericIdentity(authResult.UserId.ToString());
            var principal = new GenericPrincipal(identity, null);
            return principal;
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue(SimpleTokenAuthSchema);
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            
            return Task.FromResult(0);
        }
    }

    public class AddChallengeOnUnauthorizedResult : IHttpActionResult
    {
        public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
        {
            Challenge = challenge;
            InnerResult = innerResult;
        }

        public AuthenticationHeaderValue Challenge { get; private set; }

        public IHttpActionResult InnerResult { get; private set; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await InnerResult.ExecuteAsync(cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Only add one challenge per authentication scheme.
                if (!response.Headers.WwwAuthenticate.Any((h) => h.Scheme == Challenge.Scheme))
                {
                    response.Headers.WwwAuthenticate.Add(Challenge);
                }
            }

            return response;
        }
    }

    public class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        public string ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase;
            return response;
        }
    }

       

}
