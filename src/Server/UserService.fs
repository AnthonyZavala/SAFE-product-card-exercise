module UserService

open EventStoreRepo
open Domain
open System

let private projectUser userId = (Aggregate.Initialize, GetEvents userId) ||> List.fold Aggregate.Apply

// Get current user state and execute command to get event
let private executeCommand userId command =
    let resultingEvent = projectUser userId |> Aggregate.Execute command
    match resultingEvent with
    | Errored(command, state) -> Some(command, state)
    | _ ->
        AddEvent userId resultingEvent
        None

let private domainToSharedUser user: Shared.User option =
    match user with
    | user when user.Id <> Guid.Empty ->
        Some
            { Id = user.Id
              Favorites = user.Favorites }
    | _ -> None

// Project user state from events
let GetUser = projectUser >> domainToSharedUser

// Project user favorites from events
let GetUserFavorites userId =
    match userId |> (projectUser >> domainToSharedUser) with
    | Some user -> Some user.Favorites
    | None -> None

// Execute create command for given userId
let CreateUser userId = executeCommand userId (Create userId)

// Execute add favorite command for given userId
let AddFavorite(userId, productId) = executeCommand userId (AddFavorite productId)

// Execute remove favorite command for given userId
let RemoveFavorite(userId, productId) = executeCommand userId (RemoveFavorite productId)
