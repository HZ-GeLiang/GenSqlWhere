using System.Collections.Generic;

namespace PredicateBuilder.Standard.Test
{
    class TestData
    {
        public static WhereLambda<People, PeoplePageInput> GetWhereLambda()
        {
            var time = System.DateTime.Parse("2021-8-8");
            var searchModel = new PeoplePageInput()
            {
                //Id = 1,
                Id = "1,2",
                Sex = "1",
                IsDel = true,//todo://计划:添加当其他值为xx时,当前值才生效
                Url = "123",
                DataCreatedAtStart = time.AddHours(-1),
                DataCreatedAtEnd = time,
            };

            var whereLambda = new WhereLambda<People, PeoplePageInput>();
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

        public static WhereLambda<People, PeoplePageInput2> GetWhereLambda2()
        {
            var time = System.DateTime.Parse("2021-8-8");
            var searchModel = new PeoplePageInput2()
            {
                DataCreatedAt = time,
            };

            var whereLambda = new WhereLambda<People, PeoplePageInput2>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.DateTimeRange] = new List<string>
            {
                nameof(searchModel.DataCreatedAt)
            };

         
            return whereLambda;
        }

        public static WhereLambda<People, PeoplePageInput2> GetWhereLambda3()
        {
            var time = System.DateTime.Parse("2021-8-8");
            var searchModel = new PeoplePageInput2()
            {
                DataCreatedAt = time,
            };

            var whereLambda = new WhereLambda<People, PeoplePageInput2>();
            whereLambda.SearchModel = searchModel;

            //todo:一个日期符号是可配置的.  如 createAt＞＝　
            whereLambda[SearchType.DateTimeRange] = new List<string>
            {
                nameof(searchModel.DataCreatedAt)
            };


            return whereLambda;
        }
    }
}
