open System.IO
open Giraffe
open Saturn
open Shared
open Newtonsoft.Json
open Thoth.Json.Net
open UserService

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath = Path.GetFullPath "../Client/public"
let productsPath = Path.GetFullPath "./products.json"

let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us

let products = JsonConvert.DeserializeObject<Product []>(
                File.ReadAllText productsPath, Converters.OptionConverter())

let webApp = router {
    get "/api/products" (json products)
    getf "/api/users/%O" (GetUser >> json)
    getf "/api/users/%O/favorites" (GetUserFavorites >> json)
    postf "/api/users/%O" (CreateUser >> json)
    postf "/api/users/%O/addFavorite/%O" (AddFavorite >> json)
    postf "/api/users/%O/removeFavorite/%O" (RemoveFavorite >> json)
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
