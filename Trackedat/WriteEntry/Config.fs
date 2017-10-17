module Config

#if COMPILED
let getConnectionString () = 
    System.Configuration.ConfigurationManager.ConnectionStrings.["TrackedatWriter"].ConnectionString
#else                        
open System
let getConnectionString () =
    let path = __SOURCE_DIRECTORY__ + "\..\App.config"
    let connectionNode = 
        System.Xml.Linq.XDocument.Load(path).Root.Descendants() 
        |> Seq.find (fun n -> 
            n.Attributes()
            |> Seq.exists (fun a -> a.Value = "TrackedatWriter"))

    let stringAttributeName = System.Xml.Linq.XName.Get("connectionString")
    connectionNode.Attribute(stringAttributeName).Value
#endif