using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class AnnouncementController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Duyurular";
            return View("~/Views/Admin/Announcement/Index.cshtml");
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Yeni Duyuru";
            return View("~/Views/Admin/Announcement/Create.cshtml");
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(Guid id)
        {
            ViewData["Title"] = "Duyuru Düzenle";
            return View("~/Views/Admin/Announcement/Edit.cshtml");
        }

        [HttpGet("Details/{id}")]
        public IActionResult Details(Guid id)
        {
            ViewData["Title"] = "Duyuru Detayı";
            return View("~/Views/Admin/Announcement/Details.cshtml");
        }
    }
}