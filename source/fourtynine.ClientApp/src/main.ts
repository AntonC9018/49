import './index.css';

// @ts-ignore
function setGlobal(key: string, value: any)
{
    (window as any)[key] = value;
}
