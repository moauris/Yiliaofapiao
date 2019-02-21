using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EXCEL = Microsoft.Office.Interop.Excel;

namespace YiYao
{
    public class HuaMingCeReader
    {
        //Read huamingce.xls
        public event EventHandler<EventArgs> ReadStarted;
        protected virtual void OnReadStarted(EventArgs e)
        {
            ReadStarted?.Invoke(this, e);
        }

        public class hProgressChangedEventArgs : EventArgs
        {
            public readonly int CurrentProgress;
            public readonly int TotalProgress;
            public hProgressChangedEventArgs(int currentProgress)
            {
                CurrentProgress = currentProgress;
            }
            public hProgressChangedEventArgs(
                int currentProgress, int totalProgress)
            {
                CurrentProgress = currentProgress;
                TotalProgress = totalProgress;
            }
        }
        public event EventHandler<hProgressChangedEventArgs> hProgressChanged;
        protected virtual void OnProgressChanged(hProgressChangedEventArgs e)
        {
            hProgressChanged?.Invoke(this, e);
        }

        public async Task<DataSet> ReadFromExcel(FileInfo InputFile)
        {
            
            if (InputFile.Extension != ".xls")
                throw new Exception("指定文件不是xls工作簿。");
            if (!InputFile.Exists)
                throw new Exception("指定文件不存在。");
            //如果判定是xls文件并存在，则试图连接并创造dataset对象。
            OnReadStarted(new EventArgs());//发射读取开始事件
            EXCEL.Application xlApp = new EXCEL.Application();
            EXCEL.Workbooks xlWorkbooks = xlApp.Workbooks;
            EXCEL.Workbook xlWb = xlWorkbooks.Open(InputFile.FullName);
            EXCEL.Sheets xlWorksheets = xlWb.Worksheets;
            EXCEL.Worksheet xlSh = xlWorksheets["员工花名册"];
            EXCEL.Range xlRg = xlSh.UsedRange;

            int total_Cell_Count = xlRg.Count;
            int total_Row_Count = total_Cell_Count / 4;
            //Debug.Print(xlRg.Rows.Count.ToString());
            OnProgressChanged(new hProgressChangedEventArgs(0, total_Row_Count));
            //发射进度改变事件：初始化总数为Used Range Row Count
            DataSet output = new DataSet();
            DataTable outputT = new DataTable();
            //Populating Column Names
            //DataColumn[] output_columns = new DataColumn[16];
            int Current_Row = 0;
            for(int c = 1; c <= 4; c++)//以4列为准循环
            {
                /*
                string cellVal = xlRg[c].Value;
                byte[] cellValBin = new byte[cellVal.Length - 1];
                for(int ix=0;ix < (cellVal.Length - 1); ix++)
                {
                    cellValBin[ix] = (byte)cellVal[ix];
                }
                Console.WriteLine(cellValBin);
                */
                var CellValue = (string)(xlRg[c] as EXCEL.Range).Value;
                //Console.WriteLine(CellValue);
                DataColumn newColumn = new DataColumn();
                newColumn.DataType = System.Type.GetType("System.String");
                newColumn.AllowDBNull = true;
                newColumn.Caption = CellValue;
                newColumn.ColumnName = CellValue;

                outputT.Columns.Add(newColumn);
                OnProgressChanged(new hProgressChangedEventArgs(++Current_Row));
            }
            Debug.Print(xlRg.Count.ToString());

            for(int c = 5; c <= total_Cell_Count; c++)
            {
                string[] RowVal = new string[4]; // 1 - 4 total: 4
                RowVal[c % 4] = xlRg[c].Value.ToString();

                if(c % 4 == 0)
                {
                    outputT.Rows.Add(
                        RowVal[0], RowVal[1], RowVal[2], RowVal[3]);//, RowVal[4], RowVal[5],
                        //RowVal[6], RowVal[7],RowVal[8],RowVal[9],RowVal[10],RowVal[11],
                        //RowVal[12],RowVal[13],RowVal[14],RowVal[15],RowVal[16]);
                    OnProgressChanged(new hProgressChangedEventArgs(++Current_Row));
                }

            }


            xlWb.Close();
            xlWorkbooks.Close();
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
            Marshal.ReleaseComObject(xlWorkbooks);
            Marshal.ReleaseComObject(xlWb);
            Marshal.ReleaseComObject(xlWorksheets);
            Marshal.ReleaseComObject(xlSh);
            Marshal.ReleaseComObject(xlRg);

            /*
            string strConn;
            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
                + InputFile.FullName
                + ";Extended Properties=\"Excel.8.0;HDR=Yes"
                + ";IMEX=0\"";

            Debug.Print("连接字符为：");
            Debug.Print(strConn);

            DataSet output = new DataSet();

            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();

                DataTable schemaT = conn.GetOleDbSchemaTable(
                    OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                foreach (DataRow schemaRow in schemaT.Rows)
                {
                    string sheet = schemaRow["TABLE_NAME"].ToString();
                    if(sheet == "员工花名册")
                    {
                        try
                        {
                            OleDbCommand cmd = new OleDbCommand(
                                "SELECT * FROM [" + sheet + "]", conn);
                            cmd.CommandType = CommandType.Text;

                            DataTable outputT = new DataTable(sheet);
                            output.Tables.Add(outputT);
                            new OleDbDataAdapter(cmd).Fill(outputT);
                        } catch (Exception ex)
                        {
                            throw new Exception(ex.Message
                                + string.Format(
                                    "Sheet:{0}.File:F{1}",sheet
                                    , InputFile.FullName),ex);

                        }
                    }
                }
                Result = output;
            }*/
            return output;
        }
    }
}
