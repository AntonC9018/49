import {
    ApiPropertyTable, Client,
    PostingCreate, PostingKind, ProblemDetails,
} from "./api-client";

export {}

initializePostingForm();

function initializePostingForm()
{
    const form = document.querySelector(`form[name="postingForm"]`) as HTMLFormElement;
    validateForm(form, "PostingCreate", "Posting.");
    
    // Toggleable sections.
    const detailsTogglers : Array<HTMLInputElement> = [];
    // It would've been way easier to do with a framework.
    form.querySelectorAll(`div[enable-when]`).forEach(div =>
    {
        const togglerId = div.getAttribute("enable-when")!;
        const toggler = document.getElementById(togglerId) as HTMLInputElement;
        if (!toggler)
            throw new Error(`Toggler with id '${togglerId}' not found.`);
        
        detailsTogglers.push(toggler);
        function toggle()
        {
            div!.classList.toggle("hidden", !toggler.checked);
        }
        toggler.addEventListener("click", toggle);
        toggle();
    });
    
    // Discriminated union of posting details.
    const postingDetailsDivs : Array<HTMLDivElement> = [];
    for (const detailKind in PostingKind)
    {
        // Looping over only values requires a hack
        if (isNaN(+detailKind))
            continue;
        
        const input = form.querySelector(`div[for-kind='${PostingKind[detailKind]}']`) as HTMLDivElement;
        if (input == null)
            throw new Error(`Input for kind '${PostingKind[detailKind]}' not found.`);
        postingDetailsDivs.push(input);
    }
    
    const kindDropdown = form.querySelector("select[name='Posting.Details.Kind']")! as HTMLSelectElement;
    function toggleDetailsUnion()
    {
        for (const div of postingDetailsDivs)
            div.classList.toggle("hidden", true);
        postingDetailsDivs[+kindDropdown.value].classList.toggle("hidden", false);
    }
    kindDropdown.addEventListener("change", toggleDetailsUnion);
    toggleDetailsUnion();
    
    const client = new Client();
    form.addEventListener("submit", async function (event)
    {
        console.log("Submitting form...");
        
        event.preventDefault();
        const form = event.target as HTMLFormElement;
        const formData = new FormData(form);

        // This does not take into account the actual expected type of the model.
        // There is no easy way to do this, because typescript has no reflection,
        // while the reflection package is buggy and does not work with unions with undefined.
        // See issue: https://github.com/Hookyns/tst-reflect/issues/83
        // Hence there's no easy way to check for nulls either.
        // I'm surprised how bad the javascript solutions for this are,
        // nobody knows how to do this conversion reliably.
        // I'm not a javascript mastermind, so I'll settle for the mediocre solution for now.
        const dto = mapFormDataToObject(formData)["Posting"] as PostingCreate;
        
        if (dto.Details != null)
        {
            // Set unchecked details to null.
            for (const toggler of detailsTogglers)
            {
                if (toggler.checked)
                    continue;
                const propName = toggler.id.substring("Posting.Details.".length);
                delete (<any> dto.Details)[propName];
            }
            
            // Only leave the relevant discriminated union bit.
            const selectedKind = +kindDropdown.value;
            console.log(postingDetailsDivs)
            for (let i = 0; i < postingDetailsDivs.length; i++)
            {
                if (selectedKind === i)
                    continue;
                const key = PostingKind[i];
                delete (<any> dto.Details)[key];
            }
        }
        
        try
        {
            const response = await client.postingPOST(dto);
            window.location.assign(`/Posting/${response.General.Id}/${response.General.Slug}`);
        }
        catch (e: any)
        {            
            // Temporary thing.
            function toast(str: string)
            {                
                console.error(str);
            }

            form.querySelectorAll("[data-valmsg-for]")
                .forEach(validationMessageSpan =>
                {
                    validationMessageSpan.innerHTML = "";
                });
            
            const serverException = e as ProblemDetails;
            if (serverException && serverException.errors)
            {
                for (let [field, error] of Object.entries(serverException.errors))
                {
                    const errorString = error?.toString();
                    if (!errorString)
                        continue;

                    const validationMessageSpan = form.querySelector(`[data-valmsg-for="Posting.${field}"]`);
                    if (validationMessageSpan)
                        validationMessageSpan.textContent = errorString;
                    else
                        console.log(`Field '${field}' not found. Error message: ${error}`);
                }
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
        if (!name || !name.startsWith(namePrefix))
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
        if (prop.schemaTypeName && prop.schemaTypeName !== "number")
            _getDeepPropertyKeys(<ApiSchemaName> prop.schemaTypeName, p + ".", output);
        else
            output.push(p);
    }
}

function mapFormDataToObject(formData: FormData) : Record<string, any>
{
    let result : Record<string, any> = {};
    
    formData.forEach((entry, key) =>
    {
        function convert(str: string|File) : any
        {
            if (str === null || str === "")
                return null;
            
            let number = Number(str);
            if (!isNaN(number))
                return number;
            
            if (str === "true")
                return true;
            if (str === "false")
                return false;
            
            return str;
        }
        
        const value = convert(entry);
        if (value === null)
            return;
        
        const parts = key.split('.');
        let current = result;
        for (let i = 0; i < parts.length - 1; i++)
        {
            let part = parts[i];
            if (!current[part])
                current[part] = {};
            current = current[part];
        }
        const lastPart = parts[parts.length - 1];
        current[lastPart] = value;
    });
    return result;
}
