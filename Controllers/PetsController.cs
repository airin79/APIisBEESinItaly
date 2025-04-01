using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIisBEESinItaly.Data;
using APIisBEESinItaly.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;

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

        [HttpPost("addmore/{numRecords}")]
        ///
        public async Task<IActionResult> AddMorePets(int numRecords)
        {
            if (numRecords <= 0)
            {
                return BadRequest("Number of rows must be > 0");
            }

            //await _context.Database.ExecuteSqlRawAsync("EXEC AddMorePets @NumRecords = ");
            /*await _context.Database.ExecuteSqlRawAsync("EXEC AddMorePets @NumRecords",
                new SqlParameter("@NumRecords", numRecords));
            */

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC AddMorePets @NumRecords",
                    new SqlParameter("@NumRecords", numRecords));

                return Ok(new { message = $"Successfully added {numRecords} records - refresh your page" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal Server Error: {ex.Message}" });
            }

            /* STORE PROCEDURE AddMorePets
            INSERT INTO [MyPetsDB].[dbo].[Pets] ([Name], [Age])
            SELECT CONCAT('Name', n), n
            FROM (SELECT TOP 5 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n 
                  FROM master.dbo.spt_values) AS numbers;
            */
            /* Improve, adding sequencial from the last NameX
            --Eliminar la stored procedure si existe
            IF OBJECT_ID('dbo.AddMorePets', 'P') IS NOT NULL
                DROP PROCEDURE dbo.AddMorePets
            GO

            -- Crear la nueva stored procedure
            CREATE PROCEDURE AddMorePets
            AS
            BEGIN
                DECLARE @LastId INT;

            --Obtener el último Id insertado en la tabla
            SELECT @LastId = ISNULL(MAX(Id), 0) FROM[dbo].[Pets];

            --Permitir la inserción manual en la columna ID(evitar el autoincremento de IDENTITY)
            SET IDENTITY_INSERT[dbo].[Pets] ON;

            --Insertar nuevos registros, comenzando desde el último Id +1
            INSERT INTO[dbo].[Pets] ([Id], [Name], [Age])
            SELECT @LastId +n, CONCAT('Name', @LastId + n), n
            FROM(
                SELECT TOP 5 ROW_NUMBER() OVER(ORDER BY(SELECT NULL)) AS n
                FROM master.dbo.spt_values
            ) AS numbers;

            --Restablecer IDENTITY_INSERT a OFF para dejar que SQL Server maneje el autoincremento del Id
            SET IDENTITY_INSERT[dbo].[Pets] OFF;
            */


        }

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