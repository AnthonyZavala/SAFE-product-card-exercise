module EventStore

open System
open Domain

let private users = Map<Guid, List<User>> []

let GetEvents id =
    id
    |> users.TryFind
    |> Option.defaultValue []