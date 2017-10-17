module Core

open System.Net.Http
open System.Collections.Generic
open System.Net
open System

[<NoComparison>]
type Context = {
    handle: Guid
    request: HttpRequestMessage
    response: HttpResponseMessage
    data: Dictionary<string, obj>
    isOpen: bool
}

type Middleware = Context->Context
    
let (|OpenContext|ClosedContext|) (ctx:Context) =
    match ctx with
    | { isOpen = isOpen } when isOpen -> OpenContext
    | _ -> ClosedContext

let openContext (req:HttpRequestMessage) =
    {
        handle = Guid.NewGuid()
        request = req
        response = new HttpResponseMessage()
        data = new Dictionary<string, obj>()
        isOpen = true
    }
    
let bindContext (fn:Middleware) (ctx:Context) =
    match ctx with
    | OpenContext -> fn ctx
    | ClosedContext -> ctx
        
let closeContext (status:HttpStatusCode) (ctx:Context) =
    let close = (fun c ->
        c.response.StatusCode <- status
        { c with isOpen = false }
    )
    bindContext close ctx
        
let toResponse (codeIfOpen:HttpStatusCode) (ctx:Context) =
    match ctx with
    | OpenContext -> 
        ctx.response.StatusCode <- codeIfOpen
        ctx.response
    | ClosedContext -> 
        ctx.response