namespace WebForum.WebServer.HttpApi

open System

[<CLIMutable>]
type AddUserWireMessage = {
   Username : string
   Password : string
   Email : string
}

[<CLIMutable>]
type AuthenticateWireMessage = {
   Username : string
   Password : string
}

[<CLIMutable>]
type AddPostWireMessage = {
   Title : string
   Message : string
   Username : string
}