using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;

namespace CommonService.Excel
{
    public class Analysis
    {
        private Common.ExcelVersion _excelType;
        private IWorkbook _workbook;
        private Stopwatch _stopwatch;

        public delegate void AnalysisMsgDelegate(string msg, MessageWriteType writeType, Nullable<ConsoleColor> consoleColor = null, Nullable<int> coverLine = null);

        public event AnalysisMsgDelegate AnalysisMessage;

        public Analysis(string path)
        {
            _excelType = Common.ExcelVersion.NotSupported;
            var fileInfo = initFileAccess(path);
            initStreamAccess(fileInfo);

            if (_workbook == null)
                throw new ArgumentNullException($"IWorkBook init failed.");
        }

        public async Task<List<T>> SheetAsync<T>(string sheetName) where T : class
        {
            var task1 = new TaskFactory<List<T>>().StartNew(() =>
            {
                AnalysisMessage?.Invoke($"Start to analyzing Sheet - {sheetName}.", MessageWriteType.NewLine, ConsoleColor.Red);
                _stopwatch.Start();
                var resp = Sheet<T>(sheetName);
                _stopwatch.Stop();
                AnalysisMessage?.Invoke($"{sheetName} analysis completed. Total Cost: {_stopwatch.ElapsedMilliseconds}ms", MessageWriteType.NewLine, ConsoleColor.Green);
                return resp;
            });

            return await task1;
        }

        public List<T> Sheet<T>(string sheetName) where T : class
        {
            ISheet sheet = getSheetByName(sheetName);

            IRow firstRow = sheet.GetRow(0);

            if (firstRow.Cells.Count == 0)
                throw new ArgumentNullException($"Excel sheet {sheetName} Line 1 is empty");

            var cusAttrDict = initExcelAttribute<T>();
            var mapping = initMapping(firstRow, cusAttrDict);

            AnalysisMessage?.Invoke($"Total Row: {sheet.LastRowNum}.", MessageWriteType.NewLine);
            List<T> entities = new List<T>();

            var topCursor = Console.CursorTop;
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                AnalysisMessage?.Invoke($"line {i}", MessageWriteType.NewLine, ConsoleColor.Green, topCursor);
                IRow row = sheet.GetRow(i);
                T entity = initProperty<T>(row, mapping);
                entities.Add(entity);
            }

            return entities;
        }

        #region Private

        private T initProperty<T>(IRow row, Dictionary<PropertyInfo, Common.AttrProp> mapping) where T : class
        {
            var _instance = Assembly.CreateInstance<T>()();

            foreach (var item in mapping)
            {
                var cell = row.GetCell(item.Value.Index, MissingCellPolicy.RETURN_BLANK_AS_NULL);
                if (cell is null)
                    continue;

                var isRequired = item.Value.IsRequired;
                var cellTypeName = row.GetCell(item.Value.Index, MissingCellPolicy.CREATE_NULL_AS_BLANK).CellType.ToString();
                var propertyTypeName = item.Key.PropertyType.Name;

                if (propertyTypeName == typeof(int).Name || propertyTypeName == typeof(double).Name ||
                    propertyTypeName == typeof(float).Name || propertyTypeName == typeof(decimal).Name)
                {
                    if (!cellTypeName.Equals("numeric", StringComparison.OrdinalIgnoreCase))
                        throw new ArgumentException($"Excel column data type was wrong. At row {row.RowNum + 1}, column {item.Value.Index + 1}");
                }
                if (propertyTypeName == typeof(bool).Name)
                {
                    if (!cellTypeName.Equals("boolean", StringComparison.OrdinalIgnoreCase))
                        throw new ArgumentException($"Excel column data type was wrong. At row {row.RowNum + 1}, column {item.Value.Index + 1}");
                }

                if (cellTypeName == "String")
                {
                    var v = row.GetCell(item.Value.Index);
                    if (isRequired && (v == null || string.IsNullOrEmpty(v.StringCellValue)))
                        throw new ArgumentNullException($"Data is required. At row {row.RowNum + 1}, column {item.Value.Index + 1}");
                    Assembly.SetValue(item.Key)(_instance, v.StringCellValue);
                }
                else if (cellTypeName == "Boolean")
                {
                    var v = row.GetCell(item.Value.Index);
                    if (isRequired && v == null)
                        throw new ArgumentNullException($"Data is required. At row {row.RowNum + 1}, column {item.Value.Index + 1}");

                    Assembly.SetValue(item.Key)(_instance, v.BooleanCellValue);
                }
                else if (cellTypeName == "Numeric")
                {
                    var v = row.GetCell(item.Value.Index);
                    if (isRequired && v == null)
                        throw new ArgumentNullException($"Data is required. At row {row.RowNum + 1}, column {item.Value.Index + 1}");

                    if (DateUtil.IsCellDateFormatted(row.Cells[item.Value.Index]))
                        Assembly.SetValue(item.Key)(_instance, v.DateCellValue);
                    else
                    {
                        if (item.Key.PropertyType.Name == typeof(int).Name)
                            Assembly.SetValue(item.Key)(_instance, (int)v.NumericCellValue);
                        else if (item.Key.PropertyType.Name == typeof(double).Name)
                            Assembly.SetValue(item.Key)(_instance, (double)v.NumericCellValue);
                        else if (item.Key.PropertyType.Name == typeof(float).Name)
                            Assembly.SetValue(item.Key)(_instance, (float)v.NumericCellValue);
                        else if (item.Key.PropertyType.Name == typeof(decimal).Name)
                            Assembly.SetValue(item.Key)(_instance, (decimal)v.NumericCellValue);
                        else if (item.Key.PropertyType.Name == typeof(string).Name)
                            Assembly.SetValue(item.Key)(_instance, v.NumericCellValue.ToString());
                    }
                }
                else
                    throw new NotSupportedException($"Set value to instance. PropertyType not supported {cellTypeName}");
            }

            return _instance;
        }

