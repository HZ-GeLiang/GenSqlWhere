using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ExpressionToSqlWhereClause.Helpers;

internal sealed class DebuggerHelper
{

    public static StackTraceInfo GetStackFrame()
    {
        StackFrame frame = new StackFrame(1, true);
        return new StackTraceInfo(frame);
    }


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

        Debugger.Break();
    }


}

/// <summary>
/// 堆栈信息
/// </summary>
internal sealed class StackTraceInfo
{
    //public StackFrame Frame { get; set; }
    //public MethodBase Method { get; set; }
    //public string MethodName { get; set; }
    //public string FileName { get; set; }
    //public int LineNumber { get; set; }

    public StackTraceInfo(StackFrame frame)
    {
        this.Frame = frame;
    }

    public StackFrame Frame { get; set; }
    public MethodBase Method => this.Frame.GetMethod();

    public string MethodName => GetMethodName();

    private string GetMethodName()
    {
        var method = this.Method.Name;
        if (method == "MoveNext")
        {
            method += "---" + this.Method.DeclaringType.Name;  //<GetDataInterfaceData>d__33
        }
        return method;
    }

    public string FileName => this.Frame.GetFileName();
    public int LineNumber => this.Frame.GetFileLineNumber();

    public string GetMethodFullName()
    {
        //返回类似 GetDynamicList_MethodParam(FieldsModel, List{DataInterfaceReqParameterInfo}, int, DataInterfaceEntity?, bool?, bool)
        var parameters = this.Method.GetParameters();
        string paramString = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name}{(p.IsOptional ? "?" : "")}"));
        string result = $"{MethodName}({paramString})";
        return result;
    }

    public string GetSqlTagWith() => GetSqlTagWith(string.Empty);

    /// <summary>
    ///
    /// </summary>
    /// <param name="methodSummary">方法说明</param>
    /// <param name="paraInfo">参数信息</param>
    /// <returns></returns>
    public string GetSqlTagWith(string methodSummary, object paraInfo)
    {
        StringBuilder sb = new StringBuilder();
        if (string.IsNullOrWhiteSpace(methodSummary) == false)
        {
            sb.Append(methodSummary);
            sb.Append(":");
        }

        sb.Append(DebuggerHelper.ToJson(paraInfo, true));

        var custom = sb.ToString();
        return GetSqlTagWith(custom);
    }

    /// <summary>
    /// 自定义信息
    /// </summary>
    /// <param name="custom"></param>
    /// <returns></returns>
    public string GetSqlTagWith(string custom)
    {
        MessageBuilder mb = MessageBuilder.WinLine();
        mb.AppendLine("方法名", this.GetMethodFullName());
        mb.AppendLine("文件", Path.GetFileName(this.FileName));
        mb.AppendLine("行", this.LineNumber);
        if (string.IsNullOrWhiteSpace(custom) == false)
        {
            mb.AppendLine(custom);
        }
        return mb;
    }
}