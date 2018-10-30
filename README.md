# Hallo
[![Build status](https://ci.appveyor.com/api/projects/status/h1a8hd8i9aj6upwu/branch/master?svg=true)](https://ci.appveyor.com/project/jasonmitchell/hallo/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Hallo.svg?style=flat)](https://www.nuget.org/packages/Hallo/)

Hallo is an implementation of the [Hypertext Application Language (HAL)](http://stateless.co/hal_specification.html)
format for ASP.NET Core.

## Why Hallo?
The primary design goal of Hallo is to enable generation of HAL documents through content negotiation
without requiring HAL-specific code in models or controllers.


## Getting started with Hallo

## Installing Hallo
Hallo is available on [Nuget](https://www.nuget.org/packages/Hallo/): `dotnet add package Hallo`


## Using Hallo
Hallo does not require any changes to existing models or controllers so it can easily be added to an 
existing project.  

To get started using Hallo you need to first register the output formatter in ASP.NET Core MVC to enable
content negotiation for HAL responses:

```csharp
services.AddMvc(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.OutputFormatters.Add(new JsonHalOutputFormatter());
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
        yield return new Link("contacts", $"/people{resource.Id}/contacts");
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
      "href": "/people1/contacts"
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
        yield return new Link("contacts", $"/people{resource.Id}/contacts");
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
        yield return new Link("contacts", $"/people{resource.Id}/contacts");
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
          "href": "/people321/contacts"
        }
      }
    }
  },
  "_links": {
    "self": {
      "href": "/people/1"
    },
    "contacts": {
      "href": "/people1/contacts"
    }
  }
}
```
