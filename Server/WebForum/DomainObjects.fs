namespace WebForum.WebServer.HttpApi

open System

   [<CLIMutable>]
   type User = {
      Username : string
      Password : string
      Email : string
   }

   [<CLIMutable>]
   type BoardPost = {
      Title : string
      Content : string
      Owner : User
   }

   [<CLIMutable>]
   type Board = {
      Title : string
      Description : string
      Content : BoardPost array
   }