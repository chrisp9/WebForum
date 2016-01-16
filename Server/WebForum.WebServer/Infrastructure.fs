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
      let sessions = ConcurrentBag<Envelope<SessionMessage>>()
      let notifications = ConcurrentBag<Envelope<Notification>>()

      let usersSubject = new Subjects.Subject<Envelope<User>>()
      usersSubject.Subscribe users.Add |> ignore

      let authSubject = new Subjects.Subject<Envelope<SessionMessage>>()
      authSubject.Subscribe sessions.Add |> ignore

      let notificationSubject = new Subjects.Subject<Notification>()
      notificationSubject
      |> Observable.map EnvelopWithDefaults
      |> Observable.subscribe notifications.Add ignore ignore
      |> ignore

      Infrastructure.Configure 
         (users |> Users.ToUsers)
         (notifications |> Notifications.ToNotifications)
         (GlobalConfiguration.Configuration)