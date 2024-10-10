using Biograf_Repository.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationSeat> ReservationSeats { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {           
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Showtimes)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);
           
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId);

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreId);
          
            modelBuilder.Entity<Cinema>()
                .HasMany(c => c.Halls)
                .WithOne(h => h.Cinema)
                .HasForeignKey(h => h.CinemaId)
                .OnDelete(DeleteBehavior.Restrict); 
        
            modelBuilder.Entity<Hall>()
                .HasMany(h => h.Showtimes)
                .WithOne(s => s.Hall)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Restrict);  
            
            modelBuilder.Entity<Hall>()
                .HasMany(h => h.Seats)
                .WithOne(s => s.Hall)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Restrict); 
         
            modelBuilder.Entity<Showtime>()
                .HasMany(s => s.Tickets)
                .WithOne(t => t.Showtime)
                .HasForeignKey(t => t.ShowtimeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Seat>()
                .HasMany(s => s.ReservationSeats)
                .WithOne(rs => rs.Seat)
                .HasForeignKey(rs => rs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
           
            modelBuilder.Entity<ReservationSeat>()
                .HasOne(rs => rs.Showtime)
                .WithMany(s => s.ReservationSeats)
                .HasForeignKey(rs => rs.ShowtimeId)
                .OnDelete(DeleteBehavior.Restrict);
         
            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.ReservationSeats)
                .WithOne(rs => rs.Reservation)
                .HasForeignKey(rs => rs.ReservationId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Payment>()
                .HasMany(p => p.Tickets)
                .WithOne(t => t.Payment)
                .HasForeignKey(t => t.PaymentId)
                .OnDelete(DeleteBehavior.Restrict); 
          
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<User>()
                .HasOne(u => u.Payment)
                .WithOne(p => p.User)
                .HasForeignKey<Payment>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
       
            modelBuilder.Entity<ReservationSeat>()
                .HasKey(rs => new { rs.ReservationId, rs.SeatId, rs.ShowtimeId }); 
          
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });
        }
    }
}
