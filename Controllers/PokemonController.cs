using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController: Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons =  _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemons);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int id)
        {
            if(!_pokemonRepository.PokemonExists(id)) return NotFound(id);

            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(id));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemon);
        }

        [HttpGet("{id}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int id)
        {
            if(!_pokemonRepository.PokemonExists(id))
                return NotFound(id);
            
            var rating = _pokemonRepository.GetPokemonRating(id);
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromBody] PokemonDto incomingPokemon)
        {
            if(incomingPokemon == null)
                return BadRequest(ModelState);
            
            var exists = _pokemonRepository.GetPokemons()
                                .Where(p => p.Name.Trim().ToUpper().Equals(incomingPokemon.Name.Trim().ToUpper()))
                                .FirstOrDefault();
            
            if(exists != null)
            {
                ModelState.AddModelError("", "Pokemon already exists!");
                return StatusCode(422, ModelState);
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var pokemon = _mapper.Map<Pokemon>(incomingPokemon);
            if(!_pokemonRepository.CreatePokemon(pokemon))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Pokemon created successfully!");
        }
    }
}