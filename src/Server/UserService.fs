module UserService

open EventStoreRepo
open Domain

let projectUser userId = (Aggregate.Initialize, GetEvents userId) ||> List.fold Aggregate.Apply

// Get current user state and execute command to get event
let executeCommand userId command =
    let resultingEvent = projectUser userId |> Aggregate.Execute command
    match resultingEvent with
    | Errored(command, state) -> Some(command, state)
    | _ ->
        AddEvent userId resultingEvent
        None

// Project user state from events
let GetUser = projectUser

// Project user favorites from events
let GetUserFavorites userId = (projectUser userId).Favorites

// Execute create command for given userId
let CreateUser userId = executeCommand userId (Create userId)

// Execute add favorite command for given userId
let AddFavorite (userId, productId) = executeCommand userId (AddFavorite productId)

// Execute remove favorite command for given userId
let RemoveFavorite (userId, productId) = executeCommand userId (RemoveFavorite productId)
