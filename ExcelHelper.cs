using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using System.Data.OleDb;
using System.IO;
using System.Data;

namespace Utility
{
    public class ExcelHelper
    {

       static  LogWriter log = new LogWriter("log.txt");

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public static void DataTable2XLSX(System.Data.DataTable dt, string fileName)
        {

            if (string.IsNullOrEmpty(fileName))
                return;
            int rowCount = dt.Rows.Count;
            int columnCount = dt.Columns.Count;
            int index = 0;
            //diyProcessBar1.Maximum = rowCount;
            //diyProcessBar1.Visible = true;

            Microsoft.Office.Interop.Excel.Application xlsApp = new Microsoft.Office.Interop.Excel.Application();
            xlsApp.Application.Workbooks.Add(true);
            xlsApp.Visible = false;
            //Microsoft.Office.Interop.Excel.Workbooks workBooks = xlsApp.ActiveWorkbook;
            //Microsoft.Office.Interop.Excel.Workbook workBook = workBooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Workbook workBook = xlsApp.ActiveWorkbook;
            Microsoft.Office.Interop.Excel.Worksheet workSheet1 = null;

            try
            {
                #region range create
                workSheet1 = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                for (int i = 0; i < rowCount; i++)
                {
                    if (i == 0)
                    {
                        for (int j = 2; j < columnCount + 2; j++)
                        {
                            Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)workSheet1.Cells[i + 1, j - 1];
                            range.Value2 = dt.Columns[j - 2].ColumnName;
                            range.Font.Bold = true;
                            range.Interior.Color = Color.Azure.ToArgb();
                            range.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, null);
                            range.EntireColumn.AutoFit();
                        }
                    }
                    for (int j = 2; j < columnCount + 2; j++)
                    {
                        Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)workSheet1.Cells[i + 2, j - 1];
                        range.Value = dt.Rows[i][j - 2];
                        range.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, null);
                        range.EntireColumn.AutoFit();
                        if (range.Value2 != null && range.Value2.ToString().StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                        {
                            workSheet1.Hyperlinks.Add(range, range.Value2.ToString(), Missing.Value, Missing.Value, Missing.Value);
                        }
                    }
                    index++;
                    //diyProcessBar1.Value = index;
                    //Application.DoEvents();
                    //workSheet.get_Range((Microsoft.Office.Interop.Excel.Range)workSheet.Cells[i, 1], (Microsoft.Office.Interop.Excel.Range)workSheet.Cells[i, 8]).Select();
                }

                xlsApp.DisplayAlerts = false;
                xlsApp.AlertBeforeOverwriting = false;
                //diyProcessBar1.Visible = false;


                workBook.SaveCopyAs(fileName);

                #endregion
            }
            catch (Exception ex)
            {
                log.WriteLine("WriteExcelField error" + ex.Message);
            }
            finally
            {
                #region finally
                if (workBook != null)
                {
                    workBook.Close(false, Type.Missing, Type.Missing);
                }
                IntPtr t = new IntPtr(xlsApp.Hwnd);
                xlsApp.Quit();
                if (workSheet1 != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet1);
                }

                if (workBook != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workBook);
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlsApp);
                int k = 0;
                GetWindowThreadProcessId(t, out k);
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);
                p.Kill();
                GC.Collect();
                #endregion
            }
        }


        static ExcelHelper()
        {

        }

        public static String GetOleDbTypeString(object type, int maxlength)
        {

            string typeName = (type as Type).FullName;
            string result = maxlength == 255 ? "Text" : "Memo";
            switch (typeName)
            {
                case "System.String":
                    result = "Text";
                    break;
                case "System.Byte":
                    result = "Byte";
                    break;
                case "System.Integer":
                    result = "Integer";
                    break;
                case "System.Single":
                    result = "Single";
                    break;
                case "System.Long":
                    result = "Long";
                    break;
                case "System.Double":
                    result = "Double";
                    break;
                case "System.Currency":
                    result = "Currency";
                    break;
                case "System.DateTime":
                    result = "Date";
                    break;
            }
            return result;
        }

        public static OleDbType GetOleDbType(object type)
        {
            string typeName = (type as Type).FullName;
            OleDbType result = OleDbType.VarChar;
            switch (typeName)
            {
                case "System.String":
                    result = OleDbType.VarWChar;
                    break;
                case "System.Byte":
                    result = OleDbType.UnsignedTinyInt;
                    break;
                case "System.Byte[]":
                    result = OleDbType.Binary;
                    break;
                case "System.Boolean":
                    result = OleDbType.Boolean;
                    break;
                case "System.DateTime":
                    result = OleDbType.Date;
                    break;
                case "System.Decimal":
                    result = OleDbType.Decimal;
                    break;
                case "System.Double":
                    result = OleDbType.Double;
                    break;
                case "System.Guid":
                    result = OleDbType.Guid;
                    break;
                case "System.Int32":
                    result = OleDbType.Integer;
                    break;
                case "System.Int64":
                    result = OleDbType.BigInt;
                    break;
                case "System.Single":
                    result = OleDbType.Single;
                    break;
                case "System.Int16":
                    result = OleDbType.SmallInt;
                    break;
            }
            return result;
        }

        public static void CreateExcel(System.Data. DataTable dt, List<ObjectItem<string, Type, int>> listColumns, string fileName = null, string idColumn = null)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                string extName = Path.GetExtension(fileName);
                string connString = string.Empty;
                if (extName.Equals(".xlsx"))
                {
                    connString = "Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties=Excel 12.0 XML;Data Source=" + fileName + ";";
                }
                else
                {
                    connString = "Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;Data Source=" + fileName + ";";
                }
                OleDbConnection conn = new OleDbConnection(connString);
                try
                {
                    conn.Open();
                    int col = dt.Columns.Count;
                    if (col > 3)
                    {
                        if (!String.IsNullOrWhiteSpace(idColumn))
                        {
                            var rr = dt.Select(idColumn + " IS NULL");
                            foreach (DataRow r in rr)
                            {
                                dt.Rows.Remove(r);
                            }
                        }
                        string tabName = "Test";
                        List<string> list1 = new List<string>();
                        List<string> list2 = new List<string>();
                        List<string> list3 = new List<string>();
                        OleDbDataAdapter ada = new OleDbDataAdapter();
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = conn;
                        ada.InsertCommand = cmd;
                        foreach (var oi in listColumns)
                        {
                            string colName = oi.Id;
                            if (dt.Columns.Contains(colName))
                            {
                                if (!cmd.Parameters.Contains("@" + colName))
                                {
                                    OleDbType odType = GetOleDbType((oi.Name));
                                    String typeStr = odType.ToString();
                                    list1.Add("[" + colName + "]");
                                    list2.Add("@" + colName);
                                    list3.Add("[" + colName + "] " + GetOleDbTypeString(oi.Name, oi.Tag));
                                    cmd.Parameters.Add("@" + colName, GetOleDbType(oi.Name), oi.Tag, colName);
                                }
                            }
                        }

                        cmd.CommandText = string.Format("CREATE TABLE [{0}]({1})", tabName, string.Join(",", list3.ToArray()));
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = string.Format("INSERT INTO [{0}] ({1}) VALUES({2})", tabName, string.Join(",", list1.ToArray()), string.Join(",", list2.ToArray()));
                        cmd.UpdatedRowSource = UpdateRowSource.None;
                        int count = ada.Update(dt);
                        if (count > 0)
                        {
                           log.WriteLine("成功");
                        }
                        else
                        {
                            log.WriteLine("失败");
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

    }
}

