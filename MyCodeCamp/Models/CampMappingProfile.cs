using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyCodeCamp.Data.Entities;

namespace MyCodeCamp.Models
{
    public class CampMappingProfile : Profile
    {
        public CampMappingProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(c => c.StartDate, 
                    opt => opt.MapFrom(camp => camp.EventDate))
                .ForMember(c => c.EndDate, 
                    opt => opt.ResolveUsing(Camp => Camp.EventDate.AddDays(Camp.Length - 1)))
                .ForMember(c => c.Url,
                    opt => opt.ResolveUsing(camp =>
                    {
                        
                    }));
        }
    }
}
