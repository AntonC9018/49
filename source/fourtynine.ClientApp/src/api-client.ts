//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v12.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

export class Client {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    /**
     * @return Success
     */
    navbarActions(): Promise<INavbarAction[]> {
        let url_ = this.baseUrl + "/api/NavbarActions";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processNavbarActions(_response);
        });
    }

    protected processNavbarActions(response: Response): Promise<INavbarAction[]> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(INavbarAction.fromJS(item));
            }
            else {
                result200 = <any>null;
            }
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<INavbarAction[]>(null as any);
    }

    /**
     * @return Success
     */
    postingGET(id: number): Promise<PostingGetGeneral> {
        let url_ = this.baseUrl + "/api/Posting/{id}";
        if (id === undefined || id === null)
            throw new Error("The parameter 'id' must be defined.");
        url_ = url_.replace("{id}", encodeURIComponent("" + id));
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processPostingGET(_response);
        });
    }

    protected processPostingGET(response: Response): Promise<PostingGetGeneral> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = PostingGetGeneral.fromJS(resultData200);
            return result200;
            });
        } else if (status === 404) {
            return response.text().then((_responseText) => {
            let result404: any = null;
            let resultData404 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result404 = ProblemDetails.fromJS(resultData404);
            return throwException("Not Found", status, _responseText, _headers, result404);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<PostingGetGeneral>(null as any);
    }

    /**
     * @return Success
     */
    detailed(id: number): Promise<PostingGetDetailed> {
        let url_ = this.baseUrl + "/api/Posting/detailed/{id}";
        if (id === undefined || id === null)
            throw new Error("The parameter 'id' must be defined.");
        url_ = url_.replace("{id}", encodeURIComponent("" + id));
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processDetailed(_response);
        });
    }

    protected processDetailed(response: Response): Promise<PostingGetDetailed> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = PostingGetDetailed.fromJS(resultData200);
            return result200;
            });
        } else if (status === 404) {
            return response.text().then((_responseText) => {
            let result404: any = null;
            let resultData404 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result404 = ProblemDetails.fromJS(resultData404);
            return throwException("Not Found", status, _responseText, _headers, result404);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<PostingGetDetailed>(null as any);
    }

    /**
     * @param count (optional) 
     * @param startId (optional) 
     * @return Success
     */
    postingAll(count: number | undefined, startId: number | undefined): Promise<PostingGetGeneral[]> {
        let url_ = this.baseUrl + "/api/Posting?";
        if (count === null)
            throw new Error("The parameter 'count' cannot be null.");
        else if (count !== undefined)
            url_ += "Count=" + encodeURIComponent("" + count) + "&";
        if (startId === null)
            throw new Error("The parameter 'startId' cannot be null.");
        else if (startId !== undefined)
            url_ += "StartId=" + encodeURIComponent("" + startId) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processPostingAll(_response);
        });
    }

    protected processPostingAll(response: Response): Promise<PostingGetGeneral[]> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(PostingGetGeneral.fromJS(item));
            }
            else {
                result200 = <any>null;
            }
            return result200;
            });
        } else if (status === 400) {
            return response.text().then((_responseText) => {
            let result400: any = null;
            let resultData400 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result400 = ProblemDetails.fromJS(resultData400);
            return throwException("Bad Request", status, _responseText, _headers, result400);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<PostingGetGeneral[]>(null as any);
    }

    /**
     * @param body (optional) 
     * @return Created
     */
    postingPOST(body: PostingCreate | undefined): Promise<PostingGetDetailed> {
        let url_ = this.baseUrl + "/api/Posting";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(body);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processPostingPOST(_response);
        });
    }

    protected processPostingPOST(response: Response): Promise<PostingGetDetailed> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 201) {
            return response.text().then((_responseText) => {
            let result201: any = null;
            let resultData201 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result201 = PostingGetDetailed.fromJS(resultData201);
            return result201;
            });
        } else if (status === 400) {
            return response.text().then((_responseText) => {
            let result400: any = null;
            let resultData400 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result400 = ProblemDetails.fromJS(resultData400);
            return throwException("Bad Request", status, _responseText, _headers, result400);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<PostingGetDetailed>(null as any);
    }
}

/** 1 = Offer 2 = Request 4 = Permanent 5 = Sell 6 = Buy 8 = Temporary 9 = Rent 10 = Lease 13 = SaleOrRent 14 = BuyOrLease 15 = All */
export enum BargainKinds {
    Offer = 1,
    Request = 2,
    Permanent = 4,
    Sell = 5,
    Buy = 6,
    Temporary = 8,
    Rent = 9,
    Lease = 10,
    SaleOrRent = 13,
    BuyOrLease = 14,
    All = 15,
}

export class INavbarAction implements IINavbarAction {
    readonly DisplayName?: string | undefined;
    readonly Path?: string | undefined;

    constructor(data?: IINavbarAction) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            (<any>this).DisplayName = _data["DisplayName"];
            (<any>this).Path = _data["Path"];
        }
    }

    static fromJS(data: any): INavbarAction {
        data = typeof data === 'object' ? data : {};
        let result = new INavbarAction();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["DisplayName"] = this.DisplayName;
        data["Path"] = this.Path;
        return data;
    }
}

