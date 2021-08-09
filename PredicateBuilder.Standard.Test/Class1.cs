using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PredicateBuilder.Standard.Test
{
    class Class1
    {
        public static void Test()
        {
            var s1 = "a+(b+c)-d";
            var s2 = "a+b/(c+d)";
            var s3 = "(a*b)+c/d";
            var s4 = "((a+b)*f)-(i/j)";
            var s5 = "list[(i)]";
            var s6 = "list[(i + 1)]";
            var s7 = "(age > @age)";
            var s8 = "(age > @age and Id > @Id)";
            var s9 = "((age > @age) and (Id > @Id))";

            var r1 = "a+b+c-d";
            var r2 = "a+b/(c+d)";
            var r3 = "a*b+c/d";
            var r4 = "(a+b)*f-i/j";
            var r5 = "list[i]";
            var r6 = "list[i + 1]";
            var r7 = "age > @age";
            var r8 = "age > @age and Id > @Id";
            var r9 = "age > @age and Id > @Id";


            List<string> listInputs = new List<string> { s1, s2, s3, s4, s5, s6, s7, s8, s9 };
            List<string> listResult = new List<string> { r1, r2, r3, r4, r5, r6, r7, r8, r9 };

            for (var index = 0; index < listInputs.Count; index++)
            {
                var input = listInputs[index];
                if (index + 1 == 5)
                {
                    int a = 3;

                }
                var result = Class1.RemoveBracket(input);
                Console.WriteLine((index + 1).ToString() + ": " + (result == listResult[index]));
            }

        }
        public static string RemoveBracket(string inputStr)
        {
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                return inputStr;
            }

            var listChar = new List<char>(inputStr.Length);
            foreach (char item in inputStr)
            {
                listChar.Add(item);
            }

            for (int i = 0; i < listChar.Count; i++)
            {
                if (listChar[i] == '+' || listChar[(i)] == '-')
                {
                    if (listChar[(i - 1)] == ')' && listChar[(i + 1)] == '(') //   ((a+b)*f)-(i/j)  的 )-(
                    {
                        listChar.RemoveAt(i - 1);
                        listChar.RemoveAt(i);
                        for (int j = i - 1; j > -1; j--)
                        {
                            if (listChar[(j)] == '(')
                            {
                                listChar.RemoveAt(j);
                                break;
                            }
                        }
                        for (int k = i + 1; k < listChar.Count; k++)
                        {
                            if (listChar[(k)] == ')')
                            {
                                listChar.RemoveAt(k);
                                break;
                            }
                        }
                    }
                    if (listChar[(i - 1)] == ')')
                    {
                        listChar.RemoveAt(i - 1);
                        for (int j = i - 1; j > -1; j--)
                        {
                            if (listChar[(j)] == '(')
                            {
                                listChar.RemoveAt(j);
                                break;
                            }
                        }
                    }
                    if (listChar[(i + 1)] == '(')
                    {
                        if (listChar[(i)] == '+')
                            for (int k = i + 1; k < listChar.Count(); k++)
                            {
                                if (listChar[(k)] == ')' && !listChar.Contains('/') && !listChar.Contains('*'))
                                {
                                    listChar.RemoveAt(k);
                                    listChar.RemoveAt(i + 1);
                                    break;
                                }
                            }
                    }
                }
            }
            //return list;

            StringBuilder sb = new StringBuilder(listChar.Count);

            foreach (var item in listChar)
            {
                sb.Append(item);
            }

            var result = sb.ToString();
            return result;
        }
    }
}
