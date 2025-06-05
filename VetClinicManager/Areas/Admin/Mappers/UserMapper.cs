using Riok.Mapperly.Abstractions;
using VetClinicManager.Areas.Admin.DTOs;
using VetClinicManager.Areas.Admin.DTOs.User;
using VetClinicManager.Models;

namespace VetClinicManager.Areas.Admin.Mappers;

[Mapper]
public partial class UserMapper 
{
    // Mapowanie z User na UserListDto (Index)
    public partial UserListDto ToUserListDtoFromUser(User user);
    
    // Mapowanie z User na UserEditDto (Edit GET)
    public partial UserEditDto ToUserEditDto(User user);

    // Mapowanie z UserCreateDto na User (Create POST)
    [MapProperty(nameof(UserCreateDto.Email), nameof(User.UserName))]
    [MapValue(nameof(User.EmailConfirmed), true)]
    [MapperIgnoreSource(nameof(UserCreateDto.Password))]
    [MapperIgnoreSource(nameof(UserCreateDto.ConfirmPassword))]
    public partial User ToUser(UserCreateDto userDto);

    // Mapowanie z UserEditDto na istniejący User (Edit POST)
    [MapperIgnoreSource(nameof(UserEditDto.Id))]
    [MapperIgnoreSource(nameof(UserEditDto.Email))]
    [MapperIgnoreTarget(nameof(User.UserName))]
    [MapperIgnoreSource(nameof(UserEditDto.AvailableRoles))]
    [MapperIgnoreSource(nameof(UserEditDto.SelectedRoles))]
    public partial void UpdateUserFromDto(UserEditDto userDto, User user);

    // Mapowanie z User na UserDeleteDto (Delete GET)
    public partial UserDeleteDto ToUserDeleteDto(User user);
}
