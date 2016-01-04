namespace WebForum.WebServer.HttpApi

open System

module Users =

   type IUsers =
      abstract WithUsername : string -> Envelope<User>
      inherit seq<Envelope<User>>

   type UsersInMemory(users : seq<Envelope<User>>) =
      interface IUsers with
         member this.WithUsername(name) =
            users 
            |> Seq.filter(fun x -> x.Item.Username = name) 
            |> Seq.head

         member this.GetEnumerator() =
            users.GetEnumerator()
         member this.GetEnumerator() =
            (this :> seq<Envelope<User>>).GetEnumerator() :> System.Collections.IEnumerator

   let ToUsers users = UsersInMemory(users)

   let Handle (users : seq<Envelope<User>>) (request : Envelope<AddUserMessage>) =
      match Seq.contains request users with
      | true -> None
      | false -> {
                    Email = request.Item.Email
                    Username = request.Item.Username
                    Password = request.Item.Password
                 } 
                 |> EnvelopWithDefaults
                 |> Some