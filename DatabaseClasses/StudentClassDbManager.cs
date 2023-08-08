using _4DOT_RATT.Models;
using System.Data.SQLite;

namespace _4DOT_RATT.DatabaseClasses
{
    public class StudentClassDbManager : DbManager
    {
        public StudentClassDbManager(string connectionString) : base(connectionString) { }
        #region SCHOOLS
        public List<StudentClass> GetStudentClass()
        {
            List<StudentClass> listStudentClass = new List<StudentClass>();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM studentclass", conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listStudentClass.Add(CreateStudentClassObject(reader));
                        }
                    }
                }
            }

            return listStudentClass;

        }

        private StudentClass CreateStudentClassObject(SQLiteDataReader reader)
        {
            return new StudentClass
            {
                Id = Convert.ToInt32(reader["id"]),
                ClassID = Convert.ToInt32(reader["class_id"]),
                StudentID = Convert.ToInt32(reader["student_id"]),
                Year = Convert.ToInt32(reader["year"]),
            };
        }

        public int AddStudentClass(StudentClass studentclass)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("INSERT INTO studentclass (class_id, student_id, year) VALUES (@ClassID, @StudentID, @Year); SELECT last_insert_rowid();", conn))
                {
                    //command.Parameters.AddWithValue("@Id", school.Id);
                    command.Parameters.AddWithValue("@ClassID", studentclass.ClassID);
                    command.Parameters.AddWithValue("@StudentID", studentclass.StudentID);
                    command.Parameters.AddWithValue("@Year", studentclass.Year);

                    var result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int newId))
                    {
                        return newId;
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve the new Student class ID.");
                    }
                }
            }
        }

        public StudentClass GetStudentClassById(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT * FROM studentclass WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return CreateStudentClassObject(reader);
                        }
                    }
                }
            }

            return null;
        }

        public int ExistanceStudentYear(int studentId, int year)
        {
            int value = -1;
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT * FROM studentclass WHERE student_id=@StudentID AND year=@Year", conn))
                {
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    command.Parameters.AddWithValue("@Year", year);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            value = 1;
                        }
                        else
                        {
                            value = 0;
                        }
                    }
                }
            }

            return value;
        }

        //Get students of a class
        public List<object> GetSudentOfClass(int schoolId)
        {
            List<object> listStudent = new List<object>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT first_name, last_name, adress FROM studentclass s, student e, class c  WHERE e.id=s.student_id AND c.id=s.class_id AND class_id=@ClassId", conn))
                {
                    command.Parameters.AddWithValue("@ClassId", schoolId);

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

                            listStudent.Add(result);

                        }
                    }
                }
            }

            return listStudent;
        }
        // Number students of a school by class
        public List<object> GetNumberStudentByClassOfSchool(int schoolId)
        {
            List<object> listClass = new List<object>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT count(student_id) as Total_students, c.name FROM studentclass s, class c, school e WHERE c.id=s.class_id AND c.school_id=e.id AND school_id=@SchoolId GROUP BY class_id", conn))
                {
                    command.Parameters.AddWithValue("@SchoolId", schoolId);

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

                            listClass.Add(result);

                        }
                    }
                }
            }

            return listClass;
        }

        // Number students by class
        public List<object> GetNumberStudentByClass(int classId)
        {
            List<object> listClass = new List<object>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT count(student_id) as Total_students FROM studentclass WHERE class_id=@ClassId GROUP BY class_id", conn))
                {
                    command.Parameters.AddWithValue("@ClassId", classId);

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

                            listClass.Add(result);

                        }
                    }
                }
            }

            return listClass;
        }
        public bool UpdateStudentClass(StudentClass studentclass)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("UPDATE studentclass SET class_id=@ClassID, student_id=@StudentID, year=@Year  WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", studentclass.Id);
                    command.Parameters.AddWithValue("@ClassID", studentclass.ClassID);
                    command.Parameters.AddWithValue("@StudentID", studentclass.StudentID);
                    command.Parameters.AddWithValue("@Year", studentclass.Year);

                    if (command.ExecuteNonQuery() >= 1) return true;
                    else return false;
                }
            }
        }

        public bool DeleteStudentClass(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("DELETE FROM studentclass WHERE id=@id", conn))
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