export interface IINavbarAction {
    DisplayName?: string | undefined;
    Path?: string | undefined;
}

export class LocationPostingDetails implements ILocationPostingDetails {
    Country!: string;
    City?: string | undefined;
    Address?: string | undefined;
    Latitude?: number | undefined;
    Longitude?: number | undefined;

    constructor(data?: ILocationPostingDetails) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.Country = _data["Country"];
            this.City = _data["City"];
            this.Address = _data["Address"];
            this.Latitude = _data["Latitude"];
            this.Longitude = _data["Longitude"];
        }
    }

    static fromJS(data: any): LocationPostingDetails {
        data = typeof data === 'object' ? data : {};
        let result = new LocationPostingDetails();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["Country"] = this.Country;
        data["City"] = this.City;
        data["Address"] = this.Address;
        data["Latitude"] = this.Latitude;
        data["Longitude"] = this.Longitude;
        return data;
    }
}

export interface ILocationPostingDetails {
    Country: string;
    City?: string | undefined;
    Address?: string | undefined;
    Latitude?: number | undefined;
    Longitude?: number | undefined;
}

export class PostingAuthorGet implements IPostingAuthorGet {
    Id!: number;
    Name!: string;
    Email!: string;

    constructor(data?: IPostingAuthorGet) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.Id = _data["Id"];
            this.Name = _data["Name"];
            this.Email = _data["Email"];
        }
    }

    static fromJS(data: any): PostingAuthorGet {
        data = typeof data === 'object' ? data : {};
        let result = new PostingAuthorGet();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["Id"] = this.Id;
        data["Name"] = this.Name;
        data["Email"] = this.Email;
        return data;
    }
}

export interface IPostingAuthorGet {
    Id: number;
    Name: string;
    Email: string;
}

export class PostingCreate implements IPostingCreate {
    Title!: string;
    Description!: string;
    ThumbnailUrl!: string;
    Details!: PostingDetails;

    constructor(data?: IPostingCreate) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
        if (!data) {
            this.Details = new PostingDetails();
        }
    }

    init(_data?: any) {
        if (_data) {
            this.Title = _data["Title"];
            this.Description = _data["Description"];
            this.ThumbnailUrl = _data["ThumbnailUrl"];
            this.Details = _data["Details"] ? PostingDetails.fromJS(_data["Details"]) : new PostingDetails();
        }
    }

    static fromJS(data: any): PostingCreate {
        data = typeof data === 'object' ? data : {};
        let result = new PostingCreate();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["Title"] = this.Title;
        data["Description"] = this.Description;
        data["ThumbnailUrl"] = this.ThumbnailUrl;
        data["Details"] = this.Details ? this.Details.toJSON() : <any>undefined;
        return data;
    }
}

export interface IPostingCreate {
    Title: string;
    Description: string;
    ThumbnailUrl: string;
    Details: PostingDetails;
}

