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
open System


let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath = Path.GetFullPath "../Client/public"

let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us

let products = JsonConvert.DeserializeObject<Product []>(
                File.ReadAllText "products.json", Converters.OptionConverter())

let getUser userId = 
    printfn "%s" (userId.ToString())
    json (userId.ToString())
    
let getUserFavorites userId = 
    printfn "%s" (userId.ToString())
    json (userId.ToString())

let createUser userId = 
    printfn "%s" (userId.ToString())
    json (userId.ToString())

let addUserFavorite (userId: Guid, productId: Guid) = 
    printfn "%s | %s" (userId.ToString()) (productId.ToString())
    json (productId.ToString())

let removeUserFavorite (userId: Guid, productId: Guid) = 
    printfn "%s | %s" (userId.ToString()) (productId.ToString())
    json (productId.ToString())

let webApp = router {
    get "/api/products" (json products)
    getf "/api/users/%O" getUser
    getf "/api/users/%O/favorites" getUserFavorites
    postf "/api/users/%O" createUser
    postf "/api/users/%O/addFavorite/%O" addUserFavorite
    postf "/api/users/%O/removeFavorite/%O" removeUserFavorite
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
