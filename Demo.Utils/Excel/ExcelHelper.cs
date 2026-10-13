using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Demo.Utils.Excel
{
    /// <summary>
    /// Excel 导入导出助手（基于 NPOI，xlsx）
    /// </summary>
    public static class ExcelHelper
    {
        /// <summary>
        /// 将对象集合导出为 Excel 字节流
        /// </summary>
        /// <typeparam name="T">行数据类型</typeparam>
        /// <param name="list">数据集合</param>
        /// <param name="headers">表头显示名（与属性顺序一致），为空则使用属性名</param>
        public static byte[] Export<T>(IEnumerable<T> list, params string[] headers)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 表头
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < props.Length; i++)
            {
                string title = (headers != null && i < headers.Length && !string.IsNullOrEmpty(headers[i]))
                    ? headers[i]
                    : props[i].Name;
                headerRow.CreateCell(i).SetCellValue(title);
            }

            // 数据行
            int rowIndex = 1;
            foreach (var item in list)
            {
                IRow row = sheet.CreateRow(rowIndex++);
                for (int i = 0; i < props.Length; i++)
                {
                    object value = props[i].GetValue(item);
                    row.CreateCell(i).SetCellValue(value?.ToString() ?? string.Empty);
                }
            }

            using var ms = new MemoryStream();
            workbook.Write(ms, leaveOpen: true);
            return ms.ToArray();
        }

        /// <summary>
        /// 读取 Excel 第一个 Sheet 为 DataTable
        /// </summary>
        public static DataTable Read(Stream stream, int headerRowIndex = 0)
        {
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheetAt(0);
            var table = new DataTable(sheet.SheetName);

            IRow headerRow = sheet.GetRow(headerRowIndex);
            if (headerRow == null) return table;

            for (int c = 0; c < headerRow.LastCellNum; c++)
            {
                string columnName = headerRow.GetCell(c)?.ToString() ?? $"Column{c}";
                if (table.Columns.Contains(columnName)) columnName = $"{columnName}_{c}";
                table.Columns.Add(columnName);
            }

            for (int r = headerRowIndex + 1; r <= sheet.LastRowNum; r++)
            {
                IRow row = sheet.GetRow(r);
                if (row == null) continue;

                DataRow dataRow = table.NewRow();
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    dataRow[c] = row.GetCell(c)?.ToString() ?? string.Empty;
                }
                table.Rows.Add(dataRow);
            }

            return table;
        }
    }
}
