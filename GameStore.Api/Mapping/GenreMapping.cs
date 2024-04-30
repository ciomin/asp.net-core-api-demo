using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GenreMapping
{
    public static GenreDto ToGenreDto(this Genre genre) => new GenreDto(genre.Id, genre.Name);
}
