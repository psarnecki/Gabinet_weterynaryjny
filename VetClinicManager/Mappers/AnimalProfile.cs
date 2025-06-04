using AutoMapper;
using VetClinicManager.Models;
using VetClinicManager.DTOs;

namespace VetClinicManager.Mappers;

public class AnimalProfile : Profile
{
    public AnimalProfile()
    {
        CreateMap<Animal, AnimalDto>();
        CreateMap<CreateAnimalDto, Animal>();
        CreateMap<UpdateAnimalDto, Animal>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
