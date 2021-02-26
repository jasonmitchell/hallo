namespace Hallo.Giraffe.Sample

open System.Collections.Generic
open System.Text.Encodings.Web
open System.Text.Json
open System.Text.Json.Serialization
open Hallo
open Hallo.AspNetCore
open Hallo.AspNetCore.Sample
open Hallo.AspNetCore.Sample.Data
open Hallo.AspNetCore.Sample.Models
open Hallo.AspNetCore.Sample.Models.Hypermedia
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Giraffe
open FSharp.Control.Tasks

type CustomNegotiationConfig (baseConfig : INegotiationConfig) =
    
    let hal (model : obj) : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task{
                do! HalJsonGenerator.HalHandler(ctx, model)
                return Some ctx
            }

    interface INegotiationConfig with

        member __.UnacceptableHandler =
            baseConfig.UnacceptableHandler

        member __.Rules =
                let rules = Dictionary<string,obj->HttpHandler>()
                baseConfig.Rules |> Seq.iter(fun x -> rules.Add(x.Key,x.Value))
                rules.Add("application/hal+json", hal)
                rules :> IDictionary<string,obj->HttpHandler>

type Startup() =

    let getPeopleHandler: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let peopleRepo =
                    ctx.RequestServices.GetRequiredService<PeopleRepository>()

                let model = peopleRepo.List(Paging())
                return! negotiate model next ctx
            }

    let getPeopleByIdHandler (id: int): HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let peopleRepo =
                    ctx.RequestServices.GetRequiredService<PeopleRepository>()

                let model = peopleRepo.Get(id)
                return! negotiate model next ctx
            }


    let webApp =
        choose [ route "/"
                 >=> GET
                 >=> text "There's no place like 127.0.0.1" //TODO Home Document application/json-home?

                 route "/people" >=> GET >=> getPeopleHandler
                 GET >=> routef "/people/%i" getPeopleByIdHandler

                  ]

    member _.ConfigureServices(services: IServiceCollection) =
        services.AddSingleton<PeopleRepository>()
        |> ignore

        services.AddTransient<PersonRepresentation>()
        |> ignore

        services.AddTransient<Hal<Person>, PersonRepresentation>()
        |> ignore

        services.AddTransient<Hal<PagedList<Person>>, PersonListRepresentation>()
        |> ignore

        let serializerOptions = JsonSerializerOptions()
        serializerOptions.Converters.Add(Hallo.Serialization.HalRepresentationConverter())
        serializerOptions.Converters.Add(Hallo.Serialization.LinksConverter())
        serializerOptions.Converters.Add(JsonFSharpConverter(JsonUnionEncoding.FSharpLuLike))
        serializerOptions.PropertyNameCaseInsensitive <- true
        serializerOptions.Encoder <- JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        services.AddSingleton(serializerOptions) |> ignore

        services.AddGiraffe() |> ignore
        
        services.AddSingleton<INegotiationConfig>(
            CustomNegotiationConfig(
                DefaultNegotiationConfig())
        ) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) = app.UseGiraffe webApp
