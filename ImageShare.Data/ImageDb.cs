using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ImageShare.Data
{
    public class ImageDb
    {
        private readonly string _connectionString;

        public ImageDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int AddImage(string fileName, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Images (FileName, Password, Views) " +
                              "VALUES (@filename, @password, 1)" +
                              "SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@filename", fileName);
            cmd.Parameters.AddWithValue("@password", password);
            connection.Open();
            int imageId = (int)(decimal)cmd.ExecuteScalar();
            return imageId;
        }
        public void UpdateViews(int imageId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Images SET Views = Views + 1 WHERE Id = @imageId";
            cmd.Parameters.AddWithValue("@imageId", imageId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public Image GetImage(int imageId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "select * from images where id=@id";
            cmd.Parameters.AddWithValue("@id", imageId);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            Image image = new Image
            {
                Id = (int)reader["Id"],
                FileName = (string)reader["FileName"],
                Password = (string)reader["Password"],
                Views = (int)reader["Views"]
            };
            return image;
        }
    }
}
