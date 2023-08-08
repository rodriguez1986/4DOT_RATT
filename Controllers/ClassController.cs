using _4DOT_RATT.DatabaseClasses;
using _4DOT_RATT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _4DOT_RATT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        ClassDbManager db = new ClassDbManager("Data Source=DatabaseFile/dot_API.db");
        [HttpGet(Name = "GetAllClass")]
        //[Authorize]
        public IEnumerable<Class> Get()
        {
            return db.GetAllClass();
        }

        [HttpGet("{id}", Name = "GetClass")]
        public ActionResult<Class> Get(int id)
        {
            var clas = db.GetClassById(id);
            if (clas == null)
            {
                return NotFound();
            }
            return clas;
        }

        [HttpGet("School/{id}", Name = "GetSchoolIdByClassId")]
        public Dictionary<string, object> GetSchoolIdByClassId(int id)
        {
            var school_id = db.GetSchoolIDByClassID(id);
            return school_id;
        }

        [HttpGet("ClassList/{schoolId}", Name = "GetListClassSchool")]
        public IEnumerable<object> GetListClassSchool(int schoolId)
        {
            return db.GetClassOfSchool(schoolId);
        }

        [HttpPost(Name = "CreateClass")]
        public IActionResult Create([FromBody] Class clas)
        {
            if (ModelState.IsValid)
            {
                int newClassId = db.AddClass(clas);
                clas.Id = newClassId;
                return CreatedAtRoute("GetClass", new { id = newClassId }, clas);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}", Name = "UpdateClass")]
        public IActionResult Put(int id, [FromBody] Class clas)
        {
            if (clas == null || clas.Id != id)
            {
                return BadRequest();
            }

            var existingClass = db.GetClassById(id);
            if (existingClass == null)
            {
                return NotFound();
            }

            existingClass.Name = clas.Name;
            existingClass.SchoolID = clas.SchoolID;

            db.UpdateClass(existingClass);

            return NoContent();
        }


        [HttpDelete("{id}", Name = "DeleteClass")]
        public IActionResult Delete(int id)
        {
            if (db.DeleteClass(id))
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
