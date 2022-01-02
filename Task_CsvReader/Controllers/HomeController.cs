using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Task_CsvReader.Models;
using Task_CsvReader.Services;

namespace Task_CsvReader.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IService _service;
        private readonly IWebHostEnvironment _environment;
        public HomeController(ILogger<HomeController> logger,
            IService service,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _service = service;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UploadCSV(IFormCollection form)
        {
            var uploadedFile = form.Files["csvFile"];
            string path = $@"{_environment.WebRootPath}\Uploads\{uploadedFile.FileName}";
            var uploadedUsers = await _service.GetUsersFromCsvAsync(uploadedFile,path,";");
            return Json(new {message="success"});
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
