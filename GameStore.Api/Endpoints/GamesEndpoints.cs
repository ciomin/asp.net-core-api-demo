using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    private static readonly List<GameDto> games =
    [
        new(1, "The Last of Us Part II", "Action-adventure", 59.99m, new DateOnly(2020, 6, 19)),
        new(2, "Ghost of Tsushima", "Action-adventure", 59.99m, new DateOnly(2020, 7, 17)),
        new(3, "Cyberpunk 2077", "Action role-playing", 59.99m, new DateOnly(2020, 12, 10))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        // GET /games
        group.MapGet("/", () => games);

        // GET /games/{id}
        group
            .MapGet(
                "/{id}",
                (int id) =>
                {
                    GameDto? game = games.Find(game => game.Id == id);

                    return game is null ? Results.NotFound() : Results.Ok(game);
                }
            )
            .WithName(GetGameEndpointName);

        // POST /games
        group.MapPost(
            "/",
            (CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                Game game =
                    new()
                    {
                        Name = newGame.Name,
                        Genre = dbContext.Genres.Find(newGame.GenreId),
                        GenreId = newGame.GenreId,
                        Price = newGame.Price,
                        ReleaseDate = newGame.ReleaseDate
                    };

                dbContext.Games.Add(game);
                dbContext.SaveChanges();

                GameDto gameDto = new GameDto(
                    game.Id,
                    game.Name,
                    game.Genre!.Name,
                    game.Price,
                    game.ReleaseDate
                );

                return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, gameDto);
            }
        );

        // PUT /games/{id}
        group.MapPut(
            "/{id}",
            (int id, UpdateGameDto updatedGame) =>
            {
                var index = games.FindIndex(game => game.Id == id);

                if (index == -1)
                {
                    return Results.NotFound();
                }

                games[index] = new GameDto(
                    id,
                    updatedGame.Name,
                    updatedGame.Genre,
                    updatedGame.Price,
                    updatedGame.ReleaseDate
                );

                return Results.NoContent();
            }
        );

        // DELETE /games/{id}
        group.MapDelete(
            "/{id}",
            (int id) =>
            {
                games.RemoveAll(game => game.Id == id);

                return Results.NoContent();
            }
        );

        return group;
    }
}
