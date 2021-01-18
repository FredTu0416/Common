using System;
using System.Collections.Generic;
using System.Text;

namespace CommonService.Excel
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ExcelEntityAttribute : Attribute
    {
        // This is a positional argument
        public ExcelEntityAttribute()
        {
        }

        public string Field { get; set; }
        public bool IsRequired { get; set; } = false;
    }
}