export class PostingDetails implements IPostingDetails {
    Pricing?: PricingPostingDetails;
    Location?: LocationPostingDetails;
    Kind?: PostingKind;
    RealEstate?: RealEstatePostingDetails;
    Vehicle?: VehiclePostingDetails;

    constructor(data?: IPostingDetails) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.Pricing = _data["Pricing"] ? PricingPostingDetails.fromJS(_data["Pricing"]) : <any>undefined;
            this.Location = _data["Location"] ? LocationPostingDetails.fromJS(_data["Location"]) : <any>undefined;
            this.Kind = _data["Kind"];
            this.RealEstate = _data["RealEstate"] ? RealEstatePostingDetails.fromJS(_data["RealEstate"]) : <any>undefined;
            this.Vehicle = _data["Vehicle"] ? VehiclePostingDetails.fromJS(_data["Vehicle"]) : <any>undefined;
        }
    }

    static fromJS(data: any): PostingDetails {
        data = typeof data === 'object' ? data : {};
        let result = new PostingDetails();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["Pricing"] = this.Pricing ? this.Pricing.toJSON() : <any>undefined;
        data["Location"] = this.Location ? this.Location.toJSON() : <any>undefined;
        data["Kind"] = this.Kind;
        data["RealEstate"] = this.RealEstate ? this.RealEstate.toJSON() : <any>undefined;
        data["Vehicle"] = this.Vehicle ? this.Vehicle.toJSON() : <any>undefined;
        return data;
    }
}

export interface IPostingDetails {
    Pricing?: PricingPostingDetails;
    Location?: LocationPostingDetails;
    Kind?: PostingKind;
    RealEstate?: RealEstatePostingDetails;
    Vehicle?: VehiclePostingDetails;
}

export class PostingGetDetailed implements IPostingGetDetailed {
    General!: PostingGetGeneral;
    PictureUrls!: string[];
    Author!: PostingAuthorGet;
    Details!: PostingDetails;

    constructor(data?: IPostingGetDetailed) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
        if (!data) {
            this.General = new PostingGetGeneral();
            this.PictureUrls = [];
            this.Author = new PostingAuthorGet();
            this.Details = new PostingDetails();
        }
    }

    init(_data?: any) {
        if (_data) {
            this.General = _data["General"] ? PostingGetGeneral.fromJS(_data["General"]) : new PostingGetGeneral();
            if (Array.isArray(_data["PictureUrls"])) {
                this.PictureUrls = [] as any;
                for (let item of _data["PictureUrls"])
                    this.PictureUrls!.push(item);
            }
            this.Author = _data["Author"] ? PostingAuthorGet.fromJS(_data["Author"]) : new PostingAuthorGet();
            this.Details = _data["Details"] ? PostingDetails.fromJS(_data["Details"]) : new PostingDetails();
        }
    }

    static fromJS(data: any): PostingGetDetailed {
        data = typeof data === 'object' ? data : {};
        let result = new PostingGetDetailed();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["General"] = this.General ? this.General.toJSON() : <any>undefined;
        if (Array.isArray(this.PictureUrls)) {
            data["PictureUrls"] = [];
            for (let item of this.PictureUrls)
                data["PictureUrls"].push(item);
        }
        data["Author"] = this.Author ? this.Author.toJSON() : <any>undefined;
        data["Details"] = this.Details ? this.Details.toJSON() : <any>undefined;
        return data;
    }
}

export interface IPostingGetDetailed {
    General: PostingGetGeneral;
    PictureUrls: string[];
    Author: PostingAuthorGet;
    Details: PostingDetails;
}

export class PostingGetGeneral implements IPostingGetGeneral {
    Id!: number;
    Title!: string;
    Description!: string;
    ThumbnailUrl!: string;
    DatePosted!: Date;
    Slug!: string;

