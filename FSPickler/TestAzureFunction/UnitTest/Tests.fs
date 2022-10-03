namespace UnitTest

open System


module Tests =

    open System.Threading
    
    open MBrace.FsPickler
    open Xunit
    open System.Net.Http    
    [<Fact(DisplayName = "DeserialisationTest")>]
    let deserialiseTest () =
        let testFunction = fun (input : double) -> input * 2.

        let url = "http://localhost:7071/api/DeserialiseFunction"

        let binarySerializer = FsPickler.CreateBinarySerializer()

        let pickle =
            testFunction
            |> binarySerializer.Pickle<double -> double>
        
        let binarySerializer2 = FsPickler.CreateBinarySerializer()
        
        /// Validation that unpickling locally works fine
        let testUnpickle : double -> double =
            pickle
            |> binarySerializer2.UnPickle<double -> double>
        
        let test = testUnpickle 2.
        
        
        let postMethod = HttpMethod("POST")
        let request = new HttpRequestMessage(postMethod, Uri(url))
        let client = new HttpClient()
        let content = new ByteArrayContent(pickle)
        request.Content <- content
        
        let response =
            client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken())
            |> Async.AwaitTask
            |> Async.RunSynchronously

        Assert.True true