using fourtynine.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace fourtynine.Partials;

public record CheckBoxModel(
    string Label,
    string Expression)
{
    public string Id => Expression;
}

[HtmlTargetElement("CheckBox")]
public class CheckBoxTagHelper : PartialRazorTagHelperBase<CheckBoxModel>
{
    public CheckBoxTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
    {
    }
    
    public string? Label { get; set; }
    public ModelExpression? For { get; set; }

    protected override CheckBoxModel CreateModel()
    {
        if (For is null)
            throw new ArgumentNullException(nameof(For));
        
        Label ??= For.Name[(For.Name.LastIndexOf('.') + 1) ..];
        
        return new CheckBoxModel(
            Label: Label,
            Expression: For.Name);
    }
    
    protected override string PartialPath => "~/Pages/Shared/Partials/CheckBox.cshtml";
}