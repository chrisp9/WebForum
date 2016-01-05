namespace WebForum.WebServer.HttpApi

open System

[<AutoOpen>]
module Envelope =

   [<CLIMutable>]
   type AddUserMessage = {
      Username : string
      Password : string
      Email : string
   }
   
   [<CLIMutable>]
   type AuthenticateMessage = {
      Username : string
      Password : string
   }
   
   [<CLIMutable>]
   type AddPostMessage = {
      Title : string
      Message : string
      Username : string
   }

   [<CLIMutable>]
   type Envelope<'T> = {
      Id : Guid
      Created : DateTimeOffset
      Item : 'T
   }

   let Envelop id created item = {
      Id = id
      Created = created
      Item = item
   }

   let EnvelopWithDefaults item =
      // TODO Add a DateTime provider and use this here
      Envelop (Guid.NewGuid()) (DateTimeOffset.Now) item

   [<CLIMutable>]
   type Notification = {
      About : Guid
      Type : string
      Message : string
   }