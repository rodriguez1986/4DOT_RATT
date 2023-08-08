using _4DOT_RATT.Models;
using System.Data.SQLite;

namespace _4DOT_RATT.DatabaseClasses
{
    public class ClassDbManager : DbManager
    {
        public ClassDbManager(string connectionString) : base(connectionString) { }
        #region CLASS
        public List<Class> GetAllClass()
        {
            List<Class> listClass = new List<Class>();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM class", conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listClass.Add(CreateClassObject(reader));
                        }
                    }
                }
            }

            return listClass;

        }

        private Class CreateClassObject(SQLiteDataReader reader)
        {
            return new Class
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                SchoolID = Convert.ToInt32(reader["school_id"]),
            };
        }

        public int AddClass(Class clas)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("INSERT INTO class (name, school_id) VALUES (@Name, @SchoolID); SELECT last_insert_rowid();", conn))
                {
                    //command.Parameters.AddWithValue("@Id", school.Id);
                    command.Parameters.AddWithValue("@Name", clas.Name);
                    command.Parameters.AddWithValue("@SchoolID", clas.SchoolID);

                    var result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int newId))
                    {
                        return newId;
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve the new Class ID.");
                    }
                }
            }
        }

        public Class GetClassById(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT * FROM class WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return CreateClassObject(reader);
                        }
                    }
                }
            }

            return null;
        }

        public Dictionary<string, object> GetSchoolIDByClassID(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT school_id as schoolID, s.name as school_name  FROM class c, school s WHERE c.school_id=s.id AND c.id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //int i = Convert.ToInt32(reader["school_id"]);
                            //return i;

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                object value = reader.GetValue(i);
                                result.Add(columnName, value);

                            }
                        }
                    }
                }
            }

            return result;
        }

        // Retrieve all class of a school
        public List<object> GetClassOfSchool(int schoolId)
        {
            List<object> listClass = new List<object>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT id, name FROM class WHERE school_id=@SchoolId", conn))
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

        public bool UpdateClass(Class clas)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("UPDATE class SET name=@Name, school_id=@SchoolID  WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", clas.Id);
                    command.Parameters.AddWithValue("@Name", clas.Name);
                    command.Parameters.AddWithValue("@SchoolID", clas.SchoolID);

                    if (command.ExecuteNonQuery() >= 1) return true;
                    else return false;
                }
            }
        }

        public bool DeleteClass(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("DELETE FROM class WHERE id=@id", conn))
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
