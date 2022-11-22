using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace fourtynine.Razor;

// <input type="...">
public enum InputType
{
    Text,
    Email,
    Password,
    Number,
    Date,
    Time,
    DateTimeLocal,
    Month,
    Week,
    Url,
    Tel,
    Search,
    Color,
    Range,
    CheckBox,
    Radio,
    File,
    Hidden,
}

// method Convert the above to stirngs
public static class InputTypeExtensions
{
    public static string ToHtmlString(this InputType inputType)
    {
        return inputType switch
        {
            InputType.Text => "text",
            InputType.Email => "email",
            InputType.Password => "password",
            InputType.Number => "number",
            InputType.Date => "date",
            InputType.Time => "time",
            InputType.DateTimeLocal => "datetime-local",
            InputType.Month => "month",
            InputType.Week => "week",
            InputType.Url => "url",
            InputType.Tel => "tel",
            InputType.Search => "search",
            InputType.Color => "color",
            InputType.Range => "range",
            InputType.CheckBox => "checkbox",
            InputType.Radio => "radio",
            InputType.File => "file",
            InputType.Hidden => "hidden",
            _ => throw new ArgumentOutOfRangeException(nameof(inputType), inputType, null),
        };
    }
}


public abstract class PartialRazorTagHelperBase<TModel> : TagHelper
{
    protected IHtmlHelper HtmlHelper { get; }

    public PartialRazorTagHelperBase(IHtmlHelper htmlHelper)
    {
        HtmlHelper = htmlHelper;
    }
    
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;
    
    protected abstract TModel CreateModel();
    protected abstract string PartialPath { get; }
    
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var model = CreateModel();
        var partialPath = PartialPath;
        
        (HtmlHelper as IViewContextAware)?.Contextualize(ViewContext);
        
        var content = await HtmlHelper.PartialAsync(partialPath, model);
        
        output.TagName = "";
        output.Content.SetHtmlContent(content);
    } 
}
