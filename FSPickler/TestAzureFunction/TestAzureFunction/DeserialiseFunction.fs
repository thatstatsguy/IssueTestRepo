namespace TestAzureFunction

open System
open System.IO
open System.Net.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open Microsoft.Extensions.Logging
open MBrace.FsPickler

module DeserialiseFunction =

    [<FunctionName("DeserialiseFunction")>]
    let run ([<HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)>] req: HttpRequestMessage) (log: ILogger) =
        async {
            
            let binarySerializer = FsPickler.CreateBinarySerializer()

            let unpickle =
                req.Content.ReadAsByteArrayAsync()
                |> Async.AwaitTask
                |> Async.RunSynchronously

            let test =
                unpickle
                |> binarySerializer.UnPickle<double->double>

            let testResult = test 2.
            return OkObjectResult() :> IActionResult
            
        } |> Async.StartAsTask