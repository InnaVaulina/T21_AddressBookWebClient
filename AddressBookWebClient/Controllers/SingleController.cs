using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AddressBookWebClient.Models;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;

namespace AddressBookWebClient.Controllers
{
    [Authorize]
    public class SingleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public SingleController(IHttpClientFactory httpClientFactory,
                                UserManager<AppUser> userManager,
                                IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _configuration = configuration;
        }



        [HttpGet("{tab}/{id}")]
        [Route("Single/Single")]
        [AllowAnonymous]
        public async Task<IActionResult> Single(string tab, int id)
        {
            if (tab == null || id < 0)
                return NotFound();

            Note note;

            using (var client = _httpClientFactory.CreateClient())
                try
                {
                    var url = @$"{_configuration["ApplicationUrl"]}/api/Single/SingleNote?id={id}";

                    if (User.Identity.IsAuthenticated)
                    {
                        var curUser = await _userManager.GetUserAsync(HttpContext.User);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curUser.Token);
                    }

                    HttpResponseMessage result = await client.GetAsync(url);

                    if (((int)result.StatusCode) == 200)
                        note = await result.Content.ReadAsAsync<Note>();
                    else return NotFound();
                }
                catch (Exception ex)
                {
                    return NotFound(ex.Message);
                }

            ViewData["Tab"] = tab;

            return View(note);
        }





        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeNote(int id, [FromBody] Note note)
        {
            using (var client = _httpClientFactory.CreateClient())
                try
                {
                    var url = @$"{_configuration["ApplicationUrl"]}/api/Single/ChangeNote?id={id}";

                    JsonContent content = JsonContent.Create(note);

                    if (User.Identity.IsAuthenticated)
                    {
                        var curUser = await _userManager.GetUserAsync(HttpContext.User);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curUser.Token);
                    }

                    HttpResponseMessage result = await client.PutAsync(url, content);

                }
                catch (Exception ex)
                {
                    return NotFound(ex.Message);
                }

            return RedirectToActionPermanent("Single", new { tab = "Details", id = id });

        }


        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteNote(int id)
        {

            using (var client = _httpClientFactory.CreateClient())
                try
                {
                    var url = @$"{_configuration["ApplicationUrl"]}/api/Single/DeleteNote?id={id}";

                    if (User.Identity.IsAuthenticated)
                    {
                        var curUser = await _userManager.GetUserAsync(HttpContext.User);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curUser.Token);
                    }

                    HttpResponseMessage result = await client.DeleteAsync(url);

                }
                catch (Exception ex)
                {
                    return NotFound(ex.Message);
                }

            return RedirectToAction("Index", "Home");
        }

    }

}
