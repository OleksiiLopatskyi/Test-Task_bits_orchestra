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

        public async Task<IActionResult> UploadCSV(IFormCollection form)
        {
            var uploadedFile = form.Files["csvFile"];
            string path = $@"{_environment.WebRootPath}\Uploads\{uploadedFile.FileName}";
            var uploadedUsers = await _service.GetUsersFromCsvAsync(uploadedFile,path,";");
            await _service.AddUsersToDatabase(_db,uploadedUsers);
            await _service.DeleteFileAsync(path);
            return Json(new {message="success"});
        }
        public async Task<IActionResult> ShowUsers()
        {
            var users = await _db.Users.ToListAsync();
            return Json(new {users=users});
        }
        public async Task<IActionResult> Sort(string search,string property,SortBy SortBy)
        {

            var foundUsers = await _db.Users.ToListAsync();
            switch (property)
            {
              
                case "Name":
                    foundUsers = await _db.Users.Where(i => i.Name.Contains(search)).ToListAsync();
                    break;
                case "BirthDay":
                    foundUsers = await _db.Users.Where(i => i.ConvertedDateOfBirth.Contains(search)).ToListAsync();
                    break;
                case "Phone":
                    foundUsers = await _db.Users.Where(i => i.Phone.Contains(search)).ToListAsync();
                    break;
                case "Married":
                    if (search.Contains("f"))
                        foundUsers = await _db.Users.Where(i => i.Married == false).ToListAsync();
                    if (search.Contains("t"))
                        foundUsers = await _db.Users.Where(i => i.Married == true).ToListAsync();
                    break;
                case "Salary":
                    foundUsers = await _db.Users.Where(i => i.Salary.ToString().Contains(search)).ToListAsync();
                    break;
                default:
                    break;
            }
            var sortedUsers = await _service.SortUsersBy(SortBy,foundUsers);
            return Json(new { users = sortedUsers });
        }
        public async Task<IActionResult> Search(string property, string value)
        {
            List<User> foundUsers = new List<User>();
            switch (property)
            {
                case "Id":
                    var val = Int32.Parse(value);
                    var user = await _db.Users.FirstOrDefaultAsync(i => i.Id == val);
                    foundUsers.Add(user);
                    break;
                case "Name":
                    foundUsers = await _db.Users.Where(i=>i.Name.Contains(value)).ToListAsync();
                    break;
                case "BirthDay":
                    foundUsers = await _db.Users.Where(i => i.ConvertedDateOfBirth.Contains(value)).ToListAsync();
                    break;
                case "Phone":
                    foundUsers = await _db.Users.Where(i => i.Phone.Contains(value)).ToListAsync();
                    break;
                case "Married":
                    if(value.Contains("f"))
                    foundUsers = await _db.Users.Where(i => i.Married==false).ToListAsync();
                    if(value.Contains("t"))
                    foundUsers = await _db.Users.Where(i => i.Married==true).ToListAsync();
                    break;
                case "Salary":
                    foundUsers = await _db.Users.Where(i => i.Salary.ToString().Contains(value)).ToListAsync();
                    break;
                default:
                    break;
            }
            return Json(new {users=foundUsers});
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
