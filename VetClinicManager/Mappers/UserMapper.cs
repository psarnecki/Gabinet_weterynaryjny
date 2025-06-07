using Riok.Mapperly.Abstractions;
using VetClinicManager.DTOs.Users.UserBriefs;
using VetClinicManager.DTOs.Visits.VisitBriefs;
using VetClinicManager.Models;

namespace VetClinicManager.Mappers;

[Mapper]
public partial class UserMapper
{   
    // Mapowanie modelu User na UserBriefDto (używane dla Właściciela w listach zwierząt/wizyt)
    public partial UserBriefDto ToUserBriefDto(User user);

    // Mapowanie modelu User na VisitVetBriefDto (używane dla Przypisanego Weta w liście wizyt dla Użytkownika)
    public partial VisitVetBriefDto ToVisitVetBriefDto(User user);
}