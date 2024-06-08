using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController: Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Category))]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(categories);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int id)
        {
            if(!_categoryRepository.CategoryExists(id))
                return NotFound(id);
            
            var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(id));
            
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(category);
        }

        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByCategoryId(int categoryId)
        {
            if(!_categoryRepository.CategoryExists(categoryId))
                return NotFound(categoryId);
            
            var pokemons = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonByCategory(categoryId));
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            

            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto incomingCategory)
        {
            if(incomingCategory == null)
                return BadRequest(ModelState);
            
            var exists = _categoryRepository.GetCategories()
                            .Where(c => c.Name.Trim().ToUpper().Equals(incomingCategory.Name.Trim().ToUpper()))
                            .FirstOrDefault();
            
            if(exists != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            
            var category = _mapper.Map<Category>(incomingCategory);
            if(!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Category Created Successfully!");
        }

    }
}