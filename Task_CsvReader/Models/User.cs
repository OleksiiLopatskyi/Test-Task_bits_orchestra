using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration.Attributes;
using System.Threading.Tasks;

namespace Task_CsvReader.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public string ConvertedDateOfBirth { get; set; } = string.Empty;
        public string Phone { get; set; }
        public bool Married { get; set; }
        public decimal Salary { get; set; }
        public int FileId { get; set; }
    }
}
