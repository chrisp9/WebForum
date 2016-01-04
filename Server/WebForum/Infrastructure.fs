module WebForum.WebServer.HttpApi.Infrastructure

open System
open System.Web.Http
open System.Web.Http.Dispatcher
open System.Web.Http.Controllers
open WebForum.WebServer.HttpApi

type Agent<'T> = Microsoft.FSharp.Control.MailboxProcessor<'T>

type CompositionRoot() =
   let users = System.Collections.Concurrent.ConcurrentBag<Envelope<User>>()
   let agent = new Agent<Envelope<AddUserMessage>>(fun inbox ->
      let rec loop () =
         async {
            let! cmd = inbox.Receive()
            let rs = users |> Users.ToUsers
            let handle = Users.Handle rs
            let newUsers = handle cmd
            match newUsers with
            | Some r -> users.Add(r)
            | _ -> ()
            return! loop()}
      loop())
   do agent.Start()

   interface IHttpControllerActivator with
      member this.Create(request, controllerDescriptor, controllerType) =
         if controllerType = typeof<HomeController> then
            new HomeController() :> IHttpController
         elif controllerType = typeof<AddUserController> then
            let c = new AddUserController()
            c :> IHttpController
         else
            raise <| ArgumentException("Unknown controller type")

type HttpRouteDefaults = { Controller : string; Id : obj }

let ConfigureServices (config : HttpConfiguration) =
   config.Services.Replace(typeof<IHttpControllerActivator>, CompositionRoot())

let ConfigureRoutes (config : HttpConfiguration) =
   config.Routes.MapHttpRoute(
         "Default",
         "{controller}/{id}",
         { Controller = "Home"; Id = RouteParameter.Optional }) |> ignore

let ConfigureFormatting (config : HttpConfiguration) =
   config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <-
      Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()

let Configure config = 
   ConfigureRoutes config
   ConfigureServices config
   ConfigureFormatting config