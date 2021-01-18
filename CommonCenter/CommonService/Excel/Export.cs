using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;

namespace CommonService.Excel
{
    public class Export
    {
        private IWorkbook _workbook;
        private List<string> sheetNames = new List<string>();

        public Export(Common.ExcelVersion excelVersion)
        {
            initWorkBook(excelVersion);
        }

        private void initWorkBook(Common.ExcelVersion excelVersion)
        {
            switch (excelVersion)
            {
                case Common.ExcelVersion.Excel2003:
                    _workbook = new NPOI.HSSF.UserModel.HSSFWorkbook();
                    break;
                case Common.ExcelVersion.Excel2007:
                    _workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
                    break;
                case Common.ExcelVersion.NotSupported:
                default:
                    throw new NotSupportedException("Excel version not supported");
            }
        }

        private bool sheetNameExists(string sheetName)
        {
            bool isExists = false;
            for (int i = 0; i < this._workbook.NumberOfSheets; i++)
            {
                ISheet sheet = this._workbook.GetSheetAt(i);
                if (sheetName.Equals(sheet.SheetName, StringComparison.OrdinalIgnoreCase))
                {
                    isExists = true;
                    break;
                }
            }
            return isExists;
        }

        public void Sheet<T>(List<T> entities)
        {
            var sheetName = "Sheet" + sheetNames.Count.ToString();

            this.Sheet<T>(sheetName, entities);
        }

        public void Sheet<T>(string sheetName, List<T> entities)
        {
            if (sheetNameExists(sheetName))
                throw new Exception($"SheetName exists {sheetName}");

            ISheet sheet = this._workbook.CreateSheet(sheetName);
            //first row
            IRow firstRow = sheet.CreateRow(0);

            var type = typeof(T);
            var properties = type.GetProperties();

            Dictionary<PropertyInfo, Common.AttrProp> mapping = new Dictionary<PropertyInfo, Common.AttrProp>();

            for (int i = 0; i < properties.Length; i++)
            {
                var excelEntity = properties[i].GetCustomAttribute<ExcelEntityAttribute>();
                var fieldName = excelEntity.Field;
                mapping.Add(properties[i], new Common.AttrProp()
                {
                    FieldName = fieldName,
                    Index = i,
                    IsRequired = excelEntity.IsRequired
                });

                firstRow.CreateCell(i).SetCellValue(fieldName);
            }

            for (int i = 0; i < entities.Count; i++)
            {
                T rowData = entities[i];
                int row_index = i + 1;

                IRow row = sheet.CreateRow(row_index);
                foreach (var item in mapping)
                {
                    row.CreateCell(item.Value.Index).SetCellValue("");
                }
            }
        }

        public async Task<byte[]> Buffer()
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                _workbook.Write(ms);
                ms.Position = 0;
                byte[] buffer = new byte[ms.Length];
                await ms.ReadAsync(buffer, 0, buffer.Length);
                _workbook.Close();
                return buffer;
            }
        }
    }
}
