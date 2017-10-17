module Content
    
open System.Net.Http
open System.Text
open System.IO
open Newtonsoft.Json
open Core
    
let extractJsonBodyContent<'T> (ctx:Context) =
    let readStream = ctx.request.Content.ReadAsStreamAsync()
    readStream.Wait()
    let stream = new StreamReader(readStream.Result)
    let bodyString = stream.ReadToEnd()
    let deserialized = JsonConvert.DeserializeObject<'T>(bodyString)
        
    ctx.data.["body"] <- deserialized

    ctx
    
let contextBodyAs<'T> (ctx:Context) =
    // TODO: Ensure that there is a body first
    ctx.data.["body"] :?> 'T

let setJsonResponseContent (data) (ctx:Context) =
    let messageData = JsonConvert.SerializeObject(data)
    ctx.response.Content <- new StringContent(messageData, Encoding.UTF8, "application/json")
    ctx