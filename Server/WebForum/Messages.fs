namespace WebForum.WebServer.HttpApi

open System

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