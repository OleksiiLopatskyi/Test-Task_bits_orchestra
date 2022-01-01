using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_CsvReader.Models;

namespace Task_CsvReader.Services
{
    public interface IService
    {
        IEnumerable<User> ReadCSV(IFormFile file);
    }
}
