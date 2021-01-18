using System;
using System.Collections.Generic;
using System.Text;
using CommonService.Excel;

namespace DemoConsole.Models.OjbectCreate
{
    public class ClassA
    {
        [ExcelEntity(Field = "姓名")]
        public string Name { get; set; }

        public string ID { get; set; }
    }
}
