#if COMPILED
// Cloud path
#r "../../../data/Functions/packages/nuget/dapper/1.50.2/lib/net451/Dapper.dll"
#else
#r "../../packages/Dapper.1.50.2/lib/net451/Dapper.dll"
#r "System.Xml.Linq"
#endif

#r "System.Net.Http"
#r "Newtonsoft.Json"
#r "System.Configuration"
#r "System.Data"
#load "Domain.fs"
#load "Config.fs"
#load "Core.fs"
#load "Content.fs"
#load "Authorization.fs"

open System.Net.Http
open System.Net
open Core
open Content
open Authorization
open System.Data.SqlClient
open System.Data
open System
open Dapper
open Config
open Domain

let write connectionString parameters =
    let connection = new SqlConnection(connectionString)
    
    connection.Open()
    
    let cmd = new CommandDefinition(
                commandText = "trans.InsertSimpleLog", 
                parameters = parameters,
                commandType = Nullable<CommandType> CommandType.StoredProcedure)    
    connection.Execute cmd |> ignore    
    connection.Close()
    ignore

let processBody ctx:Context =
    ctx
    |> contextBodyAs<Entry>
    |> (fun args -> { args with created = DateTime.UtcNow })
    |> write (getConnectionString ())
    |> ignore
    ctx

let Run(req: HttpRequestMessage) =
    async {
        let resp = 
            openContext req
            |> bindContext extractJsonBodyContent<Entry>
            |> bindContext authorize
            |> bindContext processBody
            |> toResponse HttpStatusCode.OK

        return resp
    } |> Async.RunSynchronously