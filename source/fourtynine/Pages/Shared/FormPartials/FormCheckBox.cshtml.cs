using System.Linq.Expressions;
using fourtynine.Razor;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace fourtynine.Partials;

public record FormCheckBoxModel(
    string Label,
    string Expression)
{
    public string Id => Expression;
}

[HtmlTargetElement("FormCheckBox")]
public class FormCheckBoxTagHelper : PartialRazorTagHelperBase<FormCheckBoxModel>
{
    public FormCheckBoxTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
    {
    }
    
    public string? Label { get; set; }
    public ModelExpression? For { get; set; }

    protected override FormCheckBoxModel CreateModel()
    {
        if (For is null)
            throw new ArgumentNullException(nameof(For));
        
        Label ??= For.Name[(For.Name.LastIndexOf('.') + 1) ..];
        
        return new FormCheckBoxModel(
            Label: Label,
            Expression: For.Name);
    }
    
    protected override string PartialPath => "~/Pages/Shared/FormPartials/FormCheckBox.cshtml";
}