using MySql.Data.MySqlClient;

namespace InventoryApp.DAL
{
    public static class DatabaseHelper
    {
        public static readonly string ConnectionString =
    "Server=localhost;Port=3306;Database=inventorydb;Uid=root;Pwd=;Charset=utf8mb4;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}