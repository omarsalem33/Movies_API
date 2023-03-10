using MoviesAPI.Models;

namespace MoviesAPI.Services
{
    public class MoviesServices : IMoviesService
    {

        private readonly ApplicationDbContext _context;

        public MoviesServices(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Movie> Add(Movie movie)
        {
            await _context.AddAsync(movie);
            _context.SaveChanges();
            return movie;
        }

        public Movie Delete(Movie movie)
        {
            _context.Remove(movie);
            _context.SaveChanges();
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAll()
        {
            return await _context.Movies
                .Include(g => g.Genre)
                .OrderByDescending(r => r.Rate)
                .ToListAsync();
        }

        public async Task<Movie> GetById(int id)
        {
            return await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);
        }

        public Movie Update(Movie movie)
        {
            _context.Update(movie);
            _context.SaveChanges();
            return movie;
        }
    }
}
