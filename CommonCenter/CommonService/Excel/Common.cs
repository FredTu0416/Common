using System;
using System.Collections.Generic;
using System.Text;

namespace CommonService.Excel
{
    public class Common
    {
        public enum ExcelVersion
        {
            Excel2003,
            Excel2007,
            NotSupported
        }

        public struct AttrProp
        {
            public int Index { get; set; }
            public bool IsRequired { get; set; }
            public string FieldName { get; set; }
        }
    }
}
