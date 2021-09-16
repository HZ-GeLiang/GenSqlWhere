﻿using System.Collections.Generic;

namespace PredicateBuilder.Standard.Test
{
    class Test
    {
        public static WhereLambda<Route, RoutePageInput> GetWhereLambda()
        {
            var time = System.DateTime.Parse("2021-8-8");
            var searchModel = new RoutePageInput()
            {
                //Id = 1,
                Id = "1,2",
                Sex = "1",
                IsDel = true,//todo://计划:添加当其他值为xx时,当前值才生效
                Url = "123",
                DataCreatedAtStart = time.AddHours(-1),
                DataCreatedAtEnd = time,
            };

            var whereLambda = new WhereLambda<Route, RoutePageInput>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.Like] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };

            whereLambda[SearchType.Equal] = new List<string>
            {
                nameof(searchModel.IsDel),
            };
            whereLambda[SearchType.In] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.Sex),
            };

            whereLambda[SearchType.DateTimeRange] = new List<string>
            {
                nameof(searchModel.DataCreatedAtStart),
                nameof(searchModel.DataCreatedAtEnd),
                nameof(searchModel.DataUpdatedAtStart),
                nameof(searchModel.DataUpdatedAtEnd),
            };

            whereLambda[SearchType.NumberRange] = new List<string>
            {
            };

            return whereLambda;
        }
    }
}
