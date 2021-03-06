# Hallo
![](https://github.com/jasonmitchell/hallo/workflows/Build/badge.svg?branch=master)
[![NuGet](https://img.shields.io/nuget/v/Hallo.svg?style=flat)](https://www.nuget.org/packages/Hallo/)
[![NuGet](https://img.shields.io/nuget/v/Hallo.AspNetCore.svg?style=flat)](https://www.nuget.org/packages/Hallo.AspNetCore/)
[![NuGet](https://img.shields.io/nuget/v/Hallo.AspNetCore.Mvc.svg?style=flat)](https://www.nuget.org/packages/Hallo.AspNetCore.Mvc/)

Hallo is an implementation of the [Hypertext Application Language (HAL)](http://stateless.co/hal_specification.html)
format for ASP.NET Core.

## Why Hallo?
The primary design goal of Hallo is to enable generation of HAL documents through content negotiation
without requiring HAL-specific code in models or controllers.


## Getting started with Hallo

## Installing Hallo
Hallo is available on Nuget as three packages:
- [Hallo](https://www.nuget.org/packages/Hallo/)
- [Hallo.AspNetCore](https://www.nuget.org/packages/Hallo.AspNetCore/)
- [Hallo.AspNetCore.Mvc](https://www.nuget.org/packages/Hallo.AspNetCore.Mvc)

```
dotnet add package Hallo
dotnet add package Hallo.AspNetCore
dotnet add package Hallo.AspNetCore.Mvc
```

The Hallo package is the core library which provides types for writing HAL representation generators
and serializing objects to HAL+JSON strings. The Hallo.AspNetCore provides basic support for serializing 
HAL representations to the `HttpResponse` body stream and the Hallo.AspNetCore.Mvc package provides an
output formatter to leverage ASP.NET MVC content negotiation functionality.

The rest of this readme will assume you are using the Hallo.AspNetCore.MVC package.


## Using Hallo
Hallo does not require any changes to existing models or controllers so it can easily be added to an 
existing project.  

To get started using Hallo you need to first register the output formatter in ASP.NET Core to enable
content negotiation for HAL responses:

```csharp
services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.OutputFormatters.Add(new HalJsonOutputFormatter());
})
```

For every resource you want to generate a HAL document for you need to derive a new class from `Hal<T>` 
and implement one or more of `IHalState<T>`, `IHalEmbedded<T>` or `IHalLinks<T>`:

```csharp
public class PersonRepresentation : Hal<Person>, 
                                    IHalState<Person>,
                                    IHalLinks<Person>, 
                                    IHalEmbedded<Person>
{
    public object StateFor(Person resource)
    {
        return new
        {
            resource.FirstName,
            resource.LastName
        };
    }
    
    public IEnumerable<Link> LinksFor(Person resource)
    {
        yield return new Link(Link.Self, $"/people/{resource.Id}");
        yield return new Link("contacts", $"/people/{resource.Id}/contacts");
    }

    public object EmbeddedFor(Person resource)
    {
        return new
        {
            Contacts = new List<Person>()
        };
    }
}
```

Each resource representation needs to be registered in the ASP.NET Core `IServiceCollection`:

```csharp
services.AddTransient<Hal<Person>, PersonRepresentation>();
```

Given the example above, a HTTP request such as:
```http
GET http://localhost:5000/people/1
Accept: application/hal+json
```

will produce the result:
```json
{
  "firstName": "Geoffrey",
  "lastName": "Merrill",
  "_embedded": {
    "contacts": []
  },
  "_links": {
    "self": {
      "href": "/people/1"
    },
    "contacts": {
      "href": "/people/1/contacts"
    }
  }
}
```


### Dependency Injection
As resource representations are registered with and resolved through ASP.NET Core services, the standard
approach to injecting dependencies can be applied.

#### Example
```csharp
public class PersonRepresentation : Hal<Person>, 
                                    IHalEmbeddedAsync<Person>
{
    private readonly ContactsLookup _contacts;

    public PersonRepresentation(ContactsLookup contacts)
    {
        _contacts = contacts;
    }

    public async Task<object> EmbeddedForAsync(Person resource)
    {
        var contacts = await _contacts.GetFor(resource.Id);
        
        return new
        {
            Contacts = contacts
        };
    }
}
```


### Async Support
Hallo provides the interfaces `IHalStateAsync<T>`, `IHalEmbeddedAsync<T>` and `IHalLinksAsync<T>`.
These interfaces define asynchronous version of the `StateFor`, `EmbeddedFor` and `LinksFor` methods
to enable the execution of asynchronous code as part of the HAL document generation process.

#### Example
```csharp
public class PersonRepresentation : Hal<Person>, 
                                    IHalLinks<Person>, 
                                    IHalEmbeddedAsync<Person>
{
    public IEnumerable<Link> LinksFor(Person resource)
    {
        yield return new Link(Link.Self, $"/people/{resource.Id}");
        yield return new Link("contacts", $"/people/{resource.Id}/contacts");
    }

    public async Task<object> EmbeddedForAsync(Person resource)
    {
        var contacts = await FetchContactsAsync(resource.Id);
        
        return new
        {
            Contacts = contacts
        };
    }
}
```

### Nested Representations
Sometimes it is necessary to produce "nested" HAL documents.  For example it is common to generate `_links`
for resources under the `_embedded` property in the root HAL document.

Hallo supports recursive generation of HAL documents by wrapping embedded resources in a `HalRepresentation`.

#### Example
```csharp
public class PersonRepresentation : Hal<Person>, 
                                    IHalLinks<Person>, 
                                    IHalEmbedded<Person>
{
    public IEnumerable<Link> LinksFor(Person resource)
    {
        yield return new Link(Link.Self, $"/people/{resource.Id}");
        yield return new Link("contacts", $"/people/{resource.Id}/contacts");
    }

    public object EmbeddedFor(Person resource)
    {
        var spouse = new Person
        {
            Id = 321,
            FirstName = "A",
            LastName = "Spouse"
        };

        var links = LinksFor(spouse);
        
        return new
        {
            Spouse = new HalRepresentation(spouse, links)
        };
    }
}
```

The above example will produce a response of:

```json
{
  "id": 1,
  "firstName": "Geoffrey",
  "lastName": "Merrill",
  "_embedded": {
    "spouse": {
      "id": 321,
      "firstName": "A",
      "lastName": "Spouse",
      "_links": {
        "self": {
          "href": "/people/321"
        },
        "contacts": {
          "href": "/people/321/contacts"
        }
      }
    }
  },
  "_links": {
    "self": {
      "href": "/people/1"
    },
    "contacts": {
      "href": "/people/1/contacts"
    }
  }
}
```

### Prefixing Links With a Virtual Path
If a deployed API is available via a virtual path such as an IIS sub-application/virtual directory, API 
gateway or reverse proxy it may be necessary to prefix links with the virtual path.  For example, an API 
may be developed locally with the URL `http://localhost:5000/people/{id}` however the API is deployed 
to production behind an API gateway with the URL `http://my-app/address-book/people/{id}`.
In this scenario it may be preferable to generate links prefixed with `/address-book`.

This can be easily achieved by ensuring the 
[PathBase](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httprequest.pathbase?view=aspnetcore-3.1)
property for the request is set and using the ASP.NET Core 
[IUrlHelper](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iurlhelper?view=aspnetcore-3.1)
to create links rather than the string building approach used in this README.

#### Example
The following ASP.NET Core services need to be registered on startup:
```csharp
services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
services.AddScoped(x => {
    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
    var factory = x.GetRequiredService<IUrlHelperFactory>();
    return factory.GetUrlHelper(actionContext);
});
```

The `IUrlHelper` can then be injected into representations and used to create links:
```csharp
public class PersonRepresentation : Hal<Person>, 
                                    IHalLinks<Person>
{
    private readonly IUrlHelper _urlHelper;

    public PersonRepresentation(IUrlHelper urlHelper)
    {
        _urlHelper = urlHelper;
    }

    public IEnumerable<Link> LinksFor(Person resource)
    {
        var self = _urlHelper.Action("Get", "People", new {id = resource.Id});
        var contacts = _urlHelper.Action("List", "Contacts", new {personId = resource.Id});

        yield return new Link(Link.Self, self);
        yield return new Link("contacts", contacts);
    }
}
```

Assuming a `PathBase` value of `/address-book`, the above example will produce a response of:
```json
{
  "id": 1,
  "firstName": "Geoffrey",
  "lastName": "Merrill",
  "_links": {
    "self": {
      "href": "/address-book/people/1"
    },
    "contacts": {
      "href": "/address-book/people/1/contacts"
    }
  }
}
```