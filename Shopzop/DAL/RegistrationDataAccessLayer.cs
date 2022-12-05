using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using Shopzop.Models;
using Shopzop.Common;

namespace Shopzop.DAL
{
    public class RegistrationDataAccessLayer
    {
        public string SignUpUser(RegisrationModel model)
        {
            Password encryptPassword = new Password();
            //PasswordBase64 encryptPassword = new PasswordBase64();
            SqlConnection connection = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Shopzop;Integrated Security=True");
            try
            {
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
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                return (ex.Message.ToString());
            }
        }
    }
}