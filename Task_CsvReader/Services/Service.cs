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
        public async Task AddUsersToDatabase(UsersContext context, List<User> users)
        {
                foreach (var item in users)
                {
                    item.ConvertedDateOfBirth = item.BirthDay.ToString("dd-MM-yyyy");
                    context.Users.Add(item);
                }
            await context.SaveChangesAsync();

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

        public async Task<List<User>> SortUsersBy(SortBy SortBy,List<User>users)
        {
            List<User> sortedUsers = new List<User>();
            await Task.Run(() =>
            {
                switch (SortBy)
                {
                    case SortBy.Id:
                        sortedUsers = users.OrderByDescending(i => i.Id).ToList();
                        break;
                    case SortBy.IdDesc:
                        sortedUsers = users.OrderBy(i => i.Id).ToList();
                        break;
                    case SortBy.Name:
                        sortedUsers = users.OrderByDescending(i => i.Name).ToList();
                        break;
                    case SortBy.NameDesc:
                        sortedUsers = users.OrderBy(i => i.Name).ToList();
                        break;
                    case SortBy.BirthDay:
                        sortedUsers = users.OrderByDescending(i => i.BirthDay).ToList();
                        break;
                    case SortBy.BirthDayDesc:
                        sortedUsers = users.OrderBy(i => i.BirthDay).ToList();
                        break;
                    case SortBy.Salary:
                        sortedUsers = users.OrderByDescending(i => i.Salary).ToList();
                        break;
                    case SortBy.SalaryDesc:
                        sortedUsers = users.OrderBy(i => i.Salary).ToList();
                        break;
                    case SortBy.Married:
                        sortedUsers = users.OrderByDescending(i => i.Married).ToList();
                        break;
                    case SortBy.MarriedDesc:
                        sortedUsers = users.OrderBy(i => i.Married).ToList();
                        break;
                    case SortBy.Phone:
                        sortedUsers = users.OrderByDescending(i => i.Phone).ToList();
                        break;
                    case SortBy.PhoneDesc:
                        sortedUsers = users.OrderBy(i => i.Phone).ToList();
                        break;
                }
            });
            return sortedUsers;
        }
    }
}
