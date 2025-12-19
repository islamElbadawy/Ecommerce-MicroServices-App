using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace AdoTut
{


    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var connectionString = configuration.GetSection("constr").Value;

            var conn = new SqlConnection(connectionString);


            //var sql = "SELECT * FROM Wallets";
            var sqlInsert = "INSERT INTO Wallets (Holder, Balance) VALUES "
                            + $"(@Holder, @Balance);"
                            + $"SELECT CAST (scope_identity() AS int)";

            SqlParameter holderParam = new SqlParameter
            {
                ParameterName = "@Holder",
                SqlDbType = SqlDbType.VarChar,
                Direction = ParameterDirection.Input,
                Value = "Abdullah"
            };
            SqlParameter BalanceParam = new SqlParameter
            {
                ParameterName = "@Balance",
                SqlDbType = SqlDbType.Decimal,
                Direction = ParameterDirection.Input,
                Value = 40000
            };



            SqlCommand sqlCommand = new SqlCommand(sqlInsert, conn);

            sqlCommand.Parameters.Add(holderParam);
            sqlCommand.Parameters.Add(BalanceParam);

            sqlCommand.CommandType = CommandType.Text;

            conn.Open();

            int id = (int)sqlCommand.ExecuteScalar();

            if (id > 0)
            {
                Console.WriteLine($"Wallet of id {id} added successfully");
            }
            else
            {
                Console.WriteLine("Insert failed");
            }

            Wallet wallet = new PropertyBuilder<Wallet>()
                .WithProperty(w => w.Holder ,  "Ali")
                .WithProperty(w => w.Balance, 5000m)
                .Build();

            Console.WriteLine($"From Property Builder {wallet}");

            Product product = new PropertyBuilder<Product>()
                .WithProperty(p => p.Id, 1)
                .WithProperty(p => p.Name, "Laptop")
                .Build();

            Product product2 = new PropertyBuilder<Product>().WithProperty("Id", 12).Build();

            Console.WriteLine($"From Property Builder {product}");
            Console.WriteLine($"From Property Builder {product2}");

            //SqlDataReader reader = sqlCommand.ExecuteReader();


            //Wallet wallet;

            //while (reader.Read())
            //{
            //    wallet = new Wallet();
            //    wallet.Id = reader.GetInt32("Id");
            //    wallet.Holder = reader.GetString("Holder");
            //    wallet.Balance = reader.GetDecimal("Balance");
            //    Console.WriteLine(wallet);
            //}



            conn.Close();





            Console.ReadKey();
        }
    }
}