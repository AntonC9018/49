import './api-client';
import {
    PostingCreate,
} from "./api-client";
import {getType,Type} from "tst-reflect";

export {}


class A
{
    s?: string | undefined;
}
console.warn(getType<number|string>().types);
console.warn(getType<A>().getProperties().find(p => p.name == "s")!.type.types);

console.warn("sdsdds");


initializePostingForm();

function initializePostingForm()
{
    const form = document.querySelector(`form[name="postingForm"]`) as HTMLFormElement;
    validateForm(form, getType<PostingCreate>());

    form.addEventListener("submit", function (event)
    {
        event.preventDefault();
        const form = event.target as HTMLFormElement;
        const formData = new FormData(form);

        // @ts-ignore
        const dto = mapShallowObject(formData) as PostingCreate;
        
        debugger;
    });
}

function validateForm(form: HTMLFormElement, type: Type)
{
    console.assert(form != null);

    const dtoKeys = new Set<string>(getDeepPropertyKeys(type));
    let foundKeys = new Set<string>();
    for (let i = 0; i < form.elements.length; i++)
    {
        const element = form.elements[i];
        const name = element.getAttribute('name');
        if (!name)
            continue;
        
        // I don't know how to handle this yet.
        if (name == "__RequestVerificationToken")
            continue;
        
        if (dtoKeys.has(name))
            foundKeys.add(name);
        else
            console.error("Form element with name '" + name + "' is not part of the DTO.");
    }
    let difference = new Set([...dtoKeys].filter(x => !foundKeys.has(x)));
    if (difference.size > 0)
        console.error("The following keys were not found in the form: " + Array.from(difference).join(", "));
}

function getDeepPropertyKeys(type: Type) : string[]
{
    let output : string[] = [];
    _getDeepPropertyKeys(type, "", output);
    return output;
}

function _getDeepPropertyKeys(type: Type, prefix: string, output : string[])
{
    for (let prop of type.getProperties())
    {
        let p = prefix + prop.name;
        console.log(prop.type);
        if (prop.type.isInterface())
            _getDeepPropertyKeys(prop.type, p + ".", output);
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
