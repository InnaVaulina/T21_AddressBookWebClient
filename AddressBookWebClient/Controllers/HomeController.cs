using AddressBookWebClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;


namespace AddressBookWebClient.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        
        public HomeController(IHttpClientFactory httpClientFactory,
                                UserManager<AppUser> userManager,
                                IConfiguration configuration
                                )
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _configuration = configuration;
        }


        [HttpGet("{letter?}")]
        [Route("Home/Index")]
        [AllowAnonymous]
        public async Task<IActionResult> Index() 
        {
            if (this.Request.Cookies["action"] != null) 
            {
                if (this.Request.Cookies["action"] == "RetrievalByLetter")
                {
                    string letter = this.Request.Cookies["letter"];
                    int page = int.Parse(this.Request.Cookies["page"]);
                    return await RetrievalByLetter(letter, page);
                }
                if (this.Request.Cookies["action"] == "RetrievalByClue")
                {
                    string clue = this.Request.Cookies["clue"];
                    int page = int.Parse(this.Request.Cookies["page"]);
                    return await RetrievalByClue(clue, page);
                }
            }
            return await RetrievalByLetter("all", 0);
        }





        [HttpGet]
        [Route("Home/RetrievalByLetter")]
        [AllowAnonymous]
        public async Task<IActionResult> RetrievalByLetter(string letter, int trend)
        {                        
            int page = (trend < 0) ? 0 : trend;
            List<Note> notelist = new List<Note>();
            HttpResponseMessage result;
            string url = @$"{_configuration["ApplicationUrl"]}/api/Home/GetByTheLetter?letter={letter}&page={page}";
            try 
            {
                result = await Retrieval(url);
            }
            catch (Exception ex) 
            {
                return NotFound(ex.Message);
            }

            if (((int)result.StatusCode) == 200)
            {
                notelist = await result.Content.ReadAsAsync<List<Note>>();
                if (notelist.Count == 0 && page > 0)
                {
                    page--;
                    url = @$"{_configuration["ApplicationUrl"]}/api/Home/GetByTheLetter?letter={letter}&page={page}";
                    try
                    {
                        result = await Retrieval(url);
                    }
                    catch (Exception ex)
                    {
                        return NotFound(ex.Message);
                    }
                    if (((int)result.StatusCode) == 200)
                    {
                        notelist = await result.Content.ReadAsAsync<List<Note>>();
                    }
                }
            }
            
            

            ViewData["Letter"] = letter;
            ViewData["Clue"] = "";
            ViewData["Page"] = page;

            this.Response.Cookies.Append("action", "RetrievalByLetter");
            this.Response.Cookies.Append("letter", letter);
            this.Response.Cookies.Append("page", page.ToString());

            return View("Index", notelist);
        }


        [HttpGet]
        [Route("Home/RetrievalByClue")]
        [AllowAnonymous]
        public async Task<IActionResult> RetrievalByClue(string clue, int trend)
        {
            int page = (trend > 0) ? trend : 0;
            List<Note> notelist = new List<Note>();
            HttpResponseMessage result;
            string url = @$"{_configuration["ApplicationUrl"]}/api/Home/GetByTheClue?clue={clue}&page={page}";
            try
            {
                result = await Retrieval(url);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            if (((int)result.StatusCode) == 200)
            {
                notelist = await result.Content.ReadAsAsync<List<Note>>();
                if (notelist.Count == 0 && page > 0)
                {
                    page--;
                    url = @$"{_configuration["ApplicationUrl"]}/api/Home/GetByTheClue?clue={clue}&page={page}";
                    try
                    {
                        result = await Retrieval(url);
                    }
                    catch (Exception ex)
                    {
                        return NotFound(ex.Message);
                    }
                    if (((int)result.StatusCode) == 200)
                    {
                        notelist = await result.Content.ReadAsAsync<List<Note>>();
                    }
                }
            }

            ViewData["Letter"] = "";
            ViewData["Clue"] = clue;
            ViewData["Page"] = page;

            this.Response.Cookies.Append("action", "RetrievalByClue");
            this.Response.Cookies.Append("clue", clue);
            this.Response.Cookies.Append("page", page.ToString());


            return View("Index",notelist);
        }


        [HttpPost]
        public async Task<IActionResult> AddNote(Note note)
        {
            using (var client = _httpClientFactory.CreateClient())
                try
                {
                    var url = @$"{_configuration["ApplicationUrl"]}/api/Home/AddNote";
                    JsonContent content = JsonContent.Create(note);
                    var curUser = await _userManager.GetUserAsync(HttpContext.User);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curUser.Token);
                    
                    HttpResponseMessage result = await client.PostAsync(url, content);
                }
                catch (Exception ex)
                {
                    return NotFound(ex.Message);
                }

            return RedirectToAction("Index");
        }



        public async Task<HttpResponseMessage> Retrieval(string url)
        {
            using (var client = _httpClientFactory.CreateClient())
                try
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        AppUser curUser = null;
                        curUser = await _userManager.GetUserAsync(HttpContext.User);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curUser.Token);
                    }
                    return await client.GetAsync(url);
                }
                catch (Exception ex)
                {

                }
            return null;
        }


    }
}
