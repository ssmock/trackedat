//
// TODO
//

//namespace WebScriptContext

//module Validation = 
    
//    open WebScriptContext.Core
//    open WebScriptContext.Content
//    open System.Net
    
//    let setValidationMessages (messages:list<string>) (ctx:Context) =
//        match ctx with
//        | OpenContext -> 
//            match messages with
//            | _ when messages.Length > 0 ->
//                setJsonContent messages ctx 
//                |> ignore
//                closeContext HttpStatusCode.BadRequest ctx
//            | _ -> ctx
//        | ClosedContext -> ctx

//    let validateWith (validator:Context->list<string>) (ctx:Context) =
//        let validate (c:Context) =
//            let messages = validator c
//            setValidationMessages messages c

//        bindContext validate ctx
        
//    let validateWithMany (validators:seq<Context->list<string>>) (ctx:Context) =
//        let validate (c:Context) =
//            let messages =
//                validators
//                |> Seq.fold (fun msgs f -> (f c) @ msgs) (list<string>.Empty)

//            setValidationMessages messages ctx
        
//        bindContext validate ctx