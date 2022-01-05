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
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Task_CsvReader.Services
{
    public class Service : IService
    {
        public async Task AddUsersToDatabase(UsersContext context,Models.File file)
        {
            context.Files.Add(file);
            await context.SaveChangesAsync();

        }

        public async Task<List<User>> GetUsersFromCsvAsync(IFormFile file,string filePath,string Delimiter)
        {
            bool fileExists = System.IO.File.Exists(filePath);
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
                        try
                        {
                            csvReader.Context.RegisterClassMap<UsersMap>();
                            csvReader.Read();
                            csvReader.ReadHeader();
                            while (csvReader.Read())
                            {
                                var user = csvReader.GetRecord<User>();
                                user.ConvertedDateOfBirth = user.BirthDay.ToString("dd-mm-yyyy");
                                users.Add(user);
                            }
                        }catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                       
                    }
                    stream.Close();
                }
            });
            return users;
        }

        private async Task SaveAsync(IFormFile file, string path)
        {
            using (FileStream stream = System.IO.File.Create(path))
            {
                await file.CopyToAsync(stream);
            }
        }
        public async Task DeleteFileAsync(string path)
        {
            await Task.Run(() => {
                System.IO.File.Delete(path);
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

        public async Task<byte[]> DownloadCSV(UsersContext context,string fileName,string path)
        {
            var file = await context.Files.Include(i=>i.Users).FirstOrDefaultAsync(i=>i.FileName==fileName);
           
            if (file == null)
            {
                path = $@"D:\Test-Task_bits_orchestra\Task_CsvReader\wwwroot\Uploads\Example.csv";

                using (FileStream fs = System.IO.File.Create(path))
                {
                    fs.Flush();
                }
                using (var stream = new StreamWriter(path))
                {
                    using (CsvWriter writer = new CsvWriter(stream, CultureInfo.InvariantCulture))
                    {
                        writer.Context.RegisterClassMap<UsersMap>();
                        writer.WriteHeader<User>();
                    }
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                System.IO.File.Delete(path);
                return fileBytes;
            }
            else
            {
                var users = file.Users;
                using (var stream = new StreamWriter(path))
                {
                    using (CsvWriter writer = new CsvWriter(stream, CultureInfo.InvariantCulture))
                    {
                        writer.Context.RegisterClassMap<UsersMap>();
                        writer.WriteRecords(users);
                    }
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                System.IO.File.Delete(path);
                return fileBytes;
            }
        }
    }
}
