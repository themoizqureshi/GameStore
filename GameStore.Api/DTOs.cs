using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.DTOs;

public record GetGamesDTOV1(
    int pageNumber = 1,
    int pageSize = 5,
    string? filter = null
);
public record GameDTOV1(
    int Id,
    string Name,
    string Genre,
    decimal Price,
    DateTime ReleaseDate,
    string ImageUri
);
public record GetGamesDTOV2(
    int pageNumber = 1,
    int pageSize = 5,
    string? filter = null
);
public record GameDTOV2(
    int Id,
    string Name,
    string Genre,
    decimal Price,
    decimal RetailPrice,
    DateTime ReleaseDate,
    string ImageUri
);

public record CreateGameDTO(
    int Id,
    [Required][StringLength(50)]
    string Name,
    [Required][StringLength(20)]
    string Genre,
    [Range(1, 100)]
    decimal Price,
    DateTime ReleaseDate,
    [Url][StringLength(100)]
    string ImageUri
);
public record UpdateGameDTO(
    int Id,
    [Required][StringLength(50)]
    string Name,
    [Required][StringLength(20)]
    string Genre,
    [Range(1, 100)]
    decimal Price,
    DateTime ReleaseDate,
    [Url][StringLength(100)]
    string ImageUri
);

