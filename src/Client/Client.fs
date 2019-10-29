module Client

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Thoth.Fetch
open Fulma
open Thoth.Json
open Shared
open ProductCard
open System
open Browser.WebStorage

// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model =
    { Products: Product list
      User: User }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
    | InitialStateLoaded of Model
    | FavoriteAdded of Guid
    | FavoriteRemoved of Guid

module UnitCoder =
    let encoder = fun _ -> Encode.string "null"
    let decoder = fun _ _ -> Ok()

let extraCoders =
    Extra.empty
    |> Extra.withDecimal
    |> Extra.withCustom UnitCoder.encoder UnitCoder.decoder

let initializeModel() =
    let productsPromise =
        Fetch.get<Product list> ("/api/products", ?isCamelCase = Some true, ?extra = Some extraCoders)

    let userPromise =
        promise {
            let userId =
                match localStorage.getItem "userId" with
                | userId when not (isNull userId) -> Guid.Parse(userId)
                | _ -> Guid.NewGuid()
            let! user = Fetch.get<User option>
                            (sprintf "/api/users/%O" userId, ?isCamelCase = Some true, ?extra = Some extraCoders)
            match user with
            | Some user -> return user
            | None ->
                do! Fetch.post (sprintf "/api/users/%O" userId, null, ?extra = Some extraCoders)
                localStorage.setItem ("userId", userId.ToString())
                return { Id = userId
                         Favorites = [] }
        }

    let orderFavoriteProducts products favorites =
        (products |> List.filter (fun product -> favorites |> List.contains (product.ProductId)))
        @ (products |> List.filter (fun product -> not (favorites |> List.contains (product.ProductId))))

    Promise.PromiseBuilder().Merge(productsPromise, userPromise,
                                   (fun products user ->
                                   { Products = orderFavoriteProducts products user.Favorites
                                     User = user }))

// defines the initial state and initial command (= side-effect) of the application
let init(): Model * Cmd<Msg> =
    let initialModel =
        { Products = List.empty
          User =
              { Id = Guid.Empty
                Favorites = List.empty } }

    let loadInitial = Cmd.OfPromise.perform initializeModel () InitialStateLoaded
    initialModel, loadInitial

let setFavorite selected userId productId =
    Fetch.post
        (sprintf "/api/users/%O/%s/%O" userId
             (match selected with
              | true -> "addFavorite"
              | false -> "removeFavorite") productId, null, ?extra = Some extraCoders) |> Promise.start

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg: Msg) (currentModel: Model): Model * Cmd<Msg> =
    match msg, currentModel with
    | InitialStateLoaded initialModel, _ -> initialModel, Cmd.none
    | FavoriteAdded favoriteAdded, _ ->
        setFavorite true currentModel.User.Id favoriteAdded
        let user = { currentModel.User with Favorites = currentModel.User.Favorites @ [ favoriteAdded ] }
        { currentModel with User = user }, Cmd.none
    | FavoriteRemoved favoriteRemoved, _ ->
        setFavorite false currentModel.User.Id favoriteRemoved
        let user =
            { currentModel.User with
                  Favorites = currentModel.User.Favorites |> List.filter (fun productId -> productId = favoriteRemoved) }
        { currentModel with User = user }, Cmd.none

let isUserFavorite productId favorites = favorites |> List.exists (fun favoriteId -> favoriteId = productId)

let show =
    function
    | { Products = products }, _ when products |> List.isEmpty -> [ Heading.h3 [] [ str "Loading..." ] ]
    | { Products = products; User = user }, dispatch ->
        products
        |> List.map (fun x ->
            productCard x (isUserFavorite x.ProductId user.Favorites) (fun selected product ->
                dispatch
                    (product.ProductId
                     |> match selected with
                        | true -> FavoriteAdded
                        | false -> FavoriteRemoved)))

let view (model: Model) (dispatch: Msg -> unit) =
    div [ ClassName "app" ] [ Container.container [ Container.Option.CustomClass "App-row" ] [ div [ ClassName "Products-grid" ] (show (model, dispatch)) ] ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.withConsoleTrace
|> Program.withReactBatched "elmish-app"
|> Program.withDebugger
|> Program.run