        private Dictionary<PropertyInfo, Common.AttrProp> initMapping(IRow row, Dictionary<PropertyInfo, Dictionary<string, object>> cusAttrDict)
        {
            Dictionary<PropertyInfo, Common.AttrProp> mapping = new Dictionary<PropertyInfo, Common.AttrProp>();
            List<ICell> cells = row.Cells;
            for (int i = 0; i < cells.Count; i++)
            {
                var columnName = cells[i].StringCellValue;

                var attrInfo = FindTarget(cusAttrDict, columnName);
                if (!attrInfo.HasValue)
                    throw new NotSupportedException($"Column {columnName} was not define");

                mapping.Add(attrInfo.Value.Key,
                    new Common.AttrProp()
                    {
                        Index = i,
                        IsRequired = (bool)attrInfo.Value.Value["IsRequired"]
                    });
            }

            return mapping;
        }

        private KeyValuePair<PropertyInfo, Dictionary<string, object>>? FindTarget(Dictionary<PropertyInfo, Dictionary<string, object>> attributeSettings, string columnName)
        {
            foreach (var item in attributeSettings)
            {
                if (columnName.Equals(item.Value["Field"].ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        private Dictionary<PropertyInfo, Dictionary<string, object>> initExcelAttribute<T>() where T : class
        {
            var type = typeof(T);
            Dictionary<PropertyInfo, Dictionary<string, object>> dict = new Dictionary<PropertyInfo, Dictionary<string, object>>();
            foreach (var item in type.GetProperties())
            {
                var excelAttribute = item.GetCustomAttribute<Excel.ExcelEntityAttribute>();
                Dictionary<string, object> attrDict = new Dictionary<string, object>();
                attrDict.Add(nameof(excelAttribute.Field), excelAttribute.Field);
                attrDict.Add(nameof(excelAttribute.IsRequired), excelAttribute.IsRequired);
                dict.Add(item, attrDict);
            }

            return dict;
        }

        private ISheet getSheetByName(string sheetName)
        {
            ISheet sheet = null;
            for (int i = 0; i < this._workbook.NumberOfSheets; i++)
            {
                ISheet _sheet = this._workbook.GetSheetAt(i);
                if (_sheet.SheetName.Trim().Equals(sheetName, StringComparison.OrdinalIgnoreCase))
                {
                    sheet = _sheet;
                    break;
                }
            }
            if (sheet == null)
            {
                AnalysisMessage?.Invoke($"Sheet - {sheetName} dose not exist.", MessageWriteType.NewLine);
                throw new ArgumentNullException($"Sheet not found: {sheetName}");
            }
            return sheet;
        }

        private void initStreamAccess(System.IO.FileInfo fileInfo)
        {
            System.IO.Stream stream = fileInfo.OpenRead();
            System.IO.MemoryStream mStream = new System.IO.MemoryStream();
            stream.CopyTo(mStream);
            mStream.Position = 0;

            if (_excelType == Common.ExcelVersion.Excel2003)
                this._workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(mStream);
            else if (_excelType == Common.ExcelVersion.Excel2007)
                this._workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(mStream);

            stream.Close();
            stream.Dispose();
        }

        private System.IO.FileInfo initFileAccess(string path)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
            if (!fileInfo.Exists)
                throw new System.IO.FileNotFoundException($"In path {path}");

            var extension = fileInfo.Extension;

            if (".xls".Equals(extension, StringComparison.OrdinalIgnoreCase))
                _excelType = Common.ExcelVersion.Excel2003;
            else if (".xlsx".Equals(extension, StringComparison.OrdinalIgnoreCase))
                _excelType = Common.ExcelVersion.Excel2007;
            else
                throw new NotSupportedException($"File type not supported: {extension}");

            return fileInfo;
        }

        #endregion Private

        #region Enum

        public enum MessageWriteType
        {
            InLine = 0x01,
            NewLine = 0x02
        }

        #endregion Enum
    }
}