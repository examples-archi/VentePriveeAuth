using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using VentePriveeAuth.Entities;
using VentePriveeAuth.Services;

namespace VentePriveeAuth.Handlers
{
    //https://joonasw.net/view/creating-auth-scheme-in-aspnet-core-2
    public class AWSAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string AWSSchemeName = "AWS";

        private readonly IClientService _clientService;
        private readonly IUtilisateurService _utilisateurService;

        public AWSAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUtilisateurService utilisateurService,
            IClientService clientService)
            : base(options, logger, encoder, clock)
        {
            _utilisateurService = utilisateurService;
            _clientService = clientService;
        }

        protected override  async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Vérifier si l'entête "Authorization" existe dans la requête 
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                return AuthenticateResult.NoResult();
            }
            var value = Request.Headers[AuthorizationHeaderName];
            // Parser et valider la valeur de l'entête "Authorisation"
            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue headerValue))
            {
                return AuthenticateResult.NoResult();
            }
            // Vérifier le nom du schéma 
            if (!AWSSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

            Client client = null;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                var credentials = authHeader.Parameter.Split(':');
                if (credentials.Length != 2)
                {
                    return AuthenticateResult.Fail("En-tête d'authentification AWS non valide");
                }
                var accessKeyId = credentials[0];
                var secretAccessKey = credentials[1];
                client = await _clientService.Authorisation(accessKeyId, secretAccessKey);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail("En-tête d'autorisation non valide");
            }

            if (client == null)
                return AuthenticateResult.Fail("accessKeyId ou secretAccessKey non valide");

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
                
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
