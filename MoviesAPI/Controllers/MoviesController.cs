using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Dtos;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _moviesService;
        private readonly IGenresServices _genresServices;


        private new List<string> _allowExtenstions = new List<string>{".jpg" , "png"};
        private long _maxAllowPosterSize = 1048576;

        public MoviesController(IMoviesService moviesService, IGenresServices genresServices)
        {
            _moviesService = moviesService;
            _genresServices = genresServices;
        }


        [HttpGet]
        public async Task <IActionResult> GettAllAsync()
        {
            var movies = await _moviesService.GetAll();
            //To Do map Moive to Dto 
            return Ok(movies);
        }
        [HttpGet ("{id}")] // must to send id   
        public async Task<IActionResult>GetByIdAsync(int id)
        {
            var movie = await _moviesService.GetById(id);
            if(movie == null)
                return NotFound();
            
            return Ok(movie);
        }

       // [HttpGet ("{genreId}")]
       /* public async Task<IActionResult> GetByGenreIdAsync(byte id)
        {
            var movies = await _context.Movies
                .Where(m => m.GenreId == id)
                .Include(m => m.Genre)
                .OrderByDescending(r => r.Rate)
                .ToListAsync();
            return Ok(movies);
        }*/

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto movieDto)
        {
            if (!_allowExtenstions.Contains(Path.GetExtension(movieDto.Poster.FileName).ToLower()))
                return BadRequest("Only .png and .jpg images are allowed!");
            if (movieDto.Poster.Length > _maxAllowPosterSize)
                return BadRequest("Max allowed size for size images is 1MB!");
            
            var IsValidGenre = await _genresServices.IsValidGenre(movieDto.GenreId);
            if (!IsValidGenre) return BadRequest("Invalid Genre ID!");
            
            using var datastreem = new MemoryStream();
            await movieDto.Poster.CopyToAsync(datastreem);
            var movie = new Movie
            {
                GenreId = movieDto.GenreId,
                Title= movieDto.Title,
                Poster = datastreem.ToArray(),
                Rate = movieDto.Rate,
                StoreLine = movieDto.StoreLine,
                Year= movieDto.Year
            };

            await _moviesService.Add(movie);
            return Ok(movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id ,[FromForm] MovieDto Dto)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null) 
                return NotFound("No movie was found with ID");

            var IsValidGenre = await _genresServices.IsValidGenre(Dto.GenreId);
            if (!IsValidGenre) 
                return BadRequest("Invalid Genre ID!");


            if (Dto.Poster != null)
            {
                if (!_allowExtenstions.Contains(Path.GetExtension(Dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png and .jpg images are allowed!");
                if (Dto.Poster.Length > _maxAllowPosterSize)
                    return BadRequest("Max allowed size for size images is 1MB!");
                using var datastreem = new MemoryStream();
                await Dto.Poster.CopyToAsync(datastreem);
                movie.Poster = datastreem.ToArray(); 
            }

            movie.Title = Dto.Title;
            movie.StoreLine = Dto.StoreLine;
            movie.Year = Dto.Year;
            movie.Rate = Dto.Rate;
            movie.GenreId = Dto.GenreId;
            
           _moviesService.Update(movie);

            return Ok (movie);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _moviesService.GetById(id);
            if (movie == null)
                return NotFound();
            _moviesService.Delete(movie);
            return Ok(movie);
        }


    }
}
