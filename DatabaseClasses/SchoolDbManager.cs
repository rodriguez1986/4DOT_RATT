using _4DOT_RATT.Models;
using System.Data.SQLite;

namespace _4DOT_RATT.DatabaseClasses
{
    public class SchoolDbManager: DbManager
    {
        public SchoolDbManager(string connectionString) : base(connectionString) { }
        #region SCHOOLS
        public List<School> GetSchools()
        {
            List<School> listSchool = new List<School>();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM school", conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listSchool.Add(CreateSchoolObject(reader));
                        }
                    }
                }
            }

            return listSchool;

        }

        private School CreateSchoolObject(SQLiteDataReader reader)
        {
            return new School
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Adress = Convert.ToString(reader["adress"]),
                City = Convert.ToString(reader["city"]),
                ZipCode = Convert.ToString(reader["zipcode"]),
                Country = Convert.ToString(reader["country"]),
            };
        }

        public int AddSchool(School school)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("INSERT INTO School (name, adress, city, zipCode, country) VALUES (@Name, @Adress, @City, @ZipCode, @Country); SELECT last_insert_rowid();", conn))
                {
                    //command.Parameters.AddWithValue("@Id", school.Id);
                    command.Parameters.AddWithValue("@Name", school.Name);
                    command.Parameters.AddWithValue("@Adress", school.Adress);
                    command.Parameters.AddWithValue("@City", school.City);
                    command.Parameters.AddWithValue("@ZipCode", school.ZipCode);
                    command.Parameters.AddWithValue("@Country", school.Country);

                    var result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int newId))
                    {
                        return newId;
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve the new School ID.");
                    }
                }
            }
        }

        public School GetSchoolById(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("SELECT * FROM school WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return CreateSchoolObject(reader);
                        }
                    }
                }
            }

            return null;
        }

        public bool UpdateSchool(School school)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("UPDATE school SET name=@Name, adress=@Adress, city=@City, zipcode=@ZipCode, country=@Country WHERE id=@id", conn))
                {
                    command.Parameters.AddWithValue("@id", school.Id);
                    command.Parameters.AddWithValue("@Name", school.Name);
                    command.Parameters.AddWithValue("@Adress", school.Adress);
                    command.Parameters.AddWithValue("@City", school.City);
                    command.Parameters.AddWithValue("@ZipCode", school.ZipCode);
                    command.Parameters.AddWithValue("@Country", school.Country);

                    if (command.ExecuteNonQuery() >= 1) return true;
                    else return false;
                }
            }
        }

        public bool DeleteSchool(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var command = new SQLiteCommand("DELETE FROM school WHERE id=@id", conn))
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
