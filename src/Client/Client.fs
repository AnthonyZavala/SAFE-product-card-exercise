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

// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model =
    { Products: Product list }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg = InitialProductsLoaded of Product list

let extraCoders = Extra.empty |> Extra.withDecimal

let initialProducts() =
    Fetch.fetchAs<Product list> ("/api/products", ?isCamelCase = Some true, ?extra = Some extraCoders)

// defines the initial state and initial command (= side-effect) of the application
let init(): Model * Cmd<Msg> =
    let initialModel = { Products = List.empty }
    let loadProductsCmd = Cmd.OfPromise.perform initialProducts () InitialProductsLoaded
    initialModel, loadProductsCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg: Msg) (currentModel: Model): Model * Cmd<Msg> =
    match currentModel.Products, msg with
    | _, InitialProductsLoaded initialProducts ->
        let nextModel = { Products = initialProducts }
        nextModel, Cmd.none
// | _ -> currentModel, Cmd.none

let toImageUrl product =
    sprintf "//res.cloudinary.com/imperfect/image/upload/w_400,h_260,c_pad,b_auto,d_products:no-image-found.png/%s"
        product.ImageFilename

let show =
    function
    | { Products = products } when products |> List.isEmpty -> [ Heading.h3 [] [ str "Loading..." ] ]
    | { Products = products } -> products |> List.map (fun x -> product x false (fun x y -> ()))

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
