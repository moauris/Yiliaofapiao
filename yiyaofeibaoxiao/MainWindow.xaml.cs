using System;
using System.Collections.Generic;
using System.Data;
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

namespace yiyaofeibaoxiao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnMakeNewClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnSearchClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnDeleteClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnFixClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnDatabaseClicked(object sender, RoutedEventArgs e)
        {

        }

        private async void onSyncExcelClicked(object sender, RoutedEventArgs e)
        {
            var setProg = new Progress<int>(value => pbMain.Value = value);
            var setPTotal = new Progress<int>(value => pbMain.Maximum = value);
            DateTime startTime = DateTime.Now;
            
            FileInfo InputFile = new FileInfo(txbSheetDir.Text);
            DataSet ExcelFileDs =
                await Task<DataSet>.Run(() =>
                {
                    if (InputFile.Extension != ".xls")
                        throw new Exception("指定文件不是xls工作簿。");
                    if (!InputFile.Exists)
                        throw new Exception("指定文件不存在。");

                    EXCEL.Application xlApp = new EXCEL.Application();
                    EXCEL.Workbooks xlWorkbooks = xlApp.Workbooks;
                    EXCEL.Workbook xlWb = xlWorkbooks.Open(InputFile.FullName);
                    EXCEL.Sheets xlWorksheets = xlWb.Worksheets;
                    EXCEL.Worksheet xlSh = xlWorksheets["员工花名册"];
                    EXCEL.Range xlRg = xlSh.UsedRange;

                    int total_Cell_Count = xlRg.Count;
                    int total_Row_Count = total_Cell_Count / 4;
                    ((IProgress<int>)setPTotal).Report(total_Row_Count);
                    Debug.Print("Total Cell is:" + total_Cell_Count.ToString());
                    Debug.Print("Total Row is:" + total_Row_Count.ToString());
                    DataSet output = new DataSet();
                    DataTable outputT = new DataTable();
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
                    return output;
                });

            TimeSpan runTime = DateTime.Now - startTime;
            lbxDebug.Items.Add("关联工作表完毕，用时：" + runTime);
            GC.Collect();
        }
    }
}
