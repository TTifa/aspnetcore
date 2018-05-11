using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class Bill
    {
        [Key]
        public int Id { get; set; }
        public int Uid { get; set; }
        public DateTime PayDate { get; set; }
        public decimal Amount { get; set; }
        public FlowType FlowType { get; set; }
        public string Remark { get; set; }
        public DateTime? LogTime { get; set; }
    }

    public enum FlowType
    {
        /*支出*/
        /// <summary>
        /// 食物
        /// </summary>
        Eating,
        /// <summary>
        /// 饮料
        /// </summary>
        Beverage,
        /// <summary>
        /// 交通
        /// </summary>
        Traffic,
        /// <summary>
        /// 游戏
        /// </summary>
        Game,
        /// <summary>
        /// 购物
        /// </summary>
        Shopping,
        /// <summary>
        /// 房租水电
        /// </summary>
        Rent,
        /// <summary>
        /// 还贷
        /// </summary>
        Credit,
        /// <summary>
        /// 通讯
        /// </summary>
        Communication,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 100,
        /*收入*/
        /// <summary>
        /// 工资
        /// </summary>
        Salary = 101,
        /// <summary>
        /// 理财
        /// </summary>
        Finance
    }

}
