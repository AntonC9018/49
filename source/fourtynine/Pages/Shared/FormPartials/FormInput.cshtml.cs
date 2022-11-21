using fourtynine.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using InputType = fourtynine.Razor.InputType;

namespace fourtynine.Partials;

public record FormInputModel(
    string Label,
    string Expression,
    FormInputType InputType,
    string PlaceHolder)
{
    public string Id => Expression;
}

public enum FormInputType
{
    Text = InputType.Text,
    Email = InputType.Email,
    Password = InputType.Password,
    Number = InputType.Number,
    Date = InputType.Date,
    Url = InputType.Url,
    Tel = InputType.Tel,
    TextArea,
    Image,
    Dropdown,
}

public static class FormInputTypeExtensions
{
    public static string ToHtmlString(this FormInputType inputType)
    {
        return inputType switch
        {
            FormInputType.Text => "text",
            FormInputType.Email => "email",
            FormInputType.Password => "password",
            FormInputType.Number => "number",
            FormInputType.Date => "date",
            FormInputType.Url => "url",
            FormInputType.Tel => "tel",
            
            // TODO:
            FormInputType.TextArea => "text",
            FormInputType.Image => "file",
            FormInputType.Dropdown => "text",
            
            _ => throw new ArgumentOutOfRangeException(nameof(inputType), inputType, null),
        };
    }
}

[HtmlTargetElement("FormInput")]
public class FormInputTagHelper : PartialRazorTagHelperBase<FormInputModel>
{
    public FormInputTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
    {
    }
    
    public string? Label { get; set; }
    public ModelExpression? For { get; set; }
    public FormInputType InputType { get; set; } = FormInputType.Text;
    public string? Placeholder { get; set; }
    public string? PlaceHolder { get => Placeholder; set => Placeholder = value; }

    protected override FormInputModel CreateModel()
    {
        if (For is null)
            throw new ArgumentNullException(nameof(For));
        
        Label ??= For.Name[(For.Name.LastIndexOf('.') + 1) ..];
        PlaceHolder ??= "Enter " + Label;
        
        return new FormInputModel(
            Label: Label,
            Expression: For.Name,
            InputType: InputType,
            PlaceHolder: PlaceHolder);
    }
    
    protected override string PartialPath => "~/Pages/Shared/FormPartials/FormInput.cshtml";
}