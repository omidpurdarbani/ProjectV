using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlCode
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString =
                "Data Source=.;Initial Catalog=excelsql;"
                + "Integrated Security=true;MultipleActiveResultSets=true;";

            // Provide the query string with a parameter placeholder.
            string queryString =
                "SELECT * from dbo.temp;";

            // Specify the parameter value.
            //int paramValue = 5;

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                //command.Parameters.AddWithValue("@pricePoint", paramValue);

                // Open the connection in a try/catch block.
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        var orderId = reader[0];
                        var routingCode = reader[1].ToString().Split(';');
                        var groupName = reader[2].ToString().Split(';');

                        for (int i = 0; i < routingCode.Count(); i++)
                        {

                            string routing = $"select id from routing where code = '{routingCode[i]}'";
                            SqlCommand routingCommand = new SqlCommand(routing, connection);
                            SqlDataReader routingDataReader = routingCommand.ExecuteReader();
                            routingDataReader.Read();
                            var routingId = routingDataReader[0].ToString();
                            routingDataReader.Close();


                            string group = $"select id from [productgroup] where unit = '{groupName[i]}'";
                            SqlCommand groupCommand = new SqlCommand(group, connection);
                            SqlDataReader groupDataReader = groupCommand.ExecuteReader();
                            groupDataReader.Read();
                            var groupId = groupDataReader[0].ToString();
                            groupDataReader.Close();



                            string insert = "insert into orderdetails values(@orderid,@routing_id,@group_id)";
                            SqlCommand insertCommand = new SqlCommand(insert, connection);
                            insertCommand.Parameters.AddWithValue("@orderid", orderId);
                            insertCommand.Parameters.AddWithValue("@routing_id", routingId);
                            insertCommand.Parameters.AddWithValue("@group_id", groupId);
                            insertCommand.ExecuteNonQuery();
                            Console.WriteLine( "Record Inserted : OrderId = "+ orderId + ", RoutingId = " + routingId + ", GroupId = " + groupId + "\n\n----------------------------------------------\n");
                        }

                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("done");

                Console.ReadLine();
            }
        }
    }
}
