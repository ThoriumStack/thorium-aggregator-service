using AutoMapper;
using Thorium.Core.Model.DataModel;
using Thorium.Core.Model.DtoModel;

namespace Thorium.Aggregator
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            

            CreateMap<BaseModel, BaseDtoModel>()
                .ReverseMap()
                .ForMember(c => c.CreatedById, opt => opt.Ignore())
                .ForMember(c => c.CreatedDate, opt => opt.Ignore())
                .ForMember(c => c.ModifiedById, opt => opt.Ignore())
                .ForMember(c => c.ModifiedDate, opt => opt.Ignore())
                .ForMember(c => c.TimeStamp, opt => opt.Ignore());
        }
    }
}