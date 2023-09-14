using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace RESTServiceProject
{
    public class AuthenticatorAttribute : TypeFilterAttribute
    {
        public AuthenticatorAttribute() : base(typeof(AuthenticatorActionFilter))
        {
        }
    }

    public class AuthenticatorActionFilter : IAuthorizationFilter
    {
        private const string BEARER = "Bearer ";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                bool valid = false;

                var request = context.HttpContext.Request;

                var authorization = request.Headers["Authorization"].ToString();
                if (authorization.StartsWith(BEARER))
                {
                    var tokenString = authorization.Substring(BEARER.Length).Trim();
                    var token = TokenHelper.DecodeToken(tokenString);
                    if (token.Expires > DateTime.UtcNow)
                    {
                        valid = true;
                    }
                }

                if (!valid)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
