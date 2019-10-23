namespace Domain

open System
open Xunit

module CommandTests =

    let Given state: User = state
    let When command state = Aggregate.Execute command state
    let Then (expectedEvent: UserEvent) (actualEvents: UserEvent List) =
        Assert.Equal<List<UserEvent>>([ expectedEvent ], actualEvents)

    let rdm = Random()

    let aggregateId = Guid.NewGuid()

    [<Fact>]
    let ``Given initial user, When Create command, Then Created event``() =
        Given Aggregate.Initialize
        |> When(Create aggregateId)
        |> Then(Created aggregateId)

    [<Fact>]
    let ``Given existing user, When AddFavorite command, Then FavoriteAdded event``() =
        let productId = Guid.NewGuid()

        Given
            { Id = Guid.NewGuid()
              Favorites = [] }
        |> When(AddFavorite productId)
        |> Then(FavoriteAdded productId)

    [<Fact>]
    let ``Given user, When RemoveFavorite command, Then FavoriteRemoved event``() =
        let productId = Guid.NewGuid()

        Given
            { Id = Guid.NewGuid()
              Favorites = [ productId ] }
        |> When(RemoveFavorite productId)
        |> Then(FavoriteRemoved productId)
