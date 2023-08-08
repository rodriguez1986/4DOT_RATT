using _4DOT_RATT.Models;
using System.Data.SQLite;

namespace _4DOT_RATT.DatabaseClasses
{
	public class StudentDbManager : DbManager
	{
		public StudentDbManager(string connectionString) : base(connectionString) { }

		#region STUDENTS
		public List<Student> GetStudents()
		{
			List<Student> listStudent = new List<Student>();
			using (SQLiteConnection conn = new SQLiteConnection(connectionString))
			{
				conn.Open();
				using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM student", conn))
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							listStudent.Add(CreateStudentObject(reader));
						}
					}
				}
			}

			return listStudent;

		}

		private Student CreateStudentObject(SQLiteDataReader reader)
		{
			return new Student
			{
				Id = Convert.ToInt32(reader["id"]),
				FirstName = Convert.ToString(reader["first_name"]),
				LastName = Convert.ToString(reader["last_name"]),
				Adress = Convert.ToString(reader["adress"]),
				City = Convert.ToString(reader["city"]),
				ZipCode = Convert.ToString(reader["zipCode"]),
				Country = Convert.ToString(reader["country"]),
			};
		}

		public int AddStudent(Student student)
		{
			using (var conn = new SQLiteConnection(connectionString))
			{
				conn.Open();

				using (var command = new SQLiteCommand("INSERT INTO Student (first_name, last_name, adress, city, zipCode, country) VALUES (@FirstName, @LastName, @Adress, @City, @ZipCode, @Country); SELECT last_insert_rowid();", conn))
				{
					//command.Parameters.AddWithValue("@Id", school.Id);
					command.Parameters.AddWithValue("@FirstName", student.FirstName);
					command.Parameters.AddWithValue("@LastName", student.LastName);
					command.Parameters.AddWithValue("@Adress", student.Adress);
					command.Parameters.AddWithValue("@City", student.City);
					command.Parameters.AddWithValue("@ZipCode", student.ZipCode);
					command.Parameters.AddWithValue("@Country", student.Country);

					var result = command.ExecuteScalar();
					if (result != null && int.TryParse(result.ToString(), out int newId))
					{
						return newId;
					}
					else
					{
						throw new Exception("Failed to retrieve the new Student ID.");
					}
				}
			}
		}

		public Student GetSudentById(int id)
		{
			using (var conn = new SQLiteConnection(connectionString))
			{
				conn.Open();

				using (var command = new SQLiteCommand("SELECT * FROM student WHERE id=@id", conn))
				{
					command.Parameters.AddWithValue("@id", id);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							return CreateStudentObject(reader);
						}
					}
				}
			}

			return null;
		}

		public Student GetStudentByFirstName(string firstname)
		{
			using (var conn = new SQLiteConnection(connectionString))
			{
				conn.Open();

				using (var command = new SQLiteCommand("SELECT * FROM student WHERE first_name like @firstname ", conn))
				{
					command.Parameters.AddWithValue("@firstname", firstname);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							return CreateStudentObject(reader);
						}
					}
				}
			}

			return null;
		}

		public bool UpdateStudent(Student student)
		{
			using (var conn = new SQLiteConnection(connectionString))
			{
				conn.Open();

				using (var command = new SQLiteCommand("UPDATE student SET first_name=@firstName, last_name=@lastName, adress=@Adress, city=@City, zipCode=@ZipCode, country=@Country WHERE id=@id", conn))
				{
					command.Parameters.AddWithValue("@id", student.Id);
					command.Parameters.AddWithValue("@firstName", student.FirstName);
					command.Parameters.AddWithValue("@lastName", student.LastName);
					command.Parameters.AddWithValue("@Adress", student.Adress);
					command.Parameters.AddWithValue("@City", student.City);
					command.Parameters.AddWithValue("@ZipCode", student.ZipCode);
					command.Parameters.AddWithValue("@Country", student.Country);

					if (command.ExecuteNonQuery() >= 1) return true;
					else return false;
				}
			}
		}

		public bool DeleteStudent(int id)
		{
			using (var conn = new SQLiteConnection(connectionString))
			{
				conn.Open();

				using (var command = new SQLiteCommand("DELETE FROM student WHERE id=@id", conn))
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
