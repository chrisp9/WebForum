namespace WebForum.WebServer.HttpApi

open System

   [<CLIMutable>]
   type User = {
      Username : string
      Password : string
      Email : string
   }