    constructor(data?: IPostingGetGeneral) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.Id = _data["Id"];
            this.Title = _data["Title"];
            this.Description = _data["Description"];
            this.ThumbnailUrl = _data["ThumbnailUrl"];
            this.DatePosted = _data["DatePosted"] ? new Date(_data["DatePosted"].toString()) : <any>undefined;
            this.Slug = _data["Slug"];
        }
    }

    static fromJS(data: any): PostingGetGeneral {
        data = typeof data === 'object' ? data : {};
        let result = new PostingGetGeneral();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["Id"] = this.Id;
        data["Title"] = this.Title;
        data["Description"] = this.Description;
        data["ThumbnailUrl"] = this.ThumbnailUrl;
        data["DatePosted"] = this.DatePosted ? this.DatePosted.toISOString() : <any>undefined;
        data["Slug"] = this.Slug;
        return data;
    }
}

export interface IPostingGetGeneral {
    Id: number;
    Title: string;
    Description: string;
    ThumbnailUrl: string;
    DatePosted: Date;
    Slug: string;
}

/** 0 = RealEstate 1 = Vehicle */
export enum PostingKind {
    RealEstate = 0,
    Vehicle = 1,
}

export class PricingPostingDetails implements IPricingPostingDetails {
    BargainKinds!: BargainKinds;
    Price?: number | undefined;
    PriceMax?: number | undefined;

    constructor(data?: IPricingPostingDetails) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.BargainKinds = _data["BargainKinds"];
            this.Price = _data["Price"];
            this.PriceMax = _data["PriceMax"];
        }
    }

    static fromJS(data: any): PricingPostingDetails {
        data = typeof data === 'object' ? data : {};
        let result = new PricingPostingDetails();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["BargainKinds"] = this.BargainKinds;
        data["Price"] = this.Price;
        data["PriceMax"] = this.PriceMax;
        return data;
    }
}

export interface IPricingPostingDetails {
    BargainKinds: BargainKinds;
    Price?: number | undefined;
    PriceMax?: number | undefined;
}

export class ProblemDetails implements IProblemDetails {
    type?: string | undefined;
    title?: string | undefined;
    status?: number | undefined;
    detail?: string | undefined;
    instance?: string | undefined;

    [key: string]: any;

    constructor(data?: IProblemDetails) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.type = _data["type"];
            this.title = _data["title"];
            this.status = _data["status"];
            this.detail = _data["detail"];
            this.instance = _data["instance"];
        }
    }

    static fromJS(data: any): ProblemDetails {
        data = typeof data === 'object' ? data : {};
        let result = new ProblemDetails();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["type"] = this.type;
        data["title"] = this.title;
        data["status"] = this.status;
        data["detail"] = this.detail;
        data["instance"] = this.instance;
        return data;
    }
}

export interface IProblemDetails {
    type?: string | undefined;
    title?: string | undefined;
    status?: number | undefined;
    detail?: string | undefined;
    instance?: string | undefined;

    [key: string]: any;
}

/** 0 = House 1 = Apartment 2 = Condo 3 = Townhouse 4 = Land 5 = Other */
export enum RealEstateKind {
    House = 0,
    Apartment = 1,
    Condo = 2,
    Townhouse = 3,
    Land = 4,
    Other = 5,
}

export class RealEstatePostingDetails implements IRealEstatePostingDetails {
    Kind!: RealEstateKind;
    SpacePurpose!: RealEstateSpacePurpose;
    Area!: number;
    Rooms!: number;

    constructor(data?: IRealEstatePostingDetails) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.Kind = _data["Kind"];
            this.SpacePurpose = _data["SpacePurpose"];
            this.Area = _data["Area"];
            this.Rooms = _data["Rooms"];
        }
    }

    static fromJS(data: any): RealEstatePostingDetails {
        data = typeof data === 'object' ? data : {};
        let result = new RealEstatePostingDetails();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["Kind"] = this.Kind;
        data["SpacePurpose"] = this.SpacePurpose;
        data["Area"] = this.Area;
        data["Rooms"] = this.Rooms;
        return data;
    }
}

export interface IRealEstatePostingDetails {
    Kind: RealEstateKind;
    SpacePurpose: RealEstateSpacePurpose;
    Area: number;
    Rooms: number;
}

/** 0 = Residential 1 = Commercial 2 = Industrial 3 = Any 4 = Other */
export enum RealEstateSpacePurpose {
    Residential = 0,
    Commercial = 1,
    Industrial = 2,
    Any = 3,
    Other = 4,
}

export class VehiclePostingDetails implements IVehiclePostingDetails {
    Year!: number;
    Manufacturer!: string;
    Model!: string;

    constructor(data?: IVehiclePostingDetails) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.Year = _data["Year"];
            this.Manufacturer = _data["Manufacturer"];
            this.Model = _data["Model"];
        }
    }

    static fromJS(data: any): VehiclePostingDetails {
        data = typeof data === 'object' ? data : {};
        let result = new VehiclePostingDetails();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["Year"] = this.Year;
        data["Manufacturer"] = this.Manufacturer;
        data["Model"] = this.Model;
        return data;
    }
}

export interface IVehiclePostingDetails {
    Year: number;
    Manufacturer: string;
    Model: string;
}

export class ApiException extends Error {
    message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isApiException = true;

    static isApiException(obj: any): obj is ApiException {
        return obj.isApiException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): any {
    if (result !== null && result !== undefined)
        throw result;
    else
        throw new ApiException(message, status, response, headers, null);
}
export const ApiPropertyTable = {
    BargainKinds: [
    ],
    INavbarAction: [
        { name: "DisplayName", schemaTypeName: null },
        { name: "Path", schemaTypeName: null },
    ],
    LocationPostingDetails: [
        { name: "Country", schemaTypeName: null },
        { name: "City", schemaTypeName: null },
        { name: "Address", schemaTypeName: null },
        { name: "Latitude", schemaTypeName: "number" },
        { name: "Longitude", schemaTypeName: "number" },
    ],
    PostingAuthorGet: [
        { name: "Id", schemaTypeName: null },
        { name: "Name", schemaTypeName: null },
        { name: "Email", schemaTypeName: null },
    ],
    PostingCreate: [
        { name: "Title", schemaTypeName: null },
        { name: "Description", schemaTypeName: null },
        { name: "ThumbnailUrl", schemaTypeName: null },
        { name: "Details", schemaTypeName: "PostingDetails" },
    ],
    PostingDetails: [
        { name: "Pricing", schemaTypeName: "PricingPostingDetails" },
        { name: "Location", schemaTypeName: "LocationPostingDetails" },
        { name: "Kind", schemaTypeName: null },
        { name: "RealEstate", schemaTypeName: "RealEstatePostingDetails" },
        { name: "Vehicle", schemaTypeName: "VehiclePostingDetails" },
    ],
    PostingGetDetailed: [
        { name: "General", schemaTypeName: "PostingGetGeneral" },
        { name: "PictureUrls", schemaTypeName: null },
        { name: "Author", schemaTypeName: "PostingAuthorGet" },
        { name: "Details", schemaTypeName: "PostingDetails" },
    ],
    PostingGetGeneral: [
        { name: "Id", schemaTypeName: null },
        { name: "Title", schemaTypeName: null },
        { name: "Description", schemaTypeName: null },
        { name: "ThumbnailUrl", schemaTypeName: null },
        { name: "DatePosted", schemaTypeName: null },
        { name: "Slug", schemaTypeName: null },
    ],
    PostingKind: [
    ],
    PricingPostingDetails: [
        { name: "BargainKinds", schemaTypeName: null },
        { name: "Price", schemaTypeName: "number" },
        { name: "PriceMax", schemaTypeName: "number" },
    ],
    ProblemDetails: [
        { name: "type", schemaTypeName: null },
        { name: "title", schemaTypeName: null },
        { name: "status", schemaTypeName: null },
        { name: "detail", schemaTypeName: null },
        { name: "instance", schemaTypeName: null },
    ],
    RealEstateKind: [
    ],
    RealEstatePostingDetails: [
        { name: "Kind", schemaTypeName: null },
        { name: "SpacePurpose", schemaTypeName: null },
        { name: "Area", schemaTypeName: "number" },
        { name: "Rooms", schemaTypeName: null },
    ],
    RealEstateSpacePurpose: [
    ],
    VehiclePostingDetails: [
        { name: "Year", schemaTypeName: null },
        { name: "Manufacturer", schemaTypeName: null },
        { name: "Model", schemaTypeName: null },
    ],
};
