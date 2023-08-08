using _4DOT_RATT.DatabaseClasses;
using _4DOT_RATT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _4DOT_RATT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseNoteController : ControllerBase
    {
        CourseNoteDbManager db = new CourseNoteDbManager("Data Source=DatabaseFile/dot_API.db");
        [HttpGet(Name = "GetAllCourseNote")]
        public IEnumerable<CourseNote> Get()
        {
            return db.GetAllCourseNote();
        }

        [HttpGet("{id}", Name = "GetCourseNote")]
        public ActionResult<CourseNote> Get(int id)
        {
            var coursenote = db.GetCourseNoteById(id);
            if (coursenote == null)
            {
                return NotFound();
            }
            return coursenote;
        }

        [HttpGet("GradesList/{subjectId}/{studentId}", Name = "GetListGradesStudentSubject")]
        public IEnumerable<object> GetListGradesStudentSubject(int subjectId, int studentId)
        {
            return db.GetGradeOfStudentSubject(subjectId, studentId);
        }

        [HttpGet("AllGradesList/{studentId}/{year}", Name = "GetAllGradesStudent")]
        public IEnumerable<object> GetAllGradesStudent(int studentId, int year)
        {
            return db.GetAllGradeOfStudent(studentId, year);
        }

        [HttpGet("AverageStudentsOfClass/{classId}/{schoolId}/{year}", Name = "GetListAvgStudent")]
        public IEnumerable<object> GetListAvgStudent(int classId, int schoolId, int year)
        {
            return db.GetAllAvgOfStudent(classId, schoolId, year);
        }

        [HttpGet("GetAverageStudents/{studentId}/{classId}/{schoolId}/{year}", Name = "GetAvgStudent")]
        public IEnumerable<object> GetAvgStudent(int studentId, int classId, int schoolId, int year)
        {
            return db.GetAvgOfStudent(studentId, classId, schoolId, year);
        }

        [HttpPost(Name = "CreateCourseNote")]
        public IActionResult Create([FromBody] CourseNote coursenote)
        {
            if (ModelState.IsValid)
            {
                int maxPoint = db.GetMaxPointSubjectById(coursenote.SubjectID);
                if (maxPoint >= coursenote.Mark)
                {
                    if (coursenote.Mark >= 0 && coursenote.Mark <= 20)
                    {
                        int newCourseNoteId = db.AddCourseNote(coursenote);
                        coursenote.Id = newCourseNoteId;
                        return CreatedAtRoute("GetCourseNote", new { id = newCourseNoteId }, coursenote);
                    }
                    else
                    {
                        return BadRequest("Invalid value. The grade is in the range of 0 to 20");
                    }
                }
                else
                {
                    return BadRequest("Invalid value. Grade greater than MaxPoint");
                }

            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}", Name = "UpdateCourseNote")]
        public IActionResult Put(int id, [FromBody] CourseNote coursenote)
        {
            if (coursenote == null || coursenote.Id != id)
            {
                return BadRequest();
            }

            var existingCourseNote = db.GetCourseNoteById(id);
            if (existingCourseNote == null)
            {
                return NotFound();
            }

            existingCourseNote.Mark = coursenote.Mark;
            existingCourseNote.SubjectID = coursenote.SubjectID;
            existingCourseNote.StudentID = coursenote.StudentID;

            db.UpdateCourseNote(existingCourseNote);

            return NoContent();
        }


        [HttpDelete("{id}", Name = "DeleteCourseNote")]
        public IActionResult Delete(int id)
        {
            if (db.DeleteCourseNote(id))
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
