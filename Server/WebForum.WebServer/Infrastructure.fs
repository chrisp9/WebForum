namespace WebForum.WebServer.HttpApi.HttpHost

open System
open System.Web.Http
open System.Web
open WebForum.WebServer.HttpApi.Infrastructure
open WebForum.WebServer.HttpApi
open System.Collections.Concurrent
open FSharp.Reactive
open System.Reactive
open WebForum.WebServer.HttpApi.Envelope

type Agent<'T> = Microsoft.FSharp.Control.MailboxProcessor<'T>

type HttpRouteDefaults = { Controller : string; Id : obj }

type Global() =
   inherit System.Web.HttpApplication()
   member this.Application_Start (sender : obj) (e : EventArgs) =
      let users = ConcurrentBag<Envelope<User>>()
      let notifications = ConcurrentBag<Envelope<Notification>>()


      let usersSubject = new Subjects.Subject<Envelope<User>>()
      usersSubject.Subscribe users.Add |> ignore

      let notificationSubject = new Subjects.Subject<Notification>()
      notificationSubject
      |> Observable.map EnvelopWithDefaults
      |> Observable.subscribe notifications.Add ignore ignore
      |> ignore


      let agent = new Agent<Envelope<AddUserMessage>>(fun inbox ->
         let rec loop () =
            async {
               let! cmd = inbox.Receive()
               let rs = users |> Users.ToUsers
               let handle = Users.Handle rs
               let newUsers = handle cmd
               match newUsers with
               | Some r ->
                  usersSubject.OnNext r
                  notificationSubject.OnNext
                     {
                        Notification.About = cmd.Id
                        Notification.Type = "Success"
                        Notification.Message =
                           sprintf "A new user with username %s has been created" (cmd.Item.Username)
                     }
               | _ ->
                  notificationSubject.OnNext
                     {
                        Notification.About = cmd.Id
                        Notification.Type = "Failure"
                        Notification.Message =
                           sprintf "The user with username %s already exists. Please choose a different username" (cmd.Item.Username)
                     }
               return! loop()}
         loop())
      do agent.Start()

      Infrastructure.Configure 
         (users |> Users.ToUsers)
         (notifications |> Notifications.ToNotifications)
         (Observer.Create agent.Post)
         (GlobalConfiguration.Configuration)