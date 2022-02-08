using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{ 
    // AutoMapper helps us map one object to another
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // create mapping from AppUser to MemberDto
            CreateMap<AppUser, MemberDto>().ForMember(dest => dest.PhotoUrl, 
                                                      opt => opt.MapFrom(
                                                          src => src.Photos.FirstOrDefault(
                                                              x => x.IsMain).Url))
                                           .ForMember(dest => dest.Age, 
                                                      opt => opt.MapFrom(
                                                          src => src.DateOfBirth.CalculateAge()));
            
            // create mapping from Photo to  PhotoDto
            CreateMap<Photo, PhotoDto>();
        }

        // since this will become an injectable service, it needs to be added to our service extensions in ApplicationServiceExtensions
    }
}