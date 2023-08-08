using _4DOT_RATT.DatabaseClasses;
using _4DOT_RATT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _4DOT_RATT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        StudentDbManager db = new StudentDbManager("Data Source=DatabaseFile/dot_API.db");
        [HttpGet(Name = "GetAllStudent")]
        public IEnumerable<Student> Get()
        {
            return db.GetStudents();
        }

        [HttpGet("{id}", Name = "GetStudent")]
        public ActionResult<Student> Get(int id)
        {
            var student = db.GetSudentById(id);
            if (student == null)
            {
                return NotFound();
            }
            return student;
        }

        [HttpGet("get/{firstname}", Name = "GetStudentByFirstName")]
        public ActionResult<Student> GetStudentByFirstName(string firstname)
        {
            var student = db.GetStudentByFirstName(firstname);
            if (student == null)
            {
                return NotFound();
            }
            return student;
        }

        [HttpPost(Name = "CreateStudent")]
        public IActionResult Create([FromBody] Student student)
        {
            if (ModelState.IsValid)
            {
                int newStudentId = db.AddStudent(student);
                student.Id = newStudentId;
                return CreatedAtRoute("GetStudent", new { id = newStudentId }, student);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}", Name = "UpdateStudent")]
        public IActionResult Put(int id, [FromBody] Student student)
        {
            if (student == null || student.Id != id)
            {
                return BadRequest();
            }

            var existingStudent = db.GetSudentById(id);
            if (existingStudent == null)
            {
                return NotFound();
            }

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;
            existingStudent.Adress = student.Adress;
            existingStudent.City = student.City;
            existingStudent.ZipCode = student.ZipCode;
            existingStudent.Country = student.Country;

            db.UpdateStudent(existingStudent);

            return NoContent();
        }


        [HttpDelete("{id}", Name = "DeleteStudent")]
        public IActionResult Delete(int id)
        {
            if (db.DeleteStudent(id))
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
