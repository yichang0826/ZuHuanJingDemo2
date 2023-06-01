using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using ZuHuanJingDemo2.Models;

namespace ZuHuanJingDemo2.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login() { return View(); }

        [HttpPost]
        
        public async Task<IActionResult> Login(string username, string password)
        {
            string connectionString = _configuration.GetConnectionString("ZuHuanJingDemo2Context");
            string memberAccount = "";
            try
            { 
                using MySqlConnection connection = new(connectionString);
                await connection.OpenAsync();
                string selectQuery = "SELECT * FROM `member` WHERE `Member_account` = @username AND `Member_Password` = @password";
                using MySqlCommand command = new(selectQuery, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                using MySqlDataReader reader = command.ExecuteReader();
                while (await reader.ReadAsync())
                {
                    memberAccount = reader.GetString("Member_Account");
                    break;
                }
            }
            catch (Exception)
            {
                ViewData["Danger"] = "Invald Email or Password";
                return View();
            }

            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, memberAccount)
                };
                var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = true,
                    AllowRefresh = false
                };
                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);
            }
            catch (Exception) { return NotFound(); }
            return RedirectToAction("Index", "Members");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}