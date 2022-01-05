using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using System.Globalization;

namespace Task_CsvReader.Models
{
    public class UsersMap:ClassMap<User>
    {
        public UsersMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(i => i.Id).Ignore();
            Map(i => i.ConvertedDateOfBirth).Ignore();
            Map(i => i.FileId).Ignore();

        }
    }
}
