import Dict = NodeJS.Dict;
import ReadOnlyDict = NodeJS.ReadOnlyDict;

export {}

function initializePostingForm()
{
    const form = document.querySelector(`form[name="postingForm"]`) as HTMLFormElement;
    console.assert(form != null);
    form.addEventListener("submit", function (event)
    {
        event.preventDefault();
        const form = event.target as HTMLFormElement;
        const formData = new FormData(form);
        // @ts-ignore
        const deepFormData = mapShallowObject(formData);
        
        
        debugger;
    });
}

// maps objects of form `{ "a.b": value }` to `{ a: { b: value } }`
function mapShallowObject(sourceObject: Record<string, any>)
{
    if (!sourceObject)
        return sourceObject;
    let result : Record<string, any> = {};
    for (let key in sourceObject)
    {
        // Hack. https://stackoverflow.com/a/16175212/9731532
        if (!sourceObject.hasOwnProperty(key))
            continue;

        const parts = key.split(".");
        let current = result;
        for (let i = 0; i < parts.length - 1; i++)
        {
            let part = parts[i];
            if (!current[part])
                current[part] = {};
            current = current[part];
        }
        const lastPart = parts[parts.length - 1];
        current[lastPart] = sourceObject[key];
    }
    return result;
}

document.addEventListener("DOMContentLoaded", initializePostingForm);
