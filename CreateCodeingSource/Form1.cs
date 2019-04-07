using CreateCodeingSource.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateCodeingSource
{
    public partial class Form1 : Form
    {
        List<VALIDATION> validationList = new List<VALIDATION>();
        public Form1()
        {
            InitializeComponent();
        }
        private void InputTxt_TextChanged(object sender, EventArgs e)
        {
            //空白行で区切る
            var masses = Regex.Split(inputTxt.Text, @"^\r\n", RegexOptions.Multiline);
            //改行で区切る
            List<string[]> lineList = new List<string[]>();
            foreach (var mass in masses)
            {
                var lines = Regex.Split(mass, @"\r\n", RegexOptions.Multiline);
                Array.Reverse(lines);
                lineList.Add(lines.Where(a => !string.IsNullOrWhiteSpace(a)).Select(b => b.Trim()).ToArray());
            }

            //モデルリストに格納
            foreach (var item in lineList)
            {
                if (item.Count() == 0) continue;

                var datas = item[0].Split(' ');
                propertyLB.Items.Add(datas[2]);
                //Stringlength
                long? stringLength = null;
                var stringLengthLine = item.ToList().Where(a => a.Contains("StringLength")).FirstOrDefault();
                if (!string.IsNullOrEmpty(stringLengthLine))
                {
                    var temp = stringLengthLine.Split(',')[0].Replace("[StringLength(", "").Replace(")]", "");
                    stringLength = int.Parse(temp);
                }
                //Key
                var isNullFlag = true;
                if (item.Where(a => a.Contains("[Key]")).Count() == 1)
                {
                    isNullFlag = false;
                }
                var val = new VALIDATION()
                {
                    itemName = datas[2],
                    type = datas[1],
                    isNull = isNullFlag,
                    length = stringLength
                };
                validationList.Add(val);
            }
            vALIDATIONBindingSource.DataSource = validationList;
        }

        private void OutputBt_Click(object sender, EventArgs e)
        {
            //インスタンス生成
            string insBase = @"var instance = new Object()" + Environment.NewLine + "{{" + Environment.NewLine + "{0} " + Environment.NewLine + "}}"; ;
            string insPart = "";

            foreach (var x in validationList.Select((item, index) => new { item, index }))
            {
                string formatType;
                switch (x.item.type)
                {
                    case "String":
                    case "string":
                        formatType = @"""""";
                        break;
                    case "Boolean":
                        formatType = "true";
                        break;
                    case "DateTime":
                        formatType = "new DateTime()";
                        break;
                    default:
                        formatType = "0";
                        break;
                }
                if (validationList.Count() != x.index + 1)
                {
                    insPart = insPart + x.item.itemName + " = " + formatType + "," + Environment.NewLine;
                }
                else
                {
                    insPart = insPart + x.item.itemName + " = " + formatType;
                }
            }
            instanceTxt.Text = string.Format(insBase, insPart);

            //バリデーションコード出力
            string valBase = @"switch (columnName){{" + Environment.NewLine + "{0} " + Environment.NewLine + "default:" + Environment.NewLine + "}}" + Environment.NewLine;
            string valPartBase = @"case ""{0} "":";
            string valPart = "";
            foreach (var x in validationList.Select((item, index) => new { item, index }))
            {
                //case文出力
                if (validationList.Count() != x.index + 1)
                {
                    valPart = valPart + string.Format(valPartBase, x.item.itemName) + Environment.NewLine;
                }
                else
                {
                    valPart = valPart + string.Format(valPartBase, x.item.itemName);
                }
                //Not Nullの場合
                if (!x.item.isNull)
                {
                    valPart = valPart + Environment.NewLine + @"if (" + x.item.itemName + @" == null) return """ + x.item.itemName + @"は Null にできません。""";
                }

                //文字列長ありの場合
                if(x.item.length != null)
                {
                    valPart = valPart + Environment.NewLine + @"";
                }

                //最大

                //最小

                //半角数字
                if (x.item.hankakuSuu)
                {
                    valPart = valPart + Environment.NewLine + @"if (Regex.IsMatch(" + x.item.itemName + @".ToString(), @""^[0 - 9]+$"") == false) return ""半角数字[0-9]"";";
                }

                //半角英数字
                if (x.item.hankakuEisuu)
                {
                    valPart = valPart + Environment.NewLine + @"if (Regex.IsMatch(" + x.item.itemName + @".ToString(), @""^[0-9a-zA-Z]+$"") == false) return ""半角英数字[0-9][a-z][A-Z]"";";
                }

                //半角英数字（記号あり）
                //if (x.item.hankakuEisuu)
                //{
                //    valPart = valPart + Environment.NewLine + @"if (Regex.IsMatch(" + x.item.itemName + @".ToString(), @""^[!-~]+$"") == false) return ""	半角英数字[0-9][a-z][A-Z]半角記号"";";
                //}

                //半角帯小数
                if (x.item.hankakuObi)
                {
                    valPart = valPart + Environment.NewLine + @"if (Regex.IsMatch(" + x.item.itemName + @".ToString(), @""^[0-9]+(\.[0-9]{1,4})?$"") == false) return ""半角帯小数"";";
                }

                //break文
                valPart = valPart + Environment.NewLine + "break;";
            }
            validationTxt.Text = string.Format(valBase, valPart);

            //テストコード
            testCodeTxt.Text = "";
        }

        private void InstanceTxt_Click(object sender, EventArgs e)
        {
            if (instanceTxt.Text == "")
            {
                return;
            }
            Clipboard.SetText(instanceTxt.Text);
            var CIform = new CopyInfoForm();
            CIform.Show();
            CIform.Refresh();
            System.Threading.Thread.Sleep(1000);
            CIform.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
