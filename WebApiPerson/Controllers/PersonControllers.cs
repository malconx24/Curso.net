using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPerson.Context;
using WebApiPerson.Models;


namespace WebApiPerson.Controllers
{

    [Route("Api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PersonController(AppDbContext context)
        {
            _context = context;
        }

        // Delegado para validación
         delegate bool PersonValidation(Person person);

        // Func para calcular edad
         Func<Person, int> calculateAge = person =>
            DateTime.Today.Year - person.BirthDate.Year -
            (DateTime.Today.DayOfYear < person.BirthDate.DayOfYear ? 1 : 0);

        // Action para notificación
        private Action<string> notify = msg => Console.WriteLine($"[INFO]: {msg}");

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? city = null, [FromQuery] int? minAge = null)
        {
            var query = _context.Persons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(p => p.City.ToLower() == city.ToLower());

            var persons = await query.ToListAsync();

            // Filtro por edad con LINQ y Func
            if (minAge.HasValue)
                persons = persons.Where(p => calculateAge(p) >= minAge.Value).ToList();

            var result = persons.Select(p => new
            {
                p.Id,
                p.FullName,
                p.City,
                p.BirthDate,
                Age = calculateAge(p)
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Person newPerson)
        {
            PersonValidation isValid = person =>
                !string.IsNullOrWhiteSpace(person.FullName) &&
                person.BirthDate < DateTime.Today;

            if (!isValid(newPerson))
                return BadRequest("Nombre requerido y fecha de nacimiento debe ser en el pasado.");

            _context.Persons.Add(newPerson);
            await _context.SaveChangesAsync();

            notify($"Persona '{newPerson.FullName}' registrada correctamente.");

            return CreatedAtAction(nameof(GetById), new { id = newPerson.Id }, newPerson);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null) return NotFound();

            return Ok(new
            {
                person.Id,
                person.FullName,
                person.BirthDate,
                person.City,
                Age = calculateAge(person)
            });
        }
    }
}

//        // GET: api/Person
//        [HttpGet]

//        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
//        {
//            return await _context.Persons.ToArrayAsync();
//        }

//        //GET: api/Person/5
//        [HttpGet("{id}")]

//        public async Task<ActionResult<Person>> GetPerson(int id)
//        {
//            var person = await _context.Persons.FindAsync(id);

//            if (person == null)
//            {
//                return NotFound();
//            }
//            return person;
//        }

//        //PUT: api/Person/5
//        [HttpPut("{id}")]

//        public async Task<IActionResult> PutPerson(int id, Person person)
//        {
//            if (id != person.Id)
//            {
//                return BadRequest();
//            }
//            _context.Entry(person).State = EntityState.Modified;
//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!PersonExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }
//            return NoContent();
//        }

//        //POST: api/Person
//        [HttpPost]

//        public async Task<ActionResult<Person>> PostPerson(Person person)
//        {
//            _context.Persons.Add(person);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetPerson", new { Id = person.Id }, person);
//        }

//        //DELETE: api/Person/5
//        [HttpDelete("{id}")]

//        public async Task<IActionResult> DeletePerson(int id)
//        {
//            var person = await _context.Persons.FindAsync(id);
//            if (person == null)
//            {
//                return NotFound();
//            }
//            _context.Persons.Remove(person);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool PersonExists(int id)
//        {
//            return _context.Persons.Any(x => x.Id == id);
//        }
//    }
//}
