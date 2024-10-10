using AutoMapper;
using Biograf_Repository.DTO;
using Biograf_Repository.Models;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biograf_Repository.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<Movie, MovieDTO>()
            .ForMember(dest => dest.GenreIds, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => mg.GenreId)))
            .ReverseMap()
            .ForMember(dest => dest.MovieGenres, opt => opt.Ignore());
            CreateMap<Showtime, ShowtimeDTO>().ReverseMap();
            CreateMap<Cinema, CinemaDTO>().ReverseMap();
            CreateMap<Seat, SeatDTO>().ReverseMap();
            CreateMap<Reservation, ReservationDTO>().ReverseMap();
            CreateMap<Payment, PaymentDTO>().ReverseMap();
            CreateMap<Hall, HallDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Ticket, TicketDTO>().ReverseMap();
        }
    }
}
