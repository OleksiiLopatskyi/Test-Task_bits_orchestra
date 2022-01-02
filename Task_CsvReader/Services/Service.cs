using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.IO;
using CsvHelper.Configuration;
using System.Threading.Tasks;
using Task_CsvReader.Models;
using Task_CsvReader.Models.DatabaseContext;

namespace Task_CsvReader.Services
{
    public class Service : IService
    {
        public void AddUsersToDatabase(UsersContext context, List<User> users)
        {
                foreach (var item in users)
                {
                    context.Users.Add(item);
                    context.SaveChanges();
                }
        }

        public async Task<List<User>> GetUsersFromCsvAsync(IFormFile file,string filePath,string Delimiter)
        {
            bool fileExists = File.Exists(filePath);
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter=Delimiter,
            };

            if (fileExists)
            {
                return await ReadFile(filePath,csvConfig);
            }
            else
            {
                await SaveAsync(file,filePath);
                return await ReadFile(filePath,csvConfig);
            }

        }

        private async Task<List<User>> ReadFile(string path, CsvConfiguration csvConfig)
        {
            List<User> users = new List<User>();
            await Task.Run(() =>
            {
                using (StreamReader stream = new StreamReader(path))
                {
                    using (var csvReader = new CsvReader(stream, csvConfig))
                    {
                        csvReader.Context.RegisterClassMap<UsersMap>();
                        csvReader.Read();
                        csvReader.ReadHeader();
                        while (csvReader.Read())
                        {
                            var user = csvReader.GetRecord<User>();
                            users.Add(user);
                        }
                    }
                }
            });
            return users;
        }

        private async Task SaveAsync(IFormFile file, string path)
        {
            using (FileStream stream = File.Create(path))
            {
                await file.CopyToAsync(stream);
            }
        }
        public async Task DeleteFileAsync(string path)
        {
            await Task.Run(() => {
                File.Delete(path);
            });
        }
    }
}
