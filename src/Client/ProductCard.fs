module ProductCard

open Fable.React
open Fable.React.Props
open Fable.MaterialUI.Core
open Fable.MaterialUI.Props

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

let Product { product = product; isFavorite = isFavorite; setFavoriteCallback = setFavoriteCallback } =
    div
        [ ClassName "Product-Card"
          Style
              [ Border "1px solid gray"
                CSSProp.BorderRadius "4px"
                CSSProp.Margin "36px"
                Width "20%" ] ]
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
                              (sprintf "$%A | %A %A" product.Price product.PackageUnitAmount
                                   product.PackageUnitFormatted) ] ] ] ]

let inline product product isFavorite setFavoriteCallback =
    ofFunction Product
        { product = product
          isFavorite = isFavorite
          setFavoriteCallback = setFavoriteCallback } []
