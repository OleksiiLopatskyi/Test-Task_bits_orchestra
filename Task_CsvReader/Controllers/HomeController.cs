using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Task_CsvReader.Models;
using Task_CsvReader.Models.DatabaseContext;
using Task_CsvReader.Services;

namespace Task_CsvReader.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IService _service;
        private readonly IWebHostEnvironment _environment;
        private UsersContext _db;
        public HomeController(ILogger<HomeController> logger,
            IService service,
            IWebHostEnvironment environment,
            UsersContext context)
        {
            _logger = logger;
            _service = service;
            _environment = environment;
            _db = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ShowAvailableFiles()
        {
            var availableFiles = await _db.Files.ToListAsync();
            return Json(new { files = availableFiles });
        }
        public async Task<IActionResult> ShowUsersFromFile(string fileName) {
            var file = await _db.Files.Include(i=>i.Users).FirstOrDefaultAsync(i=>i.FileName==fileName);
            var users = file.Users;
            return Json(new {users=users});
        }
        public async Task<IActionResult> FindUsersInFile(string property,string value,string fileName)
        {
            List<User> foundUsers = new List<User>();
            var file = await _db.Files.Include(i=>i.Users).FirstOrDefaultAsync(i=>i.FileName==fileName);
            switch (property)
            {
                case "Id":
                    var val = Int32.Parse(value);
                    var user = await _db.Users.FirstOrDefaultAsync(i => i.Id == val);
                    foundUsers.Add(user);
                    break;
                case "Name":
                    foundUsers = file.Users.Where(i => i.Name.Contains(value)).ToList();
                    break;
                case "BirthDay":
                    foundUsers = file.Users.Where(i => i.ConvertedDateOfBirth.Contains(value)).ToList();
                    break;
                case "Phone":
                    foundUsers = file.Users.Where(i => i.Phone.Contains(value)).ToList();
                    break;
                case "Married":
                    if (value.Contains("f"))
                        foundUsers = file.Users.Where(i => i.Married == false).ToList();
                    if (value.Contains("t"))
                        foundUsers = file.Users.Where(i => i.Married == true).ToList();
                    break;
                case "Salary":
                    foundUsers = file.Users.Where(i => i.Salary.ToString().Contains(value)).ToList();
                    break;
                default:
                    break;
            }
            return Json(new { users = foundUsers });
        }
        public async Task<IActionResult> ShowUsers()
        {
            var users = await _db.Users.ToListAsync();
            return Json(new {users=users});
        }
        public async Task<IActionResult> Sort(string search,string property,SortBy SortBy,string fileName)
        {
            var file = await _db.Files.Include(i=>i.Users).FirstOrDefaultAsync(i=>i.FileName==fileName);
            var foundUsers = file.Users.ToList();
            var sortedUsers = new List<User>();
            if (search == string.Empty) foundUsers = file.Users.ToList();
            else
            switch (property)
            {
              
                case "Name":
                    foundUsers = file.Users.Where(i => i.Name.Contains(search)).ToList();
                    break;
                case "BirthDay":
                    foundUsers = file.Users.Where(i => i.ConvertedDateOfBirth.Contains(search)).ToList();
                    break;
                case "Phone":
                    foundUsers = file.Users.Where(i => i.Phone.Contains(search)).ToList();
                    break;
                case "Married":
                    if (search.Contains("f"))
                        foundUsers = file.Users.Where(i => i.Married == false).ToList();
                    if (search.Contains("t"))
                        foundUsers = file.Users.Where(i => i.Married == true).ToList();
                    break;
                case "Salary":
                    foundUsers = file.Users.Where(i => i.Salary.ToString().Contains(search)).ToList();
                    break;
                default:
                    break;
            }
            sortedUsers = await _service.SortUsersBy(SortBy,foundUsers);
            return Json(new { users = sortedUsers });
        }
        public async Task<IActionResult> DownloadCSV(string fileName)
        {
            string path = $@"{_environment.WebRootPath}\Uploads\{fileName}.csv";
            byte[] fileBytes = await _service.DownloadCSV(_db,fileName,path);
            // Тип файла - content-type
            string file_type = "application/octet-stream";
            // Имя файла - необязательно
            string file_name = string.Empty;
            if (fileName == null)
                file_name = fileName + "Example.csv";
            else
                file_name = fileName + ".csv";
            return File(fileBytes, file_type, file_name);
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
