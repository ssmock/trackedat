module Authorization
    
open Content
open Domain
open System.Data.SqlClient
open Dapper
open System.Data
open System
open Core
open System.Net
open System.Collections.Generic
    
type Tenant = {
    tenantId:int
    tenantHash:string
    nickName:string
}

let mutable tenantRegistry = Map.empty<string,Tenant>

let tenantFromRecord (record:obj) = record :?> Tenant
    //let recordDictionary = record :?> IDictionary<string,obj>
    //{
    //    tenantId = Convert.ToInt32(recordDictionary.["TenantId"])
    //    tenantHash = recordDictionary.["TenantHash"].ToString()
    //    nickName = recordDictionary.["NickName"].ToString()
    //}

let refreshTenantRegistry =
    let con = new SqlConnection(Config.getConnectionString())
    con.Open();
    tenantRegistry <- con.Query<Tenant>(
        "auth.GetTenants", 
        commandType = Nullable<CommandType> CommandType.StoredProcedure)
        //|> Seq.map tenantFromRecord
        |> Seq.map (fun t -> t.tenantHash, t)
        |> Map.ofSeq<string,Tenant> 
    con.Close();
    tenantRegistry

let tenantExists (tenantHash:string) =
    if tenantRegistry |> Map.containsKey tenantHash then true
    else
        refreshTenantRegistry
        |> Map.containsKey tenantHash

let authorize ctx:Context =
    ctx
    |> contextBodyAs<Entry>
    |> (fun entry -> entry.tenantHash)
    |> (fun hash ->
            if tenantExists hash then
                closeContext HttpStatusCode.Unauthorized ctx
            else ctx)
    |> ignore
    ctx