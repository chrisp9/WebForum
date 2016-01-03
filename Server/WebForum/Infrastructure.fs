module WebForum.WebServer.HttpApi.Infrastructure

open System.Web.Http

type HttpRouteDefaults = { Controller : string; Id : obj }

let ConfigureRoutes (config : HttpConfiguration) =
   config.Routes.MapHttpRoute(
         "Default",
         "{controller}/{id}",
         { Controller = "Home"; Id = RouteParameter.Optional }) |> ignore

let Configure = ConfigureRoutes