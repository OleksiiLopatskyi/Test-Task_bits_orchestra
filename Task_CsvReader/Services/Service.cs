using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using CsvHelper;
using System.IO;
using CsvHelper.Configuration;
using System.Threading.Tasks;
using Task_CsvReader.Models;

namespace Task_CsvReader.Services
{
    public class Service : IService
    {
        public IEnumerable<User> ReadCSV(IFormFile file)
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter=";"
            };
            using(var fileReader = new StreamReader(file.OpenReadStream()))
            {
                using(var csvReader = new CsvReader(fileReader, csvConfig))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();
                    while (csvReader.Read())
                    {
                        var user = csvReader.GetRecord<User>();
                        yield return user;
                    }
                }
            }
        }
    }
}
