﻿namespace MoviesAPI.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public int Year { get; set; }
        
        [MaxLength(250)]
        public string Title { get; set; }
        
        [MaxLength(2500)]
        public string StoreLine { get; set; }
        
        public double Rate { get; set; }
        public byte[] Poster { get; set; }
        
        public byte GenreId { get; set; }
        public Genre Genre { get; set; }
    }

}