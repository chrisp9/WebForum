namespace WebForum.WebServer.HttpApi

open System.Net
open System.Net.Http
open System.Web.Http
open System.Reactive.Subjects
open System

type HomeController() =
   inherit ApiController()
   member this.Get() = new HttpResponseMessage()

type AddUserController() =
   inherit ApiController()
   let subject = new Subject<Envelope<AddUserMessage>>()

   member this.Post(message : AddUserWireMessage) =
      let cmd =
              {
                 AddUserMessage.Username = message.Username
                 Password = message.Password
                 Email = message.Email
              }
              |> EnvelopWithDefaults

      subject.OnNext(cmd)

      // TODO...
      new HttpResponseMessage(HttpStatusCode.Accepted)

   interface IObservable<Envelope<AddUserMessage>> with
      member this.Subscribe observer = subject.Subscribe observer
   override this.Dispose disposing =
      if disposing then subject.Dispose()
      base.Dispose disposing

type NotificationsController(notifications : Notifications.INotifications) =
   inherit ApiController()

   member this.Notifications = notifications