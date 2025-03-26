using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIisBEESinItaly.Data;
using APIisBEESinItaly.Models;
using Newtonsoft.Json;

namespace APIisBEESinItaly.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPets(int page = 1, int pageSize = 10)
        {
            var query = _context.Pets.AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pets = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Add("X-Total-Pages", totalPages.ToString());

            return Ok(pets);
        }

        /*
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPets(
        int page = 1,
        int pageSize = 10,
        string searchTerm = "",
        string species = "")
        {
            var query = _context.Pets.AsQueryable();

            // Búsqueda por nombre de mascota (si se pasa un término de búsqueda)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            // Paginación: saltamos los resultados anteriores (page-1) * pageSize
            var totalItems = await query.CountAsync();
            var pets = await query
                .Skip((page - 1) * pageSize) // Skip paginación
                .Take(pageSize) // Limitar los resultados
                .ToListAsync();

            // Retornar los resultados junto con los metadatos de la paginación
            var paginationMetadata = new
            {
                totalItems,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                currentPage = page,
                pageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            return Ok(pets);
        }*/

        [HttpPost("addmore")]
        public async Task<IActionResult> AddMorePets()
        {
            /* STORE PROCEDURE AddMorePets
            INSERT INTO [MyPetsDB].[dbo].[Pets] ([Name], [Age])
            SELECT CONCAT('Name', n), n
            FROM (SELECT TOP 5 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n 
                  FROM master.dbo.spt_values) AS numbers;
            */
            // Improve, adding sequencial from the last NameX

            await _context.Database.ExecuteSqlRawAsync("EXEC AddMorePets");
            return Ok(new { message = "Se añadieron más mascotas" });
        }


        // GET: api/pets
        /*   [HttpGet]
           public async Task<ActionResult<IEnumerable<Pet>>> Index()
           {
               return await _context.Pets.ToListAsync();
           }*/

        // GET: api/pets/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pet == null)
            {
                return NotFound();
            }

            return Ok(pet);
        }

        // POST: api/pets
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Pet pet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pet);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Details), new { id = pet.Id }, pet);
            }
            return BadRequest(ModelState);
        }

        // PUT: api/pets/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Pet pet)
        {
            if (id != pet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetExists(pet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/pets/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.Id == id);
        }
    }
}