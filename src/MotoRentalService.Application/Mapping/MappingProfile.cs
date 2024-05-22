using AutoMapper;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Application.MediatR.Commands.Auth;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;
using MotoRentalService.Application.MediatR.Commands.Plans;
using MotoRentalService.Application.MediatR.Commands.Rental;
using MotoRentalService.Application.MediatR.Commands.User;

namespace MotoRentalService.Application.Mapping
{
    /// <summary>
    /// Profile for AutoMapper mappings between DTOs and entities.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<UpdateMotoCommand, Motorcycle>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.LicensePlate));

            CreateMap<CreateMotoCommand, Motorcycle>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<CreatePlanCommand, RentalPlans>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdatePlanCommand, RentalPlans>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<CreateRentalCommand, Rental>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<CreateUserDeliveryCommand, DeliveryUser>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.AuthUserId))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateDeliveryUserCommand, DeliveryUser>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());


            CreateMap<LoginAuthUserCommand, Users>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<RegisterAuthUserCommand, Users>()
               .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
               .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // DTOs to Entities and reverse map
            CreateMap<MotorcycleDto, Motorcycle>().ReverseMap();
            CreateMap<DeliveryUserDto, DeliveryUser>().ReverseMap();
            CreateMap<RentalDto, Rental>().ReverseMap();
            CreateMap<PlanDto, RentalPlans>().ReverseMap();
        }
    }

}


