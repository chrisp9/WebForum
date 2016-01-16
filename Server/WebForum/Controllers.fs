namespace WebForum.WebServer.HttpApi

open System.Net
open System.Net.Http
open System.Web.Http
open System.Reactive.Subjects
open System
open TokenAuthorization.Core
open FSharp.Interop.Dynamic
open TokenAuthorization.Core.Attributes
open TokenAuthorization.Core.Account

type HomeController() =
   inherit ApiController()
   member this.Get() = new HttpResponseMessage()


type AuthController() =
   inherit Controllers.TokenAuthApiController()

   let createResponse (controller : ApiController) (cmd : Envelope<AuthenticateMessage>)= 
                              controller.Request.CreateResponse(
                                 HttpStatusCode.Accepted,
                                 {
                                    Links =
                                       [| {
                                          Rel = "/authprovider"
                                          Href = "/authprovider/" + cmd.Item.Username.ToString() } |]})

   [<ActionName("register")>]
   [<TokenAuthentication(AccessLevel.Anonymous)>]
   member this.PostRegister(message : AddUserWireMessage) =
      this.Ok()

   [<TokenAuthentication(AccessLevel.Anonymous)>]
   [<ActionName("login")>]
   member this.PostLogin([<FromBody>] user : AuthenticateWireMessage) =  
      match user.Username, user.Password with
      | null, _  | _, null ->
         this.Error("Please enter username and password")
      | _ ->
         this.UserData?username <- user.Username
         this.UserData?password <- user.Password
         this.Error("Please enter username and password")

   member this.PostLogout(user : LogoutMessage) = 
      this.Logout()
