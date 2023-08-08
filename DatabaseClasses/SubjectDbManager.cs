using _4DOT_RATT.Models;
using System.Data.SQLite;

namespace _4DOT_RATT.DatabaseClasses
{
    public class SubjectDbManager : DbManager
    {
        public SubjectDbManager(string connectionString) : base(connectionString) { }
        #region SUBJECT
        public List<Subject> GetSubjects()
        {
            List<Subject> listSubject = new List<Subject>();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM subject", conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listSubject.Add(CreateSubjectObject(reader));
                        }
                    }
                }
            }

            return listSubject;

        }

        private Subject CreateSubjectObject(SQLiteDataReader reader)
        {
            return new Subject
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                MaxPoint = Convert.ToInt32(reader["maxPoint"]),
                ClassID = Convert.ToInt32(reader["class_id"]),
            };
        }

        public int AddSubject(Subject subject)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("INSERT INTO subject (name,maxPoint, class_id) VALUES (@Name, @MaxPoint, @ClassID); SELECT last_insert_rowid();", conn))
                {
                    //command.Parameters.AddWithValue("@Id", school.Id);
                    command.Parameters.AddWithValue("@Name", subject.Name);
                    command.Parameters.AddWithValue("@MaxPoint", subject.MaxPoint);
                    command.Parameters.AddWithValue("@ClassID", subject.ClassID);

                    var result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int newId))
                    {
                        return newId;
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve the new Object ID.");
                    }
                }
            }
        }

        public Subject GetSubjectById(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT * FROM subject WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return CreateSubjectObject(reader);
                        }
                    }
                }
            }

            return null;
        }

        public List<object> GetSubjectOfClass(int schoolId)
        {
            List<object> listSubject = new List<object>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT id, name, maxPoint FROM subject WHERE class_id=@ClassId", conn))
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

                            listSubject.Add(result);

                        }
                    }
                }
            }

            return listSubject;
        }
        public bool UpdateSubject(Subject subject)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("UPDATE subject SET name=@Name, maxPoint=@MaxPoint, class_id=@ClassID  WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", subject.Id);
                    command.Parameters.AddWithValue("@Name", subject.Name);
                    command.Parameters.AddWithValue("@MaxPoint", subject.MaxPoint);
                    command.Parameters.AddWithValue("@ClassID", subject.ClassID);

                    if (command.ExecuteNonQuery() >= 1) return true;
                    else return false;
                }
            }
        }

        public bool DeleteSubject(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("DELETE FROM subject WHERE id=@id", conn))
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
