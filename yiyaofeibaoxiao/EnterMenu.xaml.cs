using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace yiyaofeibaoxiao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class EnterMenu : Window
    {
        double JineMenZhen, JineZhuyuan;
        double JineMenZhen90, JineZhuyuan90;
        double JineXiaoji, JineXiaoji90;
        public EnterMenu()
        {
            InitializeComponent();

        }

        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            PrintDialog printPage = new PrintDialog();
            if(printPage.ShowDialog() == true)
            {
                printPage.PrintVisual(this.WrapperStackPanel, this.Title);
            }
        }

        private void OnMenzhenEntered(object sender, RoutedEventArgs e)
        {
            Debug.Print("门诊金额输入完成:" + txb_Menzhen.Text);
            //Validate Input is round 2 number
            //如果没有输入，return
            if(txb_Menzhen.Text == "") return;
            JineMenZhen = Convert.ToDouble(txb_Menzhen.Text);
            JineMenZhen90 = Math.Round(JineMenZhen * 0.9, 2);
            tbcMenZhenShenhe.Text = 
                JineMenZhen90 == 0 ? "￥ --" : JineMenZhen90.ToString("￥ ##.00");
        }

        private void OnClearClicked(object sender, RoutedEventArgs e)
        {
            //点击时清除所有填写项内容，重新建立表格。
            txb_Menzhen.Text = "";
            txb_Zhuyuan.Text = "";
            tbcMenZhenShenhe.Text = "";
            tbcZhuyuanShenhe.Text = "";
            tbcXiaojiShenhe.Text = "";
            tbcXiaoji.Text = "";
            tbcEDing.Text = "";
            tbcDaxie.Text = "";

        }

        private void OnZhuyuanEntered(object sender, RoutedEventArgs e)
        {
            Debug.Print("住院金额输入完成:" + txb_Zhuyuan.Text);
            //Validate Input is round 2 number
            if (txb_Zhuyuan.Text == "") return;

            JineZhuyuan = Convert.ToDouble(txb_Zhuyuan.Text);
            JineZhuyuan90 = Math.Round(JineZhuyuan * 0.9, 2);
            tbcZhuyuanShenhe.Text = JineZhuyuan90 == 0 ? "￥ --" : JineZhuyuan90.ToString("￥ ##.00");
            JineXiaoji = Math.Round((JineMenZhen + JineZhuyuan), 2);
            JineXiaoji90 = Math.Round((JineMenZhen90 + JineZhuyuan90), 2);
            tbcXiaoji.Text = JineXiaoji == 0 ? "￥ --" : JineXiaoji.ToString("￥ ##.00");
            tbcXiaojiShenhe.Text = JineXiaoji90 == 0 ? "￥ --" : JineXiaoji90.ToString("￥ ##.00");
            tbcEDing.Text = JineXiaoji90.ToString("￥ ##.00");
            tbcDaxie.Text = ConvertDaxie(JineXiaoji90);

        }

        private void Txb_ValidDouble_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textb = (TextBox)sender;
            TextChange tc = e.Changes.First();
            int CursorPos = textb.SelectionStart;
            // When new string cannot be parsed as double
            if (tc.RemovedLength != 0) return; //When Delete, do nothing
            var jine = double.TryParse(textb.Text, out double JinE);
            if (!jine)
            {
                //Debug.Print("Invalid");
                //When Invalid, delete the lastest input
                textb.Text = textb.Text.Remove(tc.Offset, 1);
                //Cursor location moved to right most, need to stay.
                textb.SelectionStart = CursorPos;
                textb.SelectionLength = 1;
            }

        }

        private static string ConvertDaxie(double Jine)
        {
            //This Function Outputs Numerical Values into 
            //Chinese Capitalized Numericals, 
            //Maximum 100_000_000 一百万
            Dictionary<char, string> Daxie = new Dictionary<char, string>();
            Daxie.Add('1', "壹"); Daxie.Add('2', "贰"); Daxie.Add('3', "叁");
            Daxie.Add('4', "肆"); Daxie.Add('5', "伍"); Daxie.Add('6', "陆");
            Daxie.Add('7', "柒"); Daxie.Add('8', "捌"); Daxie.Add('9', "玖");
            Daxie.Add('0', "零");
            /*
             Daxie.Add(10, "拾"); Daxie.Add(100, "佰");
            Daxie.Add(1000, "仟"); Daxie.Add(10000, "万");
            Daxie.Add(0.1, "角"); Daxie.Add(0.01, "分"); */
            //Get Total Digit Before Decimal
            string Jine00 = Jine.ToString("##.00");
            string Zhengshu = Jine00.Split('.')[0];
            string Xiaoshu = Jine00.Split('.')[1];
            int Len = Zhengshu.Length;
            if (Len > 10) return "你真有钱";
            //Top part x万, get x

            StringBuilder sbDaxie = new StringBuilder();
            if (Len >= 5)
            {
                //提取万字以上
                string AboveWan = Zhengshu.Substring(0, Len - 4);
                switch (AboveWan.Length)
                {
                    case 3:
                        sbDaxie.Append(Daxie[AboveWan[0]]);
                        sbDaxie.Append("佰");
                        sbDaxie.Append(Daxie[AboveWan[1]]);
                        if(AboveWan[1] != '0') sbDaxie.Append("拾");
                        if(AboveWan[2] != '0') sbDaxie.Append(Daxie[AboveWan[2]]);
                        sbDaxie.Append("万");
                        break;
                    case 2:
                        sbDaxie.Append(Daxie[AboveWan[0]]);
                        sbDaxie.Append("拾");
                        if (AboveWan[1] != '0') sbDaxie.Append(Daxie[AboveWan[1]]);
                        sbDaxie.Append("万");
                        break;
                    case 1:
                        sbDaxie.Append(Daxie[AboveWan[0]]);
                        sbDaxie.Append("万");
                        break;
                }
                //return sbDaxie.ToString();
            }
            //提取万字以下
            string BelowWan = Zhengshu.Length < 4 ?
                Zhengshu : Zhengshu.Substring(Len - 4, 4);
            //Debug.Print(BelowWan);
            //sbDaxie.Append(BelowWan);
            switch (BelowWan.Length)
            {
                case 4:
                    if (BelowWan[0] != '0')
                    {
                        sbDaxie.Append(Daxie[BelowWan[0]]);
                        sbDaxie.Append("仟");
                    }
                    if (BelowWan[1] != '0')
                    {
                        if (BelowWan[0] == '0') sbDaxie.Append('零');
                        sbDaxie.Append(Daxie[BelowWan[1]]);
                        sbDaxie.Append("佰");
                    }
                    if(BelowWan[2] != '0')
                    {
                        if (BelowWan[1] == '0') sbDaxie.Append('零');
                        sbDaxie.Append(Daxie[BelowWan[2]]);
                        sbDaxie.Append("拾");
                    }
                    if (BelowWan[3] != '0')
                    {
                        if (BelowWan[2] == '0') sbDaxie.Append('零');
                        sbDaxie.Append(Daxie[BelowWan[3]]);
                    }

                    break;
                case 3:
                    sbDaxie.Append(Daxie[BelowWan[0]]);
                    sbDaxie.Append("佰");
                    sbDaxie.Append(Daxie[BelowWan[1]]);
                    if (BelowWan[1] != '0') sbDaxie.Append("拾");
                    if (BelowWan[2] != '0') sbDaxie.Append(Daxie[BelowWan[2]]);
                    break;
                case 2:
                    sbDaxie.Append(Daxie[BelowWan[0]]);
                    sbDaxie.Append("拾");
                    if (BelowWan[1] != '0') sbDaxie.Append(Daxie[BelowWan[1]]);
                    break;
                case 1:
                    sbDaxie.Append(Daxie[BelowWan[0]]);
                    break;
            }

            sbDaxie.Append('圆');
            //提取小数点以后
            Debug.Print("Xiaoshu:" + Xiaoshu);
            if (Xiaoshu == "00")
            {
                sbDaxie.Append("整");
            }
            else
            {
                if (Xiaoshu[0] == '0')
                {
                    sbDaxie.Append("零");
                    sbDaxie.Append(Daxie[Xiaoshu[1]]);
                    sbDaxie.Append("分");
                }
                else
                {
                    sbDaxie.Append(Daxie[Xiaoshu[0]]);
                    sbDaxie.Append("角");
                    if(Xiaoshu[1] != '0')
                    {
                        sbDaxie.Append(Daxie[Xiaoshu[1]]);
                        sbDaxie.Append("分");
                    }
                    else
                    {
                        sbDaxie.Append("整");
                    }
                    
                }
            }


            return sbDaxie.ToString();
        }

    }
}
