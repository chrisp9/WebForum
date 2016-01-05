namespace WebForum.WebServer.HttpApi.HttpHost

open System
open System.Web.Http
open System.Web
open WebForum.WebServer.HttpApi.Infrastructure
open WebForum.WebServer.HttpApi
open System.Collections.Concurrent
open FSharp.Reactive
open System.Reactive

type Agent<'T> = Microsoft.FSharp.Control.MailboxProcessor<'T>

type HttpRouteDefaults = { Controller : string; Id : obj }

type Global() =
   inherit System.Web.HttpApplication()
   member this.Application_Start (sender : obj) (e : EventArgs) =
      let users = ConcurrentBag<Envelope<User>>()

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

      let x = FSharp.Control.Observable.subscribe agent.Post
      let g = Observer.Create agent.Post
      let sub = new Subjects.Subject<Envelope<AddUserMessage>>()

      Infrastructure.Configure 
         (users)
         (g)
         (GlobalConfiguration.Configuration)