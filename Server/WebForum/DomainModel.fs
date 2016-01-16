namespace WebForum.WebServer.HttpApi

open System

module Users =

   type IUsers =
      abstract WithUsername : string -> Envelope<User> option
      inherit seq<Envelope<User>>

   type UsersInMemory(users : seq<Envelope<User>>) =
      interface IUsers with
         member this.WithUsername(name) =
            let usersWithName = users |> Seq.filter(fun x -> x.Item.Username = name) 
            match Seq.length usersWithName with
            | 0 -> Some (Seq.head usersWithName)
            | 1 -> None
            | _ -> ArgumentException("There is more than one user with the provided username") |> raise

         member this.GetEnumerator() =
            users.GetEnumerator()
         member this.GetEnumerator() =
            (this :> seq<Envelope<User>>).GetEnumerator() :> System.Collections.IEnumerator

   let ToUsers users = UsersInMemory(users)

   let HandleAddUser (users : IUsers) (request : Envelope<AddUserMessage>) =
      match users.WithUsername(request.Item.Username) with
      | Some _ -> None
      | None -> {
                    User.Email = request.Item.Email
                    Username = request.Item.Username
                    Password = request.Item.Password
                 } 
                 |> EnvelopWithDefaults
                 |> Some

   let HandleAuth (users : IUsers) (request : Envelope<AuthenticateMessage>) =
      match users.WithUsername(request.Item.Username) with
      | Some x -> {
                     SessionMessage.Username = request.Item.Username
                  } |> EnvelopWithDefaults |> Some
      | None -> None

module Notifications =
   type INotifications =
      inherit seq<Envelope<Notification>>
      abstract About : Guid -> seq<Envelope<Notification>>

   type NotificationsInMemory(notifications : Envelope<Notification> seq) =
      interface INotifications with
         member this.About id =
            notifications |> Seq.filter (fun x -> x.Item.About = id)
         member this.GetEnumerator() = notifications.GetEnumerator()
         member this.GetEnumerator() = (this :> Envelope<Notification> seq).GetEnumerator() :> System.Collections.IEnumerator

   let ToNotifications notifications = NotificationsInMemory(notifications)
   let About id (notifications : INotifications) = notifications.About id