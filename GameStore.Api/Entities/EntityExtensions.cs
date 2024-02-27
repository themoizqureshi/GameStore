using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.DTOs;

namespace GameStore.Api.Entities;

public static class EntityExtensions
{
    public static GameDTO AsDTO(this Game game)
    {
        return new GameDTO(
            game.Id,
            game.Name,
            game.Genre,
            game.Price,
            game.ReleaseDate,
            game.ImageUri
        );
    }
}