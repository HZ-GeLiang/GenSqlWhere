using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ExpressionToSqlWhereClause.Helpers;

internal sealed class DebuggerHelper
{
    /// <summary>
    /// 只在deubg下生效
    /// </summary>
    /// <param name="ex">异常信息</param>
    /// <param name="memberName">调用此方法的方法名</param>
    /// <param name="fileName">调用此方法的文件名</param>
    /// <param name="lineNumber">调用此方法的文件的行号</param>
    [Conditional("DEBUG")]
    public static void Break(Exception ex = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string fileName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        //方便调试预览
        var message = ex?.Message;
        var message_inner = ex?.InnerException?.Message;
    }
}