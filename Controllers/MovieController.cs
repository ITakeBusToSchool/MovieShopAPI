using Microsoft.AspNetCore.Mvc;
using MovieShop.Service;
using MovieShop.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShopAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MovieController:ControllerBase
    {
        private readonly IMovieService _movieService;
        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public async Task<IActionResult> GetTopGrossingMovies()
        {
            var movies = await _movieService.GetTopGrossingMovies();
            return Ok(movies);
        }
        /*public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenres();
            return Ok(genres);
        }*/
    }
}
