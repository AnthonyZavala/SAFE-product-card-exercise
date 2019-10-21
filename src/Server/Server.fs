open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared
open Newtonsoft.Json
open Thoth.Json.Net


let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath = Path.GetFullPath "../Client/public"

let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us

let products = JsonConvert.DeserializeObject<Product []>(
                File.ReadAllText "products.json", Converters.OptionConverter())

let webApp = router {
    get "/api/products" (json products) 
}

let extraCoders =
    Extra.empty
    |> Extra.withDecimal

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    use_json_serializer(Thoth.Json.Giraffe.ThothSerializer(isCamelCase = true, extra = extraCoders))
    use_gzip
}

run app
