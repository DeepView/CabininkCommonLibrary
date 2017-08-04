using System;
using System.Threading;
using System.Data.OleDb;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
namespace Cabinink.DataTreatment.DocumentData
{
   /// <summary>
   /// Excel电子表格数据操作类，这个类只适用于数据操作，不适用于数据格式化。
   /// </summary>
   [Serializable]
   [ComVisible(true)]
   public sealed class SpreadsheetDataOperator
   {
      /// <summary>
      /// 从DataTable导出Excel，并指定列别名。
      /// </summary>
      /// <param name="dataTable">数据的导出来源。</param>
      /// <param name="excelFileUrl">含Excel名称的保存路径，在pathType＝1时有效，其它请赋值空字符串。</param>
      /// <param name="pathType">路径类型，允许的取值：0客户自定义路径；1服务端定义路径，标识文件保存路径是服务端指定还是客户自定义路径及文件名。</param>
      /// <param name="colName">各列的列名List。</param>
      /// <param name="sheetName">第一张工作薄的名称，为空字符串时保持默认名称Sheet1。</param>
      /// <param name="templatePath">模版在项目服务器中路径，例：tp.xlsx，为空字符串时表示无模版。</param>
      /// <param name="templateRow">模版中已存在数据的行数，无模版时请传入参数0。</param>
      /// <param name="exDataTableList">扩展DataTable List，用于当上下两个及以上DataTable数据类型不一致，但又都在同一列时使用，要求格式与参数第一个DataTable的列名字段名一致，仅字段类型可不同。</param>
      /// <returns>该操作如果成功，则会返回true，否则会返回false。</returns>
      public static bool DataTableToExcel(System.Data.DataTable dataTable, string excelFileUrl, string pathType, List<string> colName, string sheetName, string templatePath, int templateRow, List<System.Data.DataTable> exDataTableList)
      {
         List<string> excludeColumn = new List<string>();
         string excludeType = "0";
         return DataTableToExcel(dataTable, excelFileUrl, pathType, colName, excludeColumn, excludeType, sheetName, templatePath, templateRow, exDataTableList);
      }
      /// <summary>
      /// 从DataTable导出Excel，并指定要排除的列
      /// </summary>
      /// <param name="dataTable">数据的导出来源。</param>
      /// <param name="excelFileUrl">含Excel名称的保存路径，在pathType＝1时有效，其它请赋值空字符串。</param>
      /// <param name="pathType">路径类型，允许的取值：0客户自定义路径；1服务端定义路径，标识文件保存路径是服务端指定还是客户自定义路径及文件名。</param>
      /// <param name="excludeColumn">要显示或者排除的列。</param>
      /// <param name="excludeType">显示或者排除列方式：0为所有列，1指定的为要显示的列，2指定的为要排除的列。</param>
      /// <param name="sheetName">第一张工作薄的名称，为空字符串时保持默认名称Sheet1。</param>
      /// <param name="templatePath">模版在项目服务器中路径，例：tp.xlsx，为空字符串时表示无模版。</param>
      /// <param name="templateRow">模版中已存在数据的行数，无模版时请传入参数0。</param>
      /// <param name="exDataTableList">扩展DataTable List，用于当上下两个及以上DataTable数据类型不一致，但又都在同一列时使用，要求格式与参数第一个DataTable的列名字段名一致，仅字段类型可不同。</param>
      /// <returns>该操作如果成功，则会返回true，否则会返回false。</returns>
      public static bool DataTableToExcel(System.Data.DataTable dataTable, string excelFileUrl, string pathType, List<string> excludeColumn, string excludeType, string sheetName, string templatePath, int templateRow, List<System.Data.DataTable> exDataTableList)
      {
         List<string> colName = new List<string>();
         return DataTableToExcel(dataTable, excelFileUrl, pathType, colName, excludeColumn, excludeType, sheetName, templatePath, templateRow, exDataTableList);
      }
      /// <summary>
      /// 从DataTable导出Excel，使用默认列名，不排除导出任何列。
      /// </summary>
      /// <param name="dataTable">数据的导出来源。</param>
      /// <param name="excelFileUrl">含Excel名称的保存路径，在pathType＝1时有效，其它请赋值空字符串。</param>
      /// <param name="pathType">路径类型，允许的取值：0客户自定义路径；1服务端定义路径，标识文件保存路径是服务端指定还是客户自定义路径及文件名。</param>
      /// <param name="sheetName">第一张工作薄的名称，为空字符串时保持默认名称Sheet1。</param>
      /// <param name="templatePath">模版在项目服务器中路径，例：tp.xlsx，为空字符串时表示无模版。</param>
      /// <param name="templateRow">模版中已存在数据的行数，无模版时请传入参数0。</param>
      /// <param name="exDataTableList">扩展DataTable List，用于当上下两个及以上DataTable数据类型不一致，但又都在同一列时使用，要求格式与参数第一个DataTable的列名字段名一致，仅字段类型可不同。</param>
      /// <returns>该操作如果成功，则会返回true，否则会返回false。</returns>
      public static bool DataTableToExcel(System.Data.DataTable dataTable, string excelFileUrl, string pathType, string sheetName, string templatePath, int templateRow, List<System.Data.DataTable> exDataTableList)
      {
         List<string> colName = new List<string>();
         List<string> excludeColumn = new List<string>();
         string excludeType = "0";
         return DataTableToExcel(dataTable, excelFileUrl, pathType, colName, excludeColumn, excludeType, sheetName, templatePath, templateRow, exDataTableList);
      }
      /// <summary>
      /// 从DataTable导出Excel，并指定列别名和需要排除的列。
      /// </summary>
      /// <param name="dataTable">数据的导出来源。</param>
      /// <param name="excelFileUrl">含Excel名称的保存路径，在pathType＝1时有效，其它请赋值空字符串。</param>
      /// <param name="pathType">路径类型，允许的取值：0客户自定义路径；1服务端定义路径，标识文件保存路径是服务端指定还是客户自定义路径及文件名。</param>
      /// <param name="colName">各列的列名List。</param>
      /// <param name="excludeColumn">要显示或者排除的列。</param>
      /// <param name="excludeType">显示或者排除列方式：0为所有列，1指定的为要显示的列，2指定的为要排除的列。</param>
      /// <param name="sheetName">第一张工作薄的名称，为空字符串时保持默认名称Sheet1。</param>
      /// <param name="templatePath">模版在项目服务器中路径，例：tp.xlsx，为空字符串时表示无模版。</param>
      /// <param name="templateRow">模版中已存在数据的行数，无模版时请传入参数0。</param>
      /// <param name="exDataTableList">扩展DataTable List，用于当上下两个及以上DataTable数据类型不一致，但又都在同一列时使用，要求格式与参数第一个DataTable的列名字段名一致，仅字段类型可不同。</param>
      /// <returns>该操作如果成功，则会返回true，否则会返回false。</returns>
      public static bool DataTableToExcel(System.Data.DataTable dataTable, string excelFileUrl, string pathType, List<string> colName, List<string> excludeColumn, string excludeType, string sheetName, string templatePath, int templateRow, List<System.Data.DataTable> exDataTableList)
      {
         try
         {
            if (dataTable == null || dataTable.Rows.Count == 0) return false;
            Application xlApp = new Application();
            if (xlApp == null) return false;
            CultureInfo CurrentCI = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Workbooks workbooks = xlApp.Workbooks;
            Workbook workbook = null;
            if (templatePath == string.Empty) workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            else workbook = workbooks.Add(templatePath);
            Worksheet worksheet = (Worksheet)workbook.Worksheets[1];
            Range range;
            long totalCount = dataTable.Rows.Count;
            if (exDataTableList != null && exDataTableList.Count > 0)
            {
               foreach (System.Data.DataTable item in exDataTableList) totalCount += item.Rows.Count;
            }
            long rowRead = 0;
            float percent = 0;
            string exclStr = string.Empty;
            object exclType;
            int colPosition = 0;
            if (sheetName != null && sheetName != string.Empty) worksheet.Name = sheetName;
            if (templatePath == string.Empty)
            {
               if (colName != null && colName.Count > 0)
               {
                  for (int i = 0; i < colName.Count; i++)
                  {
                     worksheet.Cells[1, i + 1] = colName[i];
                     range = (Range)worksheet.Cells[1, i + 1];
                     range.Interior.ColorIndex = 15;
                     range.Font.Bold = true;
                     exclType = dataTable.Columns[i].DataType.Name;
                     if (exclType.ToString() != "DateTime") ((Range)worksheet.Cells[1, i + 1]).Columns.AutoFit();
                     else ((Range)worksheet.Cells[1, i + 1]).Columns.ColumnWidth = 20;
                  }
               }
               else
               {
                  for (int i = 0; i < dataTable.Columns.Count; i++)
                  {
                     worksheet.Cells[1, i + 1] = dataTable.Columns[i].ColumnName;
                     range = (Range)worksheet.Cells[1, i + 1];
                     range.Interior.ColorIndex = 15;
                     range.Font.Bold = true;
                     exclType = dataTable.Columns[i].DataType.Name;
                     if (exclType.ToString() != "DateTime") ((Range)worksheet.Cells[1, i + 1]).Columns.AutoFit();
                     else ((Range)worksheet.Cells[1, i + 1]).Columns.ColumnWidth = 20;
                  }
               }
            }
            if (excludeColumn != null && excludeColumn.Count > 0)
            {
               switch (excludeType)
               {
                  case "0":
                     {
                        int r = 0;
                        for (r = 0; r < dataTable.Rows.Count; r++)
                        {
                           colPosition = 0;
                           for (int i = 0; i < dataTable.Columns.Count; i++)
                           {
                              if (templatePath == string.Empty) worksheet.Cells[r + 2, colPosition + 1] = dataTable.Rows[r][i].ToString();
                              else worksheet.Cells[r + templateRow, colPosition + 1] = dataTable.Rows[r][i].ToString();
                              colPosition++;
                           }
                           rowRead++;
                           percent = ((float)(100 * rowRead)) / totalCount;
                        }
                        if (exDataTableList != null && exDataTableList.Count > 0)
                        {
                           foreach (System.Data.DataTable item in exDataTableList)
                           {
                              for (int k = 0; k < item.Rows.Count; r++, k++)
                              {
                                 colPosition = 0;
                                 for (int t = 0; t < item.Columns.Count; t++)
                                 {
                                    if (templatePath == string.Empty) worksheet.Cells[r + 2, colPosition + 1] = item.Rows[k][t].ToString();
                                    else worksheet.Cells[r + templateRow, colPosition + 1] = item.Rows[k][t].ToString();
                                    colPosition++;
                                 }
                                 rowRead++;
                                 percent = ((float)(100 * rowRead)) / totalCount;
                              }
                           }
                        }
                     };
                     break;
                  case "1":
                     {
                        int r = 0;
                        for (r = 0; r < dataTable.Rows.Count; r++)
                        {
                           colPosition = 0;
                           for (int i = 0; i < dataTable.Columns.Count; i++)
                           {
                              exclStr = dataTable.Columns[i].ColumnName;
                              if (excludeColumn.Contains(exclStr))
                              {
                                 if (templatePath == string.Empty) worksheet.Cells[r + 2, colPosition + 1] = dataTable.Rows[r][i].ToString();
                                 else worksheet.Cells[r + templateRow, colPosition + 1] = dataTable.Rows[r][i].ToString();
                                 colPosition++;
                              }
                           }
                           rowRead++;
                           percent = ((float)(100 * rowRead)) / totalCount;
                        }
                        if (exDataTableList != null && exDataTableList.Count > 0)
                        {
                           foreach (System.Data.DataTable item in exDataTableList)
                           {
                              for (int k = 0; k < item.Rows.Count; r++, k++)
                              {
                                 colPosition = 0;
                                 for (int t = 0; t < item.Columns.Count; t++)
                                 {
                                    exclStr = dataTable.Columns[t].ColumnName;
                                    if (excludeColumn.Contains(exclStr))
                                    {
                                       if (templatePath == string.Empty) worksheet.Cells[r + 2, colPosition + 1] = item.Rows[k][t].ToString();
                                       else worksheet.Cells[r + templateRow, colPosition + 1] = item.Rows[k][t].ToString();
                                       colPosition++;
                                    }
                                 }
                                 rowRead++;
                                 percent = ((float)(100 * rowRead)) / totalCount;
                              }
                           }
                        }
                     };
                     break;
                  case "2":
                     {
                        int r = 0;
                        for (r = 0; r < dataTable.Rows.Count; r++)
                        {
                           colPosition = 0;
                           for (int i = 0; i < dataTable.Columns.Count; i++)
                           {
                              exclStr = dataTable.Columns[i].ColumnName;
                              if (!excludeColumn.Contains(exclStr))
                              {
                                 if (templatePath == string.Empty) worksheet.Cells[r + 2, colPosition + 1] = dataTable.Rows[r][i].ToString();
                                 else worksheet.Cells[r + templateRow, colPosition + 1] = dataTable.Rows[r][i].ToString();
                                 colPosition++;
                              }
                           }
                           rowRead++;
                           percent = ((float)(100 * rowRead)) / totalCount;
                        }
                        if (exDataTableList != null && exDataTableList.Count > 0)
                        {
                           foreach (System.Data.DataTable item in exDataTableList)
                           {
                              for (int k = 0; k < item.Rows.Count; r++, k++)
                              {
                                 colPosition = 0;
                                 for (int t = 0; t < item.Columns.Count; t++)
                                 {
                                    exclStr = dataTable.Columns[t].ColumnName;
                                    if (!excludeColumn.Contains(exclStr))
                                    {
                                       if (templatePath == string.Empty) worksheet.Cells[r + 2, colPosition + 1] = item.Rows[k][t].ToString();
                                       else worksheet.Cells[r + templateRow, colPosition + 1] = item.Rows[k][t].ToString();
                                       colPosition++;
                                    }
                                 }
                                 rowRead++;
                                 percent = ((float)(100 * rowRead)) / totalCount;
                              }
                           }
                        }
                     };
                     break;
                  default:
                     break;
               }
            }
            else
            {
               int r = 0;
               for (r = 0; r < dataTable.Rows.Count; r++)
               {
                  if (templatePath == string.Empty)
                  {
                     for (int i = 0; i < dataTable.Columns.Count; i++)
                     {
                        worksheet.Cells[r + 2, i + 1] = dataTable.Rows[r][i].ToString();
                     }
                  }
                  else
                  {
                     for (int i = 0; i < dataTable.Columns.Count; i++)
                     {
                        worksheet.Cells[r + 1 + templateRow, i + 1] = dataTable.Rows[r][i].ToString();
                     }
                  }
                  rowRead++;
                  percent = ((float)(100 * rowRead)) / totalCount;
               }
            }
            switch (pathType)
            {
               case "0": { workbook.Saved = false; }; break;
               case "1": { workbook.Saved = true; workbook.SaveCopyAs(excelFileUrl); }; break;
               default:
                  return false;
            }
            xlApp.Visible = false;
            workbook.Close(true, Type.Missing, Type.Missing);
            workbook = null;
            xlApp.Quit();
            xlApp = null;
            return true;
         }
         catch (Exception)
         {
            return false;
         }
      }
      /// <summary>
      /// 将Excel数据导出到DataTable。
      /// </summary>
      /// <param name="excelFileUrl">需要被导出数据的Excel文档的文件路径。</param>
      /// <param name="sheetName">需要被导出数据的工作薄名称。</param>
      /// <returns>该操作会返回一个包含Excel数据的DataTable实例。</returns>
      public static System.Data.DataTable ExcelToDataTable(string excelFileUrl, string sheetName)
      {
         string dbconnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + excelFileUrl + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
         OleDbConnection oleDbConnection = new OleDbConnection(dbconnectionString);
         oleDbConnection.Open();
         System.Data.DataTable dtSheetName = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
         string[] strTableNames = new string[dtSheetName.Rows.Count];
         for (int k = 0; k < dtSheetName.Rows.Count; k++)
         {
            strTableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
         }
         OleDbDataAdapter myCommand = null;
         System.Data.DataTable dt = new System.Data.DataTable();
         string strExcel = "select*from[" + strTableNames[0] + "]";
         myCommand = new OleDbDataAdapter(strExcel, dbconnectionString);
         dt = new System.Data.DataTable();
         myCommand.Fill(dt);
         return dt;
      }
   }
}
