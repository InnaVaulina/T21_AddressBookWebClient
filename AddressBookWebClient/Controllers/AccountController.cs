using AddressBookWebClient.Context;
using AddressBookWebClient.Models;
using AddressBookWebClient.ViewData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AddressBookWebClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ClientStoreContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;


        public AccountController(IHttpClientFactory httpClientFactory,
                                IConfiguration configuration,
                                ClientStoreContext context,
                                UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        [Route("Account/Register")]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            using (var client = _httpClientFactory.CreateClient())
                try
                {
                    var url = @$"{_configuration["ApplicationUrl"]}/api/Account/Register";
                    JsonContent content = JsonContent.Create(model);
                    HttpResponseMessage result = await client.PostAsync(url, content);
                    if (((int)result.StatusCode) == 200)
                    {
                        return RedirectToAction("Login", "Account");
                    }

                }
                catch (Exception ex)
                {
                    return NotFound(ex.Message);
                }

            return View(model);
        }



        [HttpGet]
        [Route("Account/Login")]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(EntryViewModel model)
        {

            using (var client = _httpClientFactory.CreateClient())
                try
                {
                    var url = @$"{_configuration["ApplicationUrl"]}/api/Account/Login";                    
                    JsonContent content = JsonContent.Create(model);
                    HttpResponseMessage result = await client.PostAsync(url, content);
                    if (((int)result.StatusCode) == 200) 
                    {
                        UserModel user_param = await result.Content.ReadAsAsync<UserModel>();
                        AppUser user = new AppUser
                        {
                            UserName = user_param.UserName,
                            Token = user_param.Token
                            
                        };

                        var UMresult = await _userManager.CreateAsync(user, model.Password);
                        if (UMresult.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, user_param.UserRole);
                            await _signInManager.SignInAsync(user, false);
                        }
                        else BadRequest();
  
                        var SMresult = await _signInManager.PasswordSignInAsync(model.LoginProp, model.Password, false, false);
                        if (SMresult.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else BadRequest();
                       
                    }

                }
                catch (Exception ex)
                {
                    return NotFound(ex.Message);
                }

            return View(model);

        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            AppUser curUser;
            curUser = await _userManager.GetUserAsync(HttpContext.User);
            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(curUser);
            return RedirectToAction("Index", "Home");
        }




        [HttpGet]
        [Route("Account/AdminPage")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminPage()
        {
            List<UserModelForAdmin> userList;
            using (var client = _httpClientFactory.CreateClient())
                try
                {
                    var url = @$"{_configuration["ApplicationUrl"]}/api/Account/GetUserList";
                    AppUser curUser = await _userManager.GetUserAsync(HttpContext.User);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curUser.Token);

                    HttpResponseMessage result = await client.GetAsync(url);
                    if (((int)result.StatusCode) == 200)
                        userList = await result.Content.ReadAsAsync<List<UserModelForAdmin>>();
                    else userList = new List<UserModelForAdmin>();

                }
                catch (Exception ex)
                {
                    return NotFound(ex.Message);
                }

            return View(userList);
        }




        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(UserModelForAdmin usermodel)
        {
            using (var client = _httpClientFactory.CreateClient())
                try
                {
                    var url = @$"{_configuration["ApplicationUrl"]}/api/Account/DeleteUser?id={usermodel.Id}";
                    var curUser = await _userManager.GetUserAsync(HttpContext.User);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curUser.Token);

                    HttpResponseMessage result = await client.DeleteAsync(url);
                }
                catch (Exception ex)
                {
                    return NotFound(ex.Message);
                }

            return RedirectToAction("AdminPage", "Account");

        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeUserRole(UserModelForAdmin usermodel)
        {
            using (var client = _httpClientFactory.CreateClient())
                try
                {
                    var url = @$"{_configuration["ApplicationUrl"]}/api/Account/ChangeUserRole?id={usermodel.Id}";
                    JsonContent content = JsonContent.Create(new {Role = usermodel.UserRole});
                    var curUser = await _userManager.GetUserAsync(HttpContext.User);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curUser.Token);

                    HttpResponseMessage result = await client.PutAsync(url, content);
                }
                catch (Exception ex)
                {
                    return NotFound(ex.Message);
                }

            return RedirectToAction("AdminPage", "Account");

        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddUser(RegisterViewModelForAdmin model)
        {
            using (var client = _httpClientFactory.CreateClient())
                try
                {
                    var url = @$"{_configuration["ApplicationUrl"]}/api/Account/AddUser";
                    JsonContent content = JsonContent.Create(model);
                    var curUser = await _userManager.GetUserAsync(HttpContext.User);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curUser.Token);

                    HttpResponseMessage result = await client.PostAsync(url, content);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            return RedirectToAction("AdminPage", "Account");
        }
    }

}
