module WebForum.WebServer.HttpApi.Infrastructure

open System
open System.Reactive
open FSharp.Reactive
open System.Net.Http
open System.Web.Http
open System.Web.Http.Dispatcher
open System.Web.Http.Controllers
open FSharp.Control
open WebForum.WebServer.HttpApi

type CompositionRoot(users : System.Collections.Concurrent.ConcurrentBag<Envelope<User>>, newUserRequestObserver : IObserver<Envelope<AddUserMessage>>) =


   interface IHttpControllerActivator with
      member this.Create(request, controllerDescriptor, controllerType) =
         if controllerType = typeof<HomeController> then
            new HomeController() :> IHttpController
         elif controllerType = typeof<AddUserController> then
            let addUserController = new AddUserController()
            
            (addUserController :> IObservable<Envelope<AddUserMessage>>)
              .Subscribe newUserRequestObserver 
              |> request.RegisterForDispose
            addUserController :> IHttpController
         else
            raise <| ArgumentException("Unknown controller type")

type HttpRouteDefaults = { Controller : string; Id : obj }

let ConfigureServices users newUserRequestObserver (config : HttpConfiguration) =
   config.Services.Replace(typeof<IHttpControllerActivator>, CompositionRoot(users, newUserRequestObserver))

let ConfigureRoutes (config : HttpConfiguration) =
   config.Routes.MapHttpRoute(
         "Default",
         "{controller}/{id}",
         { Controller = "Home"; Id = RouteParameter.Optional }) |> ignore

let ConfigureFormatting (config : HttpConfiguration) =
   config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <-
      Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()

let Configure users (newUserRequestObserver : IObserver<Envelope<AddUserMessage>>) config = 
   ConfigureRoutes config
   ConfigureServices users newUserRequestObserver config
   ConfigureFormatting config