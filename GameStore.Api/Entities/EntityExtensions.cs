using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.DTOs;

namespace GameStore.Api.Entities;

public static class EntityExtensions
{
    public static GameDTOV1 AsDTOV1(this Game game)
    {
        return new GameDTOV1(
            game.Id,
            game.Name,
            game.Genre,
            game.Price,
            game.ReleaseDate,
            game.ImageUri
        );
    }
    public static GameDTOV2 AsDTOV2(this Game game)
    {
        return new GameDTOV2(
            game.Id,
            game.Name,
            game.Genre,
            game.Price - (game.Price * .3m),
            game.Price,
            game.ReleaseDate,
            game.ImageUri
        );
    }
}