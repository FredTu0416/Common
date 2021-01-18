using System;
using System.Collections.Generic;
using System.Text;
using CommonService.Excel;

namespace DemoConsole.Models.OjbectCreate
{
    public class ClassA
    {
        [ExcelEntity(Field = "name")]
        public string Name { get; set; }

        [ExcelEntity(Field = "id")]
        public string ID { get; set; }
    }
}
