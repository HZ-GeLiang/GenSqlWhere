using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionToSqlWhereClause.Test.InputOrModel
{
    public class PriceInfoInput
    {
        public int pageIndex { get; set; } = 1;
        public int pageSize { get; set; } = 10;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? DateStart { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? DateEnd { get; set; }

        internal DateTime? Date { get; set; } //冗余的

        //public long Id { get; set; }
        public long? Product_ID { get; set; }

        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Production { get; set; }

        /// <summary>
        ///  单位: 如美元/吨 
        /// </summary>
        public int? Unit { get; set; }
        public string Unit_Name { get; set; }

        /// <summary>
        /// 价格类型
        /// </summary>
        public int? Price_Type { get; set; }
        public string Price_Type_Name { get; set; }

        /// <summary>
        /// 趋势: 如: 平   
        /// </summary>
        public int? Price_Trend { get; set; }
        public string Price_Trend_Name { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public int? Fee_Type { get; set; }
        public string Fee_Type_Name { get; set; }

        /// <summary>
        /// 物流方式
        /// </summary>
        public int? Log_Type { get; set; }
        public string Log_Type_Name { get; set; }
        //public string Seq { get; set; }

        // 备注
        public string Remarks { get; set; }
        //public string InfoID { get; set; }
        public int? CreateUserID { get; set; }
        //public DateTime CreateDate { get; set; }

        public string SxJson { get; set; }
        // public Dictionary<string, string> SX { get; set; }

    }

}
