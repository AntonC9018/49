import './index.css';
import {getType,Type} from "tst-reflect";

// Hack
// https://github.com/Hookyns/tst-reflect/issues/81
(window as any)["_" + String.fromCharCode(223) + "r"] = { getType, Type };

// @ts-ignore
function setGlobal(key: string, value: any)
{
    (window as any)[key] = value;
}
