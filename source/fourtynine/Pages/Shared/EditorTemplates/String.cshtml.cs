using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace fourtynine.EditorTemplates;

public interface IInputProperties
{
    public static readonly IInputProperties Default = new InputProperties();
    string? PlaceHolder { get; }
    string? Label { get; }
    string? InputType { get; }
}

public class InputProperties : IInputProperties
{
    public string? PlaceHolder { get; set; }
    public string? Label { get; set; }
    public string? InputType { get; set; } = "text";
}

public static class Extensions
{
    public static IInputProperties GetInputProperties(this ViewDataDictionary view)
    {
        return view[nameof(IInputProperties)] as IInputProperties ?? IInputProperties.Default;
    }
    
    public static string GetLabel(this ViewDataDictionary view)
    {
        var t = view.TemplateInfo.FormattedModelValue.ToString();
        if (string.IsNullOrEmpty(t))
            t = view.ModelExplorer.Metadata.PropertyName;
        return t ?? "Label";
    }

    public static IHtmlContent InputFor<TModel, TResult>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TResult>> expression,
        InputProperties? inputProperties = null)
    {
        if (IsNumericType(typeof(TResult)))
        {
            inputProperties ??= new InputProperties();
            inputProperties.InputType = "number";
        }
        Dictionary<string, object?> additionalData = new()
        {
            { nameof(IInputProperties), inputProperties }
        };
        return htmlHelper.EditorFor(expression, templateName: "String", additionalData);
    }
    
    private static bool IsNumericType(this Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;
            default:
                return type.IsEnum;
        }
    }
}