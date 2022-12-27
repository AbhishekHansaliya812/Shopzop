using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using Shopzop.Models;
using Shopzop.Common;
using NLog;
using System.Web.Mvc;

namespace Shopzop.DAL
{
    /* Data Access Layer to send new user's data to stored procedures */
    public class RegistrationDataAccessLayer
    {

        // Private readonly variables
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region SignUpUser
        public string SignUpUser(RegisrationModel model)
        {
            // Intialising object to encrypt password
            Password encryptPassword = new Password();
            // Creating SQL Server coneection
            SqlConnection connection = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Shopzop;Integrated Security=True");
            try
            {
                // Sending data to stored procedure
                SqlCommand command = new SqlCommand("proc_RegisterUser", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserName", model.UserName);
                command.Parameters.AddWithValue("@Mobile", model.Mobile);
                command.Parameters.AddWithValue("@Email", model.Email);
                command.Parameters.AddWithValue("@Password", encryptPassword.EncryptPassword(model.Password));

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                return ("Data Save Successfully");
            }
            catch (Exception ex)
            {
                // log the error message in log file
                logger.Error(ex, "Registration DAL Method");
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                return (ex.Message.ToString());
            }
        }
        #endregion
    }
}

