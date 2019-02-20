﻿using Microsoft.Win32;
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
using static yiyaofeibaoxiao.HuaMingCeReader;

namespace yiyaofeibaoxiao
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string cfg_excelDir;

        MainWindow mainWindow = new MainWindow();
        protected override async void OnStartup(StartupEventArgs e)
        {
            //Load Configuration File
            base.OnStartup(e);
            mainWindow.Show();

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
                mainWindow.txbSheetDir.Text = excel_file.FullName;
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
            }

        }

        private void readStarted(object sender, EventArgs e)
        {
            mainWindow.lbxDebug.Items.Add("开始关联工作表，请等待。");
        }

        private void progressChanged(object sender, hProgressChangedEventArgs e)
        {
            
            mainWindow.lbxDebug.Items.Add(string.Format("{0}/{1}",e.CurrentProgress,e.TotalProgress));
        }
    }


}
