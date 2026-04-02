using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AfetYonetim.Controllers.Afetzede
{
    [Authorize(Roles = "Afetzede")]
    [Route("Afetzede/[controller]")]
    public class HelpRequestController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Taleplerim";
            return View("~/Views/Afetzede/HelpRequest/Index.cshtml");
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Yeni Talep Oluştur";
            return View("~/Views/Afetzede/HelpRequest/Create.cshtml");
        }
        
        [HttpGet("Details/{id}")]
        public IActionResult Details(Guid id)
        {
            ViewData["Title"] = "Talep Detayı";
            return View("~/Views/Afetzede/HelpRequest/Details.cshtml");
        }
    }
}