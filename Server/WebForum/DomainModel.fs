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
            | 0 -> None
            | 1 -> Some (Seq.head usersWithName)
            | _ -> ArgumentException("There is more than one user with the provided username") |> raise

         member this.GetEnumerator() =
            users.GetEnumerator()
         member this.GetEnumerator() =
            (this :> seq<Envelope<User>>).GetEnumerator() :> System.Collections.IEnumerator

   let ToUsers users = UsersInMemory(users)

   let Handle (users : IUsers) (request : Envelope<AddUserMessage>) =
      match users.WithUsername(request.Item.Username) with
      | Some _ -> None
      | None -> {
                    User.Email = request.Item.Email
                    Username = request.Item.Username
                    Password = request.Item.Password
                 } 
                 |> EnvelopWithDefaults
                 |> Some