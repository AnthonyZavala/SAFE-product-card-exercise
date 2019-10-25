module EventStoreRepo

open System
open Domain

let mutable private events = Map<Guid, List<UserEvent>> []

let GetEvents id = events.TryFind id |> Option.defaultValue []

let AddEvent id event = events <- events.Add(id, GetEvents id @ [ event ])
