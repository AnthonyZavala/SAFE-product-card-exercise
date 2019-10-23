module Domain

open System
open EventSourcing

// Record
type User =
    { Id: Guid
      Favorites: Guid list }
    static member Initialize =
        { Id = Guid.Empty
          Favorites = [] }

// Discriminated Union
type UserCommand =
    | Create of userId: Guid
    | AddFavorite of productId: Guid
    | RemoveFavorite of productId: Guid

// Discriminated Union
type UserEvent =
    | Created of userId: Guid
    | FavoriteAdded of productId: Guid
    | FavoriteRemoved of productId: Guid
    | Errored of command: UserCommand * state: User

// Pattern Matching
let private apply (state: User) event =
    match event with
    | Created userId -> { state with Id = userId }
    | FavoriteAdded productId -> { state with Favorites = state.Favorites @ [ productId ] }
    | FavoriteRemoved productId -> { state with Favorites = state.Favorites |> List.except [ productId ] }
    | _ -> state

// Pattern Matching
let private execute command (state: User) =
    match command with
    | Create userId when state.Id = Guid.Empty -> [ Created userId ]
    | AddFavorite productId when state.Id <> Guid.Empty && not (state.Favorites |> List.contains productId) ->
        [ FavoriteAdded productId ]
    | RemoveFavorite productId when state.Id <> Guid.Empty && state.Favorites |> List.contains productId ->
        [ FavoriteRemoved productId ]
    | _ -> [ Errored(command, state) ]

let Aggregate =
    { Initialize = User.Initialize
      Apply = apply
      Execute = execute }
