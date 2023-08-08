using _4DOT_RATT.Models;
using System.Data.SQLite;

namespace _4DOT_RATT.DatabaseClasses
{
    public class CourseNoteDbManager : DbManager
    {
        public CourseNoteDbManager(string connectionString) : base(connectionString) { }
        #region COURSENOTE
        public List<CourseNote> GetAllCourseNote()
        {
            List<CourseNote> listCourseNote = new List<CourseNote>();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM coursenote", conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listCourseNote.Add(CreateCourseNoteObject(reader));
                        }
                    }
                }
            }

            return listCourseNote;

        }

        private CourseNote CreateCourseNoteObject(SQLiteDataReader reader)
        {
            return new CourseNote
            {
                Id = Convert.ToInt32(reader["id"]),
                Mark = Convert.ToDecimal(reader["mark"]),
                SubjectID = Convert.ToInt32(reader["subject_id"]),
                StudentID = Convert.ToInt32(reader["student_id"]),
            };
        }

        // Get MaxPoint of a subject
        public int GetMaxPointSubjectById(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT maxPoint FROM subject WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int maxPoint = Convert.ToInt32(reader["maxPoint"]);
                            return maxPoint;
                        }
                    }
                }
            }

            return -1;
        }

        public int AddCourseNote(CourseNote coursenote)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("INSERT INTO coursenote (mark, subject_id, student_id) VALUES (@Mark, @SubjectID, @StudentID); SELECT last_insert_rowid();", conn))
                {
                    //command.Parameters.AddWithValue("@Id", school.Id);
                    command.Parameters.AddWithValue("@Mark", coursenote.Mark);
                    command.Parameters.AddWithValue("@SubjectID", coursenote.SubjectID);
                    command.Parameters.AddWithValue("@StudentID", coursenote.StudentID);

                    var result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int newId))
                    {
                        return newId;
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve the new course note ID.");
                    }
                }
            }
        }

        public CourseNote GetCourseNoteById(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT * FROM coursenote WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return CreateCourseNoteObject(reader);
                        }
                    }
                }
            }

            return null;
        }

        //Get grades of a student for a subject
        public List<object> GetGradeOfStudentSubject(int subjectId, int studentId)
        {
            List<object> listGrades = new List<object>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT mark as Grade FROM coursenote WHERE subject_id = @SubjectID AND student_id = @StudentID", conn))
                {
                    command.Parameters.AddWithValue("@SubjectID", subjectId);
                    command.Parameters.AddWithValue("@StudentID", studentId);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> result = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                object value = reader.GetValue(i);

                                result.Add(columnName, value);
                            }

                            listGrades.Add(result);

                        }
                    }
                }
            }

            return listGrades;
        }

        //Retrieve All grades of a student
        public List<object> GetAllGradeOfStudent(int studentId, int year)
        {
            List<object> listGrades = new List<object>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT mark as Grade, s.name FROM coursenote c, subject s, class a, studentclass sc WHERE c.subject_id=s.id AND s.class_id=a.id AND sc.class_id=a.id AND sc.student_id = @StudentID AND year = @Year", conn))
                {
                    //command.Parameters.AddWithValue("@SubjectID", subjectId);
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    command.Parameters.AddWithValue("@Year", year);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> result = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                object value = reader.GetValue(i);

                                result.Add(columnName, value);
                            }

                            listGrades.Add(result);

                        }
                    }
                }
            }

            return listGrades;
        }

        // Retrieve Number of subjects programmed
        public int GetTotalSubject(int classId, int year)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT count(s.id) as sum FROM subject s, class c, studentclass sc WHERE s.class_id=c.id AND sc.class_id=c.id AND c.id = @ClassID AND year=@Year", conn))
                {
                    //command.Parameters.AddWithValue("@SubjectID", subjectId);
                    command.Parameters.AddWithValue("@ClassID", classId);
                    command.Parameters.AddWithValue("@Year", year);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int i = Convert.ToInt32(reader["sum"]);
                            return i;
                        }
                    }
                }
            }

            return 0;
        }

        //Retrieve averages students
        public List<object> GetAllAvgOfStudent(int classId, int schoolId, int year)
        {
            List<object> listGrades = new List<object>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT sum(mark) as average, e.last_name, e.first_name FROM coursenote c, student e, subject s, class a, school h, studentclass sc WHERE c.student_id=e.id AND c.subject_id=s.id AND s.class_id=a.id AND a.school_id=h.id AND sc.class_id=a.id AND s.class_id=@ClassID AND a.school_id=@SchoolID AND year=@Year GROUP BY c.student_id", conn))
                {
                    //command.Parameters.AddWithValue("@SubjectID", subjectId);
                    command.Parameters.AddWithValue("@ClassID", classId);
                    command.Parameters.AddWithValue("@SchoolID", schoolId);
                    command.Parameters.AddWithValue("@Year", year);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> result = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                if (columnName == "average")
                                {
                                    decimal average = Convert.ToDecimal(reader["average"]) / GetTotalSubject(classId, year);
                                    result.Add(columnName, Convert.ToDecimal(average.ToString("0.00")));
                                }
                                else
                                {
                                    object value = reader.GetValue(i);
                                    result.Add(columnName, value);
                                }

                            }

                            listGrades.Add(result);

                        }
                    }
                }
            }

            return listGrades;
        }

        //Retrieve averages students
        public List<object> GetAvgOfStudent(int studentId, int classId, int schoolId, int year)
        {
            List<object> listGrades = new List<object>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT sum(mark) as average, e.last_name, e.first_name FROM coursenote c, student e, subject s, class a, school h, studentclass sc WHERE c.student_id=e.id AND c.subject_id=s.id AND s.class_id=a.id AND a.school_id=h.id AND sc.class_id=a.id AND e.id=@StudentID AND s.class_id=@ClassID AND a.school_id=@SchoolID AND year=@Year GROUP BY c.student_id", conn))
                {
                    //command.Parameters.AddWithValue("@SubjectID", subjectId);
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    command.Parameters.AddWithValue("@ClassID", classId);
                    command.Parameters.AddWithValue("@SchoolID", schoolId);
                    command.Parameters.AddWithValue("@Year", year);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> result = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                if (columnName == "average")
                                {
                                    decimal average = Convert.ToDecimal(reader["average"]) / GetTotalSubject(classId, year);
                                    result.Add(columnName, Convert.ToDecimal(average.ToString("0.00")));
                                }
                                else
                                {
                                    object value = reader.GetValue(i);
                                    result.Add(columnName, value);
                                }

                            }

                            listGrades.Add(result);

                        }
                    }
                }
            }

            return listGrades;
        }

        public bool UpdateCourseNote(CourseNote coursenote)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("UPDATE coursenote SET mark=@Mark, subject_id=@SubjectID, student_id=@StudentID  WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", coursenote.Id);
                    command.Parameters.AddWithValue("@Mark", coursenote.Mark);
                    command.Parameters.AddWithValue("@SubjectID", coursenote.SubjectID);
                    command.Parameters.AddWithValue("@StudentID", coursenote.StudentID);

                    if (command.ExecuteNonQuery() >= 1) return true;
                    else return false;
                }
            }
        }

        public bool DeleteCourseNote(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("DELETE FROM coursenote WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", id);

                    if (command.ExecuteNonQuery() >= 1) return true;
                    else return false;
                }
            }
        }

        #endregion
    }
}
