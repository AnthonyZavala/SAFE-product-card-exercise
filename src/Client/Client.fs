module Client

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fetch.Types
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
      User: User option }

type FavoriteEvent =
    { UserId: Guid
      ProductId: Guid }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
    | InitialStateLoaded of Model
    | FavoriteAdded of FavoriteEvent
    | FavoriteRemoved of FavoriteEvent

module UnitCoder =
    let encoder = fun _ -> Encode.string "null"
    let decoder = fun _ _ -> Ok()

let extraCoders =
    Extra.empty
    |> Extra.withDecimal
    |> Extra.withCustom UnitCoder.encoder UnitCoder.decoder

let userId =
    match localStorage.getItem "userId" with
    | userId when not (isNull userId) -> userId
    | _ -> Guid.NewGuid().ToString()

let initializeModel() =
    let productsPromise =
        Fetch.get<Product list> ("/api/products", ?isCamelCase = Some true, ?extra = Some extraCoders)

    let userPromise =
        promise {
            let! user = Fetch.get<User option>
                            ("/api/users/" + userId, ?isCamelCase = Some true, ?extra = Some extraCoders)
            if user.IsNone then
                do! Fetch.post ("/api/users/" + userId, null, ?extra = Some extraCoders)
                localStorage.setItem ("userId", userId)
            return user
        }
    Promise.PromiseBuilder().Merge(productsPromise, userPromise,
                                   (fun products user ->
                                   { Products = products
                                     User = user }))

// defines the initial state and initial command (= side-effect) of the application
let init(): Model * Cmd<Msg> =
    let initialModel =
        { Products = List.empty
          User = None }

    let loadInitial = Cmd.OfPromise.perform initializeModel () InitialStateLoaded
    initialModel, loadInitial

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg: Msg) (currentModel: Model): Model * Cmd<Msg> =
    match currentModel.Products, msg with
    | _, InitialStateLoaded initialModel -> initialModel, Cmd.none
    // | _, FavoriteAdded favoriteAdded ->
    //     Fetch.tryPost
    //         ((sprintf "api/users/%O/addFavorite/%O" (favoriteAdded.UserId, favoriteAdded.ProductId)),
    //          Encode.nil)
    | _ -> currentModel, Cmd.none

let show =
    function
    | { Products = products } when products |> List.isEmpty -> [ Heading.h3 [] [ str "Loading..." ] ]
    | { Products = products } -> products |> List.map (fun x -> product x false (fun x y -> printfn "%b | %A" x y))

let view (model: Model) (dispatch: Msg -> unit) =
    div []
        [ Container.container []
              [ div
                  [ Style
                      [ Display DisplayOptions.Flex
                        FlexWrap "wrap" ] ] (show model) ] ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.withConsoleTrace
|> Program.withReactBatched "elmish-app"
|> Program.withDebugger
|> Program.run
