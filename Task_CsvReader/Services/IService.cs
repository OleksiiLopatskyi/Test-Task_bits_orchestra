using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using CsvHelper.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Task_CsvReader.Models;

namespace Task_CsvReader.Services
{
    public interface IService
    {
        Task<List<User>> GetUsersFromCsvAsync(IFormFile file,string path,string Delimiter);
    }
}
