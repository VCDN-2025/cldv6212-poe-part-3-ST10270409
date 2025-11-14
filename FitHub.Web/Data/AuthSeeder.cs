using FitHub.Web.Security;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FitHub.Web.Data
{
    public class AuthSeeder
    {
        private readonly string _conn;
        public AuthSeeder(IConfiguration cfg) => _conn = cfg.GetConnectionString("DefaultConnection")!;

        public void EnsureUsers()
        {
            // Seed users
            UpsertUser("admin", "Admin@123!", "Admin");
            UpsertUser("customer1", "Cust@123!", "Customer");
            CreateCustomer("customer1", "Lebo", "Mokoena", "lebo@example.com", "0720000000");

            // Seed products for the shop/cart
            EnsureProducts();
        }

        private void UpsertUser(string username, string password, string role)
        {
            var (salt, hash) = Pbkdf2.Hash(password);

            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("dbo.UpsertUser", cn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@PasswordHash", hash);
            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
            cmd.Parameters.AddWithValue("@Role", role);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        private void CreateCustomer(string username, string first, string last, string email, string? phone)
        {
            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("dbo.CreateCustomerForUser", cn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@FirstName", first);
            cmd.Parameters.AddWithValue("@LastName", last);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Phone", (object?)phone ?? DBNull.Value);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        private void EnsureProducts()
        {
            const string sql = @"
IF NOT EXISTS (SELECT 1 FROM dbo.Products)
BEGIN
    INSERT INTO dbo.Products (Name, Price, ImageUrl) VALUES
    (N'Gym Membership - 1 Month', 299.00, NULL),
    (N'Personal Training Session', 450.00, NULL),
    (N'Yoga Class Pack (5)',      350.00, NULL),
    (N'Protein Shake Voucher',    80.00,  NULL);
END
";

            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, cn);
            cn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
