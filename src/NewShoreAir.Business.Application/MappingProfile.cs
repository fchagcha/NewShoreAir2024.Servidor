namespace NewShoreAir.Business.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CalcularRutaResponse, Viaje>();
            CreateMap<ViajeDto, Viaje>();
            CreateMap<VueloDto, Vuelo>()
                 .ForMember(x => x.Transporte, opt => opt.Ignore());

            CreateMap<TransporteDto, Transporte>();

            CreateMap<Viaje, CalcularRutaResponse>();
            CreateMap<Viaje, ViajeDto>();
            CreateMap<Vuelo, VueloDto>()
                .ForMember(x => x.Transporte, opt => opt.Ignore());

            CreateMap<Transporte, TransporteDto>();
        }
    }
}