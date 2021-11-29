using System.Collections.Generic;
using PredicateBuilder;

namespace ExpressionToWhereClause.Test._whereLabmdaTest_
{
    class Test
    {
        public static WhereLambda<Route, RoutePageInput> GetWhereLambda()
        {
            var searchModel = new RoutePageInput()
            {
                //Id = 1,
                Id = "1,2",
                Sex = "1",
                IsDel = true,
                Url = "123",
                DataCreatedAtStart = System.DateTime.Now.AddHours(-1),
                DataCreatedAtEnd = System.DateTime.Now,
            };

            var whereLambda = new WhereLambda<Route, RoutePageInput>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.like] = new List<string>
            {
                nameof(searchModel.Url),
                nameof(searchModel.Data_Remark),
            };

            whereLambda[SearchType.eq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };
            whereLambda[SearchType.@in] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.Sex),
            };

            whereLambda[SearchType.datetimeRange] = new List<string>
            {
                nameof(searchModel.DataCreatedAtStart),
                nameof(searchModel.DataCreatedAtEnd),
                nameof(searchModel.DataUpdatedAtStart),
                nameof(searchModel.DataUpdatedAtEnd),
            };

            whereLambda[SearchType.numberRange] = new List<string>
            {
            };

            return whereLambda;
        }
    }
}
