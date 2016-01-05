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

[<CLIMutable>]
type NotificationWireMessage = {
   About : string
   Type : string
   Message : string
}

[<CLIMutable>]
type NotificationListWireMessage = {
   Notifications : NotificationWireMessage array
}

[<CLIMutable>]
type LinkWireMessage = {
   Rel : string
   Href : string
}

type LinkListWireMessage = {
   Links : LinkWireMessage array
}