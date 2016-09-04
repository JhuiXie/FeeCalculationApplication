namespace FeeCalculationApplication.Models
{
    public class FeeModel
    {
        /// <summary>
        /// 设置名称
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// 外径 mm
        /// </summary>
        public double R { get; set; }
        /// <summary>
        /// 长度 mm
        /// </summary>
        public double L { get; set; }
        /// <summary>
        /// 加工价格 元
        /// </summary>
        public double Cost { get; set; }
    }
}
