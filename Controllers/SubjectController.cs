using _4DOT_RATT.DatabaseClasses;
using _4DOT_RATT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _4DOT_RATT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        SubjectDbManager db = new SubjectDbManager("Data Source=DatabaseFile/dot_API.db");
        [HttpGet(Name = "GetAllSubject")]
        public IEnumerable<Subject> Get()
        {
            return db.GetSubjects();
        }

        [HttpGet("{id}", Name = "GetSubject")]
        public ActionResult<Subject> Get(int id)
        {
            var subject = db.GetSubjectById(id);
            if (subject == null)
            {
                return NotFound();
            }
            return subject;
        }

        [HttpGet("SubjectList/{classId}", Name = "GetListSubjectClass")]
        public IEnumerable<object> GetListClassSchool(int classId)
        {
            return db.GetSubjectOfClass(classId);
        }

        [HttpPost(Name = "CreateSubject")]
        public IActionResult Create([FromBody] Subject subject)
        {
            if (ModelState.IsValid)
            {
                int newSubjectId = db.AddSubject(subject);
                subject.Id = newSubjectId;
                return CreatedAtRoute("GetSubject", new { id = newSubjectId }, subject);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}", Name = "UpdateSubject")]
        public IActionResult Put(int id, [FromBody] Subject subject)
        {
            if (subject == null || subject.Id != id)
            {
                return BadRequest();
            }

            var existingSubject = db.GetSubjectById(id);
            if (existingSubject == null)
            {
                return NotFound();
            }

            existingSubject.Name = subject.Name;
            existingSubject.MaxPoint = subject.MaxPoint;
            existingSubject.ClassID = subject.ClassID;

            db.UpdateSubject(existingSubject);

            return NoContent();
        }


        [HttpDelete("{id}", Name = "DeleteSubject")]
        public IActionResult Delete(int id)
        {
            if (db.DeleteSubject(id))
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
