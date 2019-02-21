using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation;
using static YiYao.HuaMingCeReader;
using System.Windows.Controls.Primitives;

namespace YiYao
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindow mainWindow = new MainWindow();
        protected override async void OnStartup(StartupEventArgs e)
        {
            //Load Configuration File
            base.OnStartup(e);
            mainWindow.Show();
            
            //MessageBox.Show(YiYao.Properties.Settings.Default.sourceSheetPath);
            //Getting settings from this setting.

            //从全局设定中找到默认设置
            FileInfo excel_file = 
                new FileInfo(YiYao.Properties.Settings.Default.sourceSheetPath);
            //如果未找到配置文件中的元数据工作表位置
            //则要求用户选择元数据工作表

            if (!excel_file.Exists)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Excel工作表|*.xls";
                ofd.Title = "请选择作为数据源的Excel表格";
                Nullable<bool> result = ofd.ShowDialog();
                YiYao.Properties.Settings.Default.sourceSheetPath = ofd.FileName; //Application = cannot modify; user = can modify;
                if (result != true)
                {
                    MessageBox.Show("Excel文件选择取消，将退出。", "未发现有效Excel文件");
                    Shutdown();
                    return;
                }

            }
            
            else
            {
                mainWindow.txbSheetDir.Text = YiYao.Properties.Settings.Default.sourceSheetPath;
                Application.Current.Properties["Source_Worksheet_Path"] = excel_file.FullName;

                /* Moving provess block to an async area in MainWindow.xaml.cs
                DateTime startTime = DateTime.Now;
                HuaMingCeReader hmcReader = new HuaMingCeReader();
                hmcReader.ReadStarted += readStarted;
                hmcReader.hProgressChanged += progressChanged;
                DataSet ExcelFileDs;
                await Task.Run(() => ExcelFileDs = hmcReader.ReadFromExcel(excel_file));
                TimeSpan runTime = DateTime.Now - startTime;
                mainWindow.lbxDebug.Items.Add("关联工作表完毕，用时：" + runTime);
                GC.Collect();*/
                mainWindow
                    .bnStartSync
                    .RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }

        }
        protected override void OnExit(ExitEventArgs e)
        {
            MessageBox.Show(YiYao.Properties.Settings.Default.sourceConnStr);
            YiYao.Properties.Settings.Default.Save();
            base.OnExit(e);
        }
    }


}
