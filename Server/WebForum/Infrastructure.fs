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

type CompositionRoot
   (users : Users.IUsers, notifications : Notifications.INotifications) =

      interface IHttpControllerActivator with
         member this.Create(request, controllerDescriptor, controllerType) =
            if controllerType = typeof<HomeController> then
               new HomeController() :> IHttpController
            elif controllerType = typeof<AuthController> then
               new AuthController() :> IHttpController
            else
               raise <| ArgumentException("Unknown controller type")

type HttpRouteDefaults = { Controller : string; Id : obj }
type AuthDefaults = { Controller : string; User : obj }

let ConfigureServices users notifications (config : HttpConfiguration) =
   config.Services.Replace(typeof<IHttpControllerActivator>, CompositionRoot(users, notifications))

let ConfigureRoutes (config : HttpConfiguration) =
   config.Routes.MapHttpRoute(
         "Default",
         "{controller}/{id}",
         { Controller = "Home"; Id = RouteParameter.Optional }) |> ignore

   config.Routes.MapHttpRoute(
         "Auth",
         "{controller}/{user}",
         { Controller = "Auth"; User = RouteParameter.Optional }) |> ignore

let ConfigureFormatting (config : HttpConfiguration) =
   config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <-
      Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()

let Configure users notifications config = 
   ConfigureRoutes config
   ConfigureServices users notifications config
   ConfigureFormatting config