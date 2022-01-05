using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Task_CsvReader.Models;
using Task_CsvReader.Models.DatabaseContext;
using Task_CsvReader.Services;
using File = Task_CsvReader.Models.File;

namespace Task_CsvReader.Controllers
{
    public class FilesController : Controller
    {
        private readonly UsersContext _db;
        private readonly IService _service;
        private readonly IWebHostEnvironment _environment;
        public FilesController(UsersContext context,IService service,IWebHostEnvironment environment)
        {
            _db = context;
            _service = service;
            _environment = environment;
        }

        // GET: Files
        public async Task<IActionResult> Index()
        {
            return View(await _db.Files.ToListAsync());
        }

        // GET: Files/Details/5

        // GET: Files/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Files/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection form)
        {
            var csvFile = form.Files.First();
            string filePath = $@"{_environment.WebRootPath}\Uploads\{csvFile.FileName}";
            string delimiter = form["delimiter"];
            var users =  await _service.GetUsersFromCsvAsync(csvFile,filePath,delimiter);
            if (users.Count() > 0)
            {

                File file = new File()
                {
                    FileName = form["fileName"],
                    Users = users,
                    DateOfCreation = DateTime.Now
                };
                await _service.AddUsersToDatabase(_db, file);
                await _service.DeleteFileAsync(filePath);
                return Json(new { message = "success" });
            }
            else
            {
                return Json(new { message = "Filed reading file or table is empty" });

            }
        }

        // GET: Files/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _db.Files.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }
            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FileName,DateOfCreation")] File file)
        {
            if (id != file.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(file);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileExists(file.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(file);
        }

        // GET: Files/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _db.Files
                .FirstOrDefaultAsync(m => m.Id == id);
            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var file = await _db.Files.Include(i=>i.Users).FirstOrDefaultAsync(i=>i.Id==id);
            _db.Files.Remove(file);
           _db.Users.RemoveRange(_db.Users.Where(i=>i.FileId==id));

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FileExists(int id)
        {
            return _db.Files.Any(e => e.Id == id);
        }
    }
}
