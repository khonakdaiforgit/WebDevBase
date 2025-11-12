using AutoMapper;
using MyApp.Application.DTOs.User;
using MyApp.Domain.Entities;


namespace MyApp.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            //CreateMap<User, RegisterUserDto>().ReverseMap();
            //CreateMap<User, LoginDto>().ReverseMap();
            //CreateMap<User, UserUpdateDto>().ReverseMap();
            //CreateMap<User, UserDto>().ReverseMap();
            //CreateMap<FileLink, FileLinkDto>().ReverseMap();
            //CreateMap<Transaction, TransactionDto>().ReverseMap();
            //CreateMap<LogEntry, LogEntryDto>().ReverseMap();
            //CreateMap<LogStats, LogStatsDto>().ReverseMap();
            //CreateMap<UserProfileStats, UserProfileDto>().ReverseMap();
            //CreateMap<ContactMessage, ContactMessageCreateDto>().ReverseMap();
            //CreateMap<ContactMessage, ContactMessageDto>().ReverseMap();

            //CreateMap<PagedResult<Transaction>, PagedResultDto<TransactionDto>>().ReverseMap();
            //CreateMap<PagedResult<ContactMessage>, PagedResultDto<ContactMessageDto>>().ReverseMap();

        }
    }
}
