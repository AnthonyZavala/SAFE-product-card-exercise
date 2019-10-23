namespace Domain

open System
open Xunit

module EventTests =
    let Given state: User = state
    let When event state = Aggregate.Apply state event
    let Then (actualState: User) (expectedState: User) = Assert.Equal(expectedState, actualState)

    let rdm = Random()

    [<Fact>]
    let ``Given initial user, When Created event, Then created user``() =
        let initialState = Aggregate.Initialize
        let aggregateId = Guid.NewGuid()

        Given initialState
        |> When(Created aggregateId)
        |> Then { initialState with Id = aggregateId }

    [<Fact>]
    let ``Given user, When FavoriteAdded event, Then favorite added to user``() =
        let initialState =
            { Id = Guid.NewGuid()
              Favorites = [] }
        let productId = Guid.NewGuid()

        Given initialState
        |> When(FavoriteAdded productId)
        |> Then { initialState with Favorites = [ productId ] }

    [<Fact>]
    let ``Given user, When FavoriteRemoved event, Then favorite removed from user``() =
        let productId = Guid.NewGuid()
        let initialState =
            { Id = Guid.NewGuid()
              Favorites = [ productId ] }

        Given initialState
        |> When(FavoriteRemoved productId)
        |> Then { initialState with Favorites = [] }
