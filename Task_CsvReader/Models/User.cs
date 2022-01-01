using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task_CsvReader.Models
{
    public class User
    {
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public string Phone { get; set; }
        public bool Married { get; set; }
        public decimal Salary { get; set; }
    }
}
