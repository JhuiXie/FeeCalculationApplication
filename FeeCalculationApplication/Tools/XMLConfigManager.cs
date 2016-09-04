using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using FeeCalculationApplication.Models;

namespace FeeCalculationApplication.Tools
{
    public class XMLConfigManager
    {
        /// <summary>
        /// 一次性读取配置文件
        /// </summary>
        public List<FeeModel> LoadConfig()
        {
            List<FeeModel> modelList = new List<FeeModel>();
            try
            {
                XmlDocument xml = new XmlDocument();
                string xmlfile = System.IO.Directory.GetCurrentDirectory() + "\\config.xml";
                if (!File.Exists(xmlfile))
                {
                    throw new Exception("配置文件不存在，路径：" + xmlfile);
                }
                xml.Load(xmlfile);
                //数据库连接字符串
                if (xml.GetElementsByTagName("CostModels").Count > 0)
                {
                    foreach (XmlNode item in xml.DocumentElement["CostModels"].ChildNodes)
                    {
                        FeeModel tempModel = new FeeModel();
                        tempModel.ModelName = item["ModelName"].InnerText.Trim();
                        tempModel.R = Convert.ToDouble(item["R"].InnerText.Trim());
                        tempModel.L = Convert.ToDouble(item["L"].InnerText.Trim());
                        tempModel.Cost = Convert.ToDouble(item["COST"].InnerText.Trim());
                        modelList.Add(tempModel);
                    }
                }
                return modelList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }

}
