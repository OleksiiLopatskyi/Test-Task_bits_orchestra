using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using CsvHelper.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Task_CsvReader.Models;
using Task_CsvReader.Models.DatabaseContext;

namespace Task_CsvReader.Services
{
    public interface IService
    {
        Task<List<User>> GetUsersFromCsvAsync(IFormFile file,string path,string Delimiter);
        Task DeleteFileAsync(string path);
        void AddUsersToDatabase(UsersContext context,List<User>users);
    }
}
