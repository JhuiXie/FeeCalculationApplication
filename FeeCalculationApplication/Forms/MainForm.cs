using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FeeCalculationApplication.Models;
using FeeCalculationApplication.Tools;

namespace FeeCalculationApplication
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 储存界面上的数据
        /// </summary>
        Dictionary<string, double> valueDictionary = new Dictionary<string, double>();
        /// <summary>
        /// 储存输入数据的输入框
        /// </summary>
        List<TextBox> boxList = new List<TextBox>();
        /// <summary>
        /// 储存自动输入的信息
        /// </summary>
        List<FeeModel> modelList = new List<FeeModel>();
        public MainForm()
        {
            InitializeComponent();
        }


        #region 计算公式
        /// <summary>
        /// 计算重量
        /// </summary>
        /// <param name="r">外径</param>
        /// <param name="h">厚度</param>
        /// <param name="l">长度</param>
        /// <returns></returns>
        double weightCalculation(double r, double h, double l)
        {
            double result = (r - h) * h * 0.02491 * l * 0.001;

            return result;
        }

        /// <summary>
        /// 计算成本
        /// </summary>
        /// <param name="weight">重量</param>
        /// <param name="feCost">钢材价格</param>
        /// <param name="washCost">酸洗价格</param>
        /// <param name="unfireCost">退火价格</param>
        /// <param name="workCost">加工价格</param>
        /// <param name="flashCost">抛光价格</param>
        /// <returns></returns>
        double costCalculation(double weight, double feCost, double washCost, double unfireCost, double workCost, double flashCost)
        {
            double result = weight * (feCost + washCost + unfireCost) + workCost + flashCost;

            return result;
        }

        /// <summary>
        /// 计算出厂价
        /// </summary>
        /// <param name="cost">成本</param>
        /// <param name="cash">利润</param>
        /// <returns></returns>
        double outPrice(double cost, double cash)
        {
            double result = Math.Round((cost * (1 + cash / 100)), 4);

            return result;
        }

        /// <summary>
        /// 计算含税价
        /// </summary>
        /// <param name="outPrice">出厂价</param>
        /// <returns></returns>
        double finalPrice(double outPrice)
        {
            double result = Math.Round((outPrice * 1.14), 4);

            return result;
        }

        #endregion

        #region 方法
        /// <summary>
        /// 进行计算
        /// </summary>
        void doCalculation()
        {
            if (checkValue())
            {

                tbWeight.Text = weightCalculation(valueDictionary["tbR"], valueDictionary["tbH"], valueDictionary["tbL"]).ToString();
                tbOutprice.Text = outPrice(costCalculation(Convert.ToDouble(tbWeight.Text), valueDictionary["tbFeCost"],
                    valueDictionary["tbWashCost"], valueDictionary["tbUnfireCost"], valueDictionary["tbWorkCost"], valueDictionary["tbFlashCost"]), valueDictionary["tbCash"]).ToString();
                tbFinalPrice.Text = finalPrice(Convert.ToDouble(tbOutprice.Text)).ToString();
            }
        }

        /// <summary>
        /// 验证输入合法性
        /// </summary>
        /// <returns></returns>
        bool checkValue()
        {
            try
            {
                saveValue();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 将数据保存到内存中
        /// </summary>
        void saveValue()
        {
            foreach (TextBox item in boxList)
            {

                if (!valueDictionary.ContainsKey(item.Name))
                {
                    if (Regex.IsMatch(item.Text, @"^[+-]?\d*[.]?\d*$"))
                    {
                        item.ForeColor = Color.Black;
                        double value = Convert.ToDouble(item.Text);
                        valueDictionary.Add(item.Name, value);
                    }
                    else
                    {
                        item.ForeColor = Color.Red;
                        valueDictionary.Add(item.Name, 0);
                    }
                }
                else
                {
                    if (Regex.IsMatch(item.Text, @"^[+-]?\d*[.]?\d*$"))
                    {
                        item.ForeColor = Color.Black;
                        double value = Convert.ToDouble(item.Text);
                        valueDictionary[item.Name] = value;
                    }
                    else
                    {
                        item.ForeColor = Color.Red;
                        valueDictionary[item.Name] = 0;
                    }
                }
            }
        }
        #endregion

        #region 事件
        private void tbR_TextChanged(object sender, EventArgs e)
        {
            if ((sender as TextBox).Name == "tbR")
            {
                if (modelList != null && checkValue())
                {
                    FeeModel tempModel = modelList.Find(m => m.R == Convert.ToDouble((sender as TextBox).Text.Trim()));
                    if (tempModel != null)
                    {
                        tbL.Text = tempModel.L.ToString();
                        tbWorkCost.Text = tempModel.Cost.ToString();
                    }
                }

            }

            foreach (var item in boxList)
            {
                if (item.Text == "")
                {
                    item.Text = "0";
                    item.SelectAll();
                }
            }
            try
            {
                doCalculation();
            }
            catch (Exception)
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            XMLConfigManager configManager = new XMLConfigManager();

            modelList = configManager.LoadConfig();

            //将控件放入内存表并设置第一次单击全选事件
            foreach (Control item in groupBox2.Controls)
            {
                if (item is TextBox)
                {
                    boxList.Add(item as TextBox);
                    item.Tag = false;
                    item.GotFocus += new EventHandler(textBox_GotFocus);
                    item.MouseUp += new MouseEventHandler(textBox_MouseUp);
                }
            }

            foreach (Control item in groupBox1.Controls)
            {
                item.Tag = false;
                item.GotFocus += new EventHandler(textBox_GotFocus);
                item.MouseUp += new MouseEventHandler(textBox_MouseUp);
            }
        }
        void textBox_MouseUp(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            //如果鼠标左键操作并且标记存在，则执行全选             
            if (e.Button == MouseButtons.Left && (bool)textBox.Tag == true)
            {
                textBox.SelectAll();
            }

            //取消全选标记              
            textBox.Tag = false;
        }
        void textBox_GotFocus(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Tag = true;    //设置标记              
            textBox.SelectAll();
        }


        #endregion
    }
}
