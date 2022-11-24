using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace fourtynine.EditorTemplates;

public interface IDropDownData
{
    public static readonly IDropDownData Default = new DropDownData();
    string? Label { get; }
    IEnumerable<SelectListItem>? Items { get; }
}

public class DropDownData : IDropDownData
{
    public string? Label { get; set; }
    public IEnumerable<SelectListItem>? Items { get; set;  }
}

public static class DropdownHelper
{
    public static IDropDownData GetDropDownData(this ViewDataDictionary view)
    {
        return view[nameof(IDropDownData)] as IDropDownData ?? IDropDownData.Default;
    }
    
    public static IHtmlContent DropDownFor<TModel, TResult>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TResult>> expression,
        DropDownData? dropDownData = null)
    {
        if (dropDownData is null)
            dropDownData = new DropDownData();
        
        if (dropDownData.Items is null)
        {
            if (!typeof(TResult).IsEnum)
                throw new ArgumentException("The dropdown items must be set explicitly, unless using an enum.");
            dropDownData.Items = htmlHelper.GetEnumSelectList(typeof(TResult));
        }
        
        Dictionary<string, object?> additionalData = new()
        {
            { nameof(IDropDownData), dropDownData }
        };
        return htmlHelper.EditorFor(expression, templateName: "DropDown", additionalData);
    }
}