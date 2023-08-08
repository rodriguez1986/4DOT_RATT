using _4DOT_RATT.DatabaseClasses;
using _4DOT_RATT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _4DOT_RATT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        SchoolDbManager db = new SchoolDbManager("Data Source=DatabaseFile/dot_API.db");
        [HttpGet(Name = "GetAllSchool")]
        public IEnumerable<School> Get()
        {
            return db.GetSchools();
        }

        [HttpGet("{id}", Name = "GetSchool")]
        public ActionResult<School> Get(int id)
        {
            var school = db.GetSchoolById(id);
            if (school == null)
            {
                return NotFound();
            }
            return school;
        }

        [HttpPost(Name = "CreateSchool")]
        public IActionResult Create([FromBody] School school)
        {
            if (ModelState.IsValid)
            {
                int newSchoolId = db.AddSchool(school);
                school.Id = newSchoolId;
                return CreatedAtRoute("GetSchool", new { id = newSchoolId }, school);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}", Name = "UpdateSchool")]
        public IActionResult Put(int id, [FromBody] School school)
        {
            if (school == null || school.Id != id)
            {
                return BadRequest();
            }

            var existingSchool = db.GetSchoolById(id);
            if (existingSchool == null)
            {
                return NotFound();
            }

            existingSchool.Name = school.Name;
            existingSchool.Adress = school.Adress;
            existingSchool.City = school.City;
            existingSchool.ZipCode = school.ZipCode;
            existingSchool.Country = school.Country;

            db.UpdateSchool(existingSchool);

            return NoContent();
        }


        [HttpDelete("{id}", Name = "DeleteSchool")]
        public IActionResult Delete(int id)
        {
            if (db.DeleteSchool(id))
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
