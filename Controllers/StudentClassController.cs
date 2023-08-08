using _4DOT_RATT.DatabaseClasses;
using _4DOT_RATT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _4DOT_RATT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentClassController : ControllerBase
    {
        StudentClassDbManager db = new StudentClassDbManager("Data Source=DatabaseFile/dot_API.db");
        [HttpGet(Name = "GetAllStudentClass")]
        public IEnumerable<StudentClass> Get()
        {
            return db.GetStudentClass();
        }

        [HttpGet("{id}", Name = "GetStudentClass")]
        public ActionResult<StudentClass> Get(int id)
        {
            var studentclass = db.GetStudentClassById(id);
            if (studentclass == null)
            {
                return NotFound();
            }
            return studentclass;
        }

        [HttpGet("StudentList/{classId}", Name = "GetListStudentClass")]
        public IEnumerable<object> GetListStudentClass(int classId)
        {
            return db.GetSudentOfClass(classId);
        }

        [HttpGet("NumberStudent/{schoolId}", Name = "GetNumberStudentClassOfSchool")]
        public IEnumerable<object> GetNumberStudentClassOfschool(int schoolId)
        {
            return db.GetNumberStudentByClassOfSchool(schoolId);
        }

        [HttpGet("CountStudentOfClass/{classId}", Name = "GetNumberStudentClass")]
        public IEnumerable<object> GetNumberStudentClass(int classId)
        {
            return db.GetNumberStudentByClass(classId);
        }

        [HttpPost(Name = "CreateStudentClass")]
        public IActionResult Create([FromBody] StudentClass studentclass)
        {
            if (ModelState.IsValid)
            {
                int value = db.ExistanceStudentYear(studentclass.StudentID, studentclass.Year);
                if (value == 0)
                {
                    int newStudentClassId = db.AddStudentClass(studentclass);
                    studentclass.Id = newStudentClassId;
                    return CreatedAtRoute("GetStudentClass", new { id = newStudentClassId }, studentclass);
                }
                else
                {
                    return BadRequest("This student has already been registered for this year");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}", Name = "UpdateStudentClass")]
        public IActionResult Put(int id, [FromBody] StudentClass studentclass)
        {
            if (studentclass == null || studentclass.Id != id)
            {
                return BadRequest();
            }

            var existingStudentClass = db.GetStudentClassById(id);
            if (existingStudentClass == null)
            {
                return NotFound();
            }

            existingStudentClass.ClassID = studentclass.ClassID;
            existingStudentClass.StudentID = studentclass.StudentID;
            existingStudentClass.Year = studentclass.Year;

            db.UpdateStudentClass(existingStudentClass);

            return NoContent();
        }


        [HttpDelete("{id}", Name = "DeleteStudentClasse")]
        public IActionResult Delete(int id)
        {
            if (db.DeleteStudentClass(id))
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
