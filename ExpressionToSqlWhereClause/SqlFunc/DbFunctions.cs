﻿using System;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.SqlFunc
{
    public class DbFunctions
    {
        // Expression 用
        [DbFunctionAttribute("Month({0})")]
        public static int Month(DateTime dt)
        {
            throw new InvalidOperationException(nameof(Month));
        }

        [DbFunctionAttribute("Month({0})")]
        public static List<int> MonthIn(DateTime dt)
        {
            throw new InvalidOperationException(nameof(MonthIn));
        }
    }
}