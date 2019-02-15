using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace yiyaofeibaoxiao
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string cfg_excelDir;

        protected override void OnStartup(StartupEventArgs e)
        {
            //Load Configuration File
            base.OnStartup(e);

            FileInfo excel_file;
            //如果未找到配置文件中的元数据工作表位置
            //则要求用户选择元数据工作表
            if (cfg_excelDir == null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Excel工作表|*.xls";
                ofd.Title = "请选择作为数据源的Excel表格";
                Nullable<bool> result = ofd.ShowDialog();
                cfg_excelDir = ofd.FileName;

                if (result != true)
                {
                    MessageBox.Show("Excel文件选择取消，将退出。", "未发现有效Excel文件");
                    Shutdown();
                    return;
                }

            }
            
            excel_file = new FileInfo(cfg_excelDir);
            if (excel_file.Exists)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }

        }
    }


}
