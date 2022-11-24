import {
    ApiPropertyTable, Client,
    PostingCreate, ProblemDetails,
} from "./api-client";

export {}

initializePostingForm();

function initializePostingForm()
{
    const form = document.querySelector(`form[name="postingForm"]`) as HTMLFormElement;
    validateForm(form, "PostingCreate", "Posting.");
    
    // It would've been way easier to do with a framework.
    form.querySelectorAll(`div[enable-when]`).forEach(div =>
    {
        const togglerId = div.getAttribute("enable-when")!;
        const toggler = document.getElementById(togglerId) as HTMLInputElement;
        if (!toggler)
            throw new Error(`Toggler with id '${togglerId}' not found.`);

        function toggle()
        {
            div!.classList.toggle("hidden", !toggler.checked);
        }
        toggler.addEventListener("click", toggle);
        toggle();
    });
    
    const client = new Client();
    form.addEventListener("submit", async function (event)
    {
        event.preventDefault();
        const form = event.target as HTMLFormElement;
        const formData = new FormData(form);

        // @ts-ignore
        const dto = mapShallowObject(formData) as PostingCreate;
        try
        {
            const response = await client.postingPOST(dto);
            window.history.pushState({}, "", 
                `/postings/${response.General.Id}/${response.General.Slug}`);            
        }
        catch (e: any)
        {
            // if (!ApiException.isApiException(e))
            //     throw e;
            console.log(e);
            
            // Temporary thing.
            function toast(str: string)
            {
                const toast = document.createElement("div");
                toast.classList.add("toast");
                toast.innerText = str;
                document.body.appendChild(toast);
                setTimeout(() => toast.remove(), 5000);
                
                console.error(str);
            }

            const serverException = e as ProblemDetails;
            if (serverException && serverException.errors)
            {
                console.error(serverException.title);
                for (let [field, error] of Object.entries(serverException.errors))
                    console.error(`${field}: ${error}`);
            }

            switch (e.status)
            {
                case 400:
                {
                    toast("Bad Request 400:" + e.result);
                    break;
                }
                case 401:
                    toast("Unauthorized 401:" + e.response);
                    break;
                case 403:
                    toast("Forbidden 403:" + e.response);
                    break;
                case 404:
                    toast("Not Found 404:" + e.response);
                    break;
            }
        }
    });
}

type ApiSchemaName = keyof typeof ApiPropertyTable;

function validateForm(form: HTMLFormElement, schemaName: ApiSchemaName, namePrefix = "")
{
    console.assert(form != null);

    const dtoKeys = new Set<string>(getDeepPropertyKeys(schemaName));
    let foundKeys = new Set<string>();
    for (let i = 0; i < form.elements.length; i++)
    {
        const element = form.elements[i];
        let name = element.getAttribute('name');
        if (!name)
            continue;
        // I don't know how to handle this yet.
        if (name == "__RequestVerificationToken")
            continue;
        if (!name.startsWith(namePrefix))
            continue;
        
        name = name.substring(namePrefix.length);
        
        if (dtoKeys.has(name))
            foundKeys.add(name);
        else
            console.error("Form element with name '" + name + "' is not part of the DTO.");
    }
    let difference = new Set([...dtoKeys].filter(x => !foundKeys.has(x)));
    if (difference.size > 0)
        console.error("The following keys were not found in the form: " + Array.from(difference).join(", "));
}

function getDeepPropertyKeys(schemaName: ApiSchemaName) : string[]
{
    let output : string[] = [];
    _getDeepPropertyKeys(schemaName, "", output);
    return output;
}

function _getDeepPropertyKeys(type: ApiSchemaName, prefix: string, output : string[])
{
    for (let prop of ApiPropertyTable[type])
    {
        let p = prefix + prop.name;
        if (prop.schemaTypeName)
            _getDeepPropertyKeys(<ApiSchemaName> prop.schemaTypeName, p + ".", output);
        else
            output.push(p);
    }
}

// @ts-ignore
function findDeepKeys(obj: Record<string, any>)
{
    let output = new Set<string>();
    _findDeepKeys(obj, "", output);
    return output;
}

function _findDeepKeys(obj: Record<string, any>, prefix: string, output : Set<string>)
{
    for (let key in obj)
    {
        if (!obj.hasOwnProperty(key))
            continue;
        
        let newPrefix = prefix + key + ".";
        if (typeof obj[key] == "object")
            _findDeepKeys(obj[key], newPrefix, output);
        else
            output.add(newPrefix);
    }
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
