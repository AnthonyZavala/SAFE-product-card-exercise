module ProductCard

open Fable.React
open Fable.React.Props
open Fable.MaterialUI.Core
open Fable.MaterialUI.Props
open Fable.MaterialUI.Icons
open Fable.Core.JsInterop
open Fable.Core

type ToggleButtonProps =
    | Value of string
    | Selected of bool
    | OnChange of (unit -> unit)

let inline toggleButton (props: ToggleButtonProps list) (elems: ReactElement list): ReactElement =
    ofImport "ToggleButton" "@material-ui/lab" (keyValueList CaseRules.LowerFirst props) elems
// module Mui = Fable.MaterialUI.Core
// module MuiProps = Fable.MaterialUI.Props

open Shared

type ProductProps =
    { product: Product
      isFavorite: bool
      setFavoriteCallback: bool -> Product -> unit }

let toImageUrl product =
    sprintf "//res.cloudinary.com/imperfect/image/upload/w_400,h_260,c_pad,b_auto,d_products:no-image-found.png/%s"
        product.ImageFilename

let ProductCard { product = product; isFavorite = isFavorite; setFavoriteCallback = setFavoriteCallback } =
    let state = Hooks.useState (isFavorite)
    div [ ClassName "Product-card" ]
        [ grid [ Container true ]
              [ grid
                  [ Item true
                    Xs(GridSizeNum.``12`` |> GridSize.Case3) ]
                    [ img
                        [ Src(product |> toImageUrl)
                          Alt product.Name
                          Style [ CSSProp.MaxWidth "100%" ] ] ]
                grid
                    [ Item true
                      Xs(GridSizeNum.``9`` |> GridSize.Case3) ]
                    [ div [] [ str product.Name ]
                      div []
                          [ str
                              (sprintf "$%A %s" product.Price
                                   (match product.PackageUnitAmount, product.PackageUnitFormatted with
                                    | Some amount, Some unit -> (sprintf "| %A %A" amount unit)
                                    | _ -> "")) ] ]
                grid
                    [ Item true
                      Xs(GridSizeNum.``3`` |> GridSize.Case3) ]
                    [ toggleButton
                        [ Value "check"
                          Selected state.current
                          OnChange(fun _ ->
                              state.update (fun s -> (not s))
                              setFavoriteCallback (not state.current) product) ] [ favoriteIcon [] ] ] ] ]

let inline productCard product isFavorite setFavoriteCallback =
    ofFunction ProductCard
        { product = product
          isFavorite = isFavorite
          setFavoriteCallback = setFavoriteCallback } []
