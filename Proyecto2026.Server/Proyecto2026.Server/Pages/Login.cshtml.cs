using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Proyecto2026.Server.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _config;

        public LoginModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public string Usuario { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect("/");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            var expectedUser = _config["Auth:Usuario"];
            var expectedPass = _config["Auth:Password"];

            if (string.IsNullOrWhiteSpace(expectedUser) || string.IsNullOrWhiteSpace(expectedPass))
            {
                ErrorMessage = "Las credenciales no están configuradas en user-secrets (Auth:Usuario / Auth:Password).";
                return Page();
            }

            var userMatches = string.Equals(Usuario?.Trim(), expectedUser, StringComparison.OrdinalIgnoreCase);
            var passBytes = Encoding.UTF8.GetBytes(Password ?? string.Empty);
            var expectedBytes = Encoding.UTF8.GetBytes(expectedPass);
            var passMatches = passBytes.Length == expectedBytes.Length &&
                              CryptographicOperations.FixedTimeEquals(passBytes, expectedBytes);

            if (!userMatches || !passMatches)
            {
                ErrorMessage = "Usuario o contraseña incorrectos.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, expectedUser)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            var target = !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
            return LocalRedirect(target);
        }
    }
}
