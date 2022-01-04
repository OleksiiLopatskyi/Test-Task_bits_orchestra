using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task_CsvReader.Models
{
    public class File
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime DateOfCreation { get; set; }
        public List<User> Users { get; set; }
        public File()
        {
            Users = new List<User>();
        }
    }
}
