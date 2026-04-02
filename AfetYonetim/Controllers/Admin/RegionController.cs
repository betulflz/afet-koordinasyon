using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class RegionController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Bölge Yönetimi";
            return View("~/Views/Admin/Regions/Index.cshtml");
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Yeni Bölge";
            return View("~/Views/Admin/Regions/Create.cshtml");
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(Guid id)
        {
            ViewData["Title"] = "Bölge Düzenle";
            return View("~/Views/Admin/Regions/Edit.cshtml");
        }
    }
}