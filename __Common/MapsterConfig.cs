using Mapster;
using MapsterMapper;

using TechStoreApi.Entities;
using TechStoreApi.Dtos;
using TechStoreApi.Auth;

namespace TechStoreApi.Common;

public static class MapsterConfig
{
    private static IFileService _fileService;
    public static void Initialize(IFileService fileService) => _fileService = fileService;

    public static void AddMapster(this IServiceCollection services)
    {
        // register mapster with Dependency Injection Container [services]
        services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        services.AddScoped<IMapper, ServiceMapper>();

        // User mappings
        TypeAdapterConfig<User, UserDto>.NewConfig()
            .Map(dest => dest.Gender, src => src.GenderId.ToString())
            .TwoWays();
        TypeAdapterConfig<User, UserInput>.NewConfig();
        TypeAdapterConfig<UserInput, User>.NewConfig()
            .Map(dest => dest.UserName, src => src.Email);
        TypeAdapterConfig<dynamic, UserDto>.NewConfig()
            .TwoWays();

        // Role mappings
        TypeAdapterConfig<Role, RoleDto>.NewConfig()
            .TwoWays();
        TypeAdapterConfig<dynamic, RoleDto>.NewConfig()
            .TwoWays();

        // RefreshToken mappings
        TypeAdapterConfig<RefreshToken, TokenDto>.NewConfig()
            .TwoWays();

    }
}
