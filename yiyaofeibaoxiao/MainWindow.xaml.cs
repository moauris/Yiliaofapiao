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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EXCEL = Microsoft.Office.Interop.Excel;

namespace YiYao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable Sheet_DataSource;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnMakeNewClicked(object sender, RoutedEventArgs e)
        {
            EnterMenu newEnterMenu = new EnterMenu();
            newEnterMenu.Show();
        }

        private void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能还未开通。");
        }

        private void OnDeleteClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能还未开通。");
        }

        private void OnFixClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能还未开通。");
        }

        private void OnDatabaseClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能还未开通。");
        }

        private async void onSyncExcelClicked(object sender, RoutedEventArgs e)
        {
            
            var setProg = new Progress<int>(value => this.pbMain.Value = value);
            var setPTotal = new Progress<int>(value => pbMain.Maximum = value);
            var setDbgMsg = new Progress<string>(value => {
                lbxDebug.Items.Add(value);
                lbxDebug.ScrollIntoView(lbxDebug.Items[lbxDebug.Items.Count - 1]);
            });
            DateTime startTime = DateTime.Now;
            
            FileInfo InputFile = new FileInfo(YiYao.Properties.Settings.Default.sourceSheetPath);
            DataSet ExcelFileDs =
                await Task<DataSet>.Run(() =>
                {
                    ((IProgress<int>)setProg).Report(0);
                    if (InputFile.Extension != ".xls" && InputFile.Extension != ".xlsx")
                        throw new Exception("指定文件不是xls工作簿。");
                    if (!InputFile.Exists)
                        throw new Exception("指定文件不存在。");
                    DataSet output = new DataSet();
                    /*
                EXCEL.Application xlApp = new EXCEL.Application();
                EXCEL.Workbooks xlWorkbooks = xlApp.Workbooks;
                EXCEL.Workbook xlWb = xlWorkbooks.Open(InputFile.FullName);
                EXCEL.Sheets xlWorksheets = xlWb.Worksheets;
                EXCEL.Worksheet xlSh = xlWorksheets["员工花名册"];
                EXCEL.Range xlRg = xlSh.UsedRange; */
                    #region Test using OLEDB switch back if not work
                    /* Test using OLEDB switch back if not work
                    int total_Cell_Count = xlRg.Count;
                    int total_Row_Count = total_Cell_Count / 4;
                    ((IProgress<int>)setPTotal).Report(total_Row_Count);
                    Debug.Print("Total Cell is:" + total_Cell_Count.ToString());
                    Debug.Print("Total Row is:" + total_Row_Count.ToString());
                    int Current_Row = 0;
                    for (int c = 1; c <= 4; c++)//以4列为准循环
                    {
                        var CellValue = (string)(xlRg[c] as EXCEL.Range).Value;
                        DataColumn newColumn = new DataColumn();
                        newColumn.DataType = System.Type.GetType("System.String");
                        newColumn.AllowDBNull = true;
                        newColumn.Caption = CellValue;
                        newColumn.ColumnName = CellValue;

                        outputT.Columns.Add(newColumn);

                    }
                    for (int c = 5; c <= total_Cell_Count; c++)
                    {
                        string[] RowVal = new string[4]; // 1 - 4 total: 4
                        RowVal[c % 4] = xlRg[c].Value.ToString();

                        if (c % 4 == 0)
                        {
                            outputT.Rows.Add(
                                RowVal[0], RowVal[1], RowVal[2], RowVal[3]);

                            ((IProgress<int>)setProg).Report(++Current_Row);
                            Debug.Print("CurrentRow is:" + Current_Row.ToString());
                        }
                    }*/
                    #endregion Test using OLEDB switch back if not work

                    #region Connect with OLEDB
                    string strConn =
                        "Provider=Microsoft.ACE.OLEDB.12.0" +
                        ";Data Source=\"" + InputFile + "\"" +
                        ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";//
                    //";Driver={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)}";
                    //";Extended Properties = \"Excel 8.0;HDR=YES;IMEX=1\";";//DBQ=path to xls/xlsx/xlsm/xlsb file";
                    YiYao.Properties.Settings.Default.sourceConnStr = strConn;

                    ((IProgress<string>)setDbgMsg).Report("Connecting With:");
                    ((IProgress<string>)setDbgMsg).Report(strConn);
                    ((IProgress<int>)setProg).Report(20);
                    using (OleDbDataAdapter datAdapter = new OleDbDataAdapter("SELECT * FROM [员工花名册$]", strConn))
                    {
                        try
                        {

                            ((IProgress<int>)setProg).Report(30);
                            ((IProgress<string>)setDbgMsg).Report("Connected.");
                            datAdapter.Fill(output, "员工花名册");
                            var outputT = output.Tables["员工花名册"];//.AsEnumerable();
                            ((IProgress<int>)setProg).Report(40);
                            /*
                             * ((IProgress<int>)setProg).Report(30);
                            Task.Run(() =>
                            {
                                foreach (DataRow r in outputT)
                                {
                                    string row_content = string.Format("{0},{1},{2},{3}"
                                        , r[0].ToString()
                                        , r[1].ToString()
                                        , r[2].ToString()
                                        , r[3].ToString());
                                    //((IProgress<string>)setDbgMsg).Report(row_content);
                                }
                            });*/
                            ((IProgress<string>)setDbgMsg).Report("共同步项目行数:" + outputT.Rows.Count);
                            ((IProgress<int>)setProg).Report(40);
                        }
                        catch (Exception ex)
                        {
                            ((IProgress<string>)setDbgMsg).Report(ex.Message);
                            ((IProgress<int>)setProg).Report(0);
                        }


                    }
                    
                    ((IProgress<int>)setProg).Report(100);
                    #endregion Connect with OLEDB

                    /*
                    xlWb.Close();
                    xlWorkbooks.Close();
                    xlApp.Quit();
                    Marshal.ReleaseComObject(xlApp);
                    Marshal.ReleaseComObject(xlWorkbooks);
                    Marshal.ReleaseComObject(xlWb);
                    Marshal.ReleaseComObject(xlWorksheets);
                    Marshal.ReleaseComObject(xlSh);
                    Marshal.ReleaseComObject(xlRg);*/
                    return output;
                });
            Sheet_DataSource = ExcelFileDs.Tables["员工花名册"];
            TimeSpan runTime = DateTime.Now - startTime;
            lbxDebug.Items.Add("关联工作表完毕，用时：" + runTime);
            GC.Collect();
        }
    }
}
