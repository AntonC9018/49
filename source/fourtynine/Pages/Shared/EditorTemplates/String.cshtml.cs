using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace fourtynine.Partials;

public interface IInputProperties
{
    public static readonly IInputProperties Default = new InputProperties();
    string? PlaceHolder { get; }
    string? Label { get; }
    string? InputType { get; }
    bool AddValidation { get; } 
}

public class InputProperties : IInputProperties
{
    public string? PlaceHolder { get; set; }
    public string? Label { get; set; }
    public string? InputType { get; set; } = "text";
    public bool AddValidation { get; set; }
}

public static class Extensions
{
    public static IInputProperties GetInputProperties(this ViewDataDictionary view)
    {
        return view.GetAdditionalData<IInputProperties>() ?? IInputProperties.Default;
    }
    
    public static T? GetAdditionalData<T>(this ViewDataDictionary view)
        where T : class
    {
        return view["AdditionalData"] as T;
    }
    
    public static T GetRequiredAdditionalData<T>(this ViewDataDictionary view)
        where T : class
    {
        return view.GetAdditionalData<T>()
            ?? throw new InvalidOperationException("Additional data not found");
    }
    
    public static string GetLabel(this ViewDataDictionary view, IInputProperties properties)
    {
        string label;
        if (properties.Label != null)
        {
            label = properties.Label;
        }
        else
        {
            var t = view.TemplateInfo.FormattedModelValue.ToString();
            if (string.IsNullOrEmpty(t))
                t = view.ModelExplorer.Metadata.PropertyName;
            label = t ?? "Label";
        }
        return label;
    }

    public static IHtmlContent _MakeEditor<TModel, TResult, TAdditionalData>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TResult>> expression,
        TAdditionalData additionalData)
        where TAdditionalData : class
    {
        return htmlHelper.EditorFor(expression,
            additionalViewData: new { AdditionalData = additionalData });
    }
    
    public static IHtmlContent MakeEditor<TModel>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, string>> expression,
        InputProperties? additionalData = null)
    {
        if (additionalData is null)
            return htmlHelper.EditorFor(expression);
        else
            return htmlHelper._MakeEditor(expression, additionalData);
    }
}