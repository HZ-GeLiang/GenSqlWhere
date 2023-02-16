using System;
using System.Text.RegularExpressions;

class Program
{
    static string RemoveExtraParentheses(string sql)
    {
        // 用正则表达式匹配括号中的内容，并将其替换为一个占位符
        string pattern = @"\([^()]+\)";
        int count = 0;
        sql = Regex.Replace(sql, pattern, match => $"{{{count++}}}");

        // 用正则表达式匹配多余的括号，并去除它们
        pattern = @"\(([^(){}]+)\)";
        while (Regex.IsMatch(sql, pattern))
        {
            sql = Regex.Replace(sql, pattern, "$1");
        }

        // 将占位符替换回括号中的内容
        pattern = @"\{(\d+)\}";
        sql = Regex.Replace(sql, pattern, match => $"({match.Groups[1].Value})");

        return sql;
    }

    static string RemoveExtraParentheses1(string sql)
    {
        // 用正则表达式匹配连续的多余括号，并去除它们
        string pattern = @"\((\([^()]+\))+\)";
        while (Regex.IsMatch(sql, pattern))
        {
            sql = Regex.Replace(sql, pattern, match => match.Value.Replace("(", "").Replace(")", ""));
        }
        return sql;
    }
    static string RemoveExtraParentheses2(string sql)
    {
        Regex pattern = new Regex(@"(\()|(\))");
        MatchCollection matches = pattern.Matches(sql);
        int count = 0;
        foreach (Match match in matches)
        {
            if (match.Value == "(")
            {
                count++;
            }
            else if (match.Value == ")")
            {
                count--;
                if (count < 0)
                {
                    throw new ArgumentException("Invalid SQL query: extra closing parentheses found");
                }
            }
        }
        if (count != 0)
        {
            throw new ArgumentException("Invalid SQL query: extra opening parentheses found");
        }
        return pattern.Replace(sql, match =>
        {
            if (match.Value == "(" && match.Index > 0 && sql[match.Index - 1] == ' ')
            {
                return match.Value;
            }
            else if (match.Value == ")" && match.Index < sql.Length - 1 && sql[match.Index + 1] == ' ')
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        });
    }

    static void Main()
    {
        string condition = "((((Sex = @Sex) And ((Age > @Age))) Or (((Sex = @Sex1)) And ((Age > @Age1)))) And (((Name = @Name)) Or (Name Like @Name1)))";

          condition = "((Id In (@Id)) And (Sex In (@Sex)))";
        string pattern = @"\((?>[^()]+|\((?<DEPTH>)|\)(?<-DEPTH>))*(?(DEPTH)(?!))\)";

        MatchCollection matches = Regex.Matches(condition, pattern);

        foreach (Match match in matches)
        {
            Console.WriteLine(match.Value);
        }


        //sql = RemoveExtraParentheses1(sql);
        //sql = RemoveExtraParentheses2(sql);
        // "SELECT * FROM ExpressionSqlTest WHERE (Sex = @Sex AND Age > @Age OR Sex = @Sex1 AND Age > @Age1) AND (Name = @Name OR Name Like @Name1)"
    }
}
