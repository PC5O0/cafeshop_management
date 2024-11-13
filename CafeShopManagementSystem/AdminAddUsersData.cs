﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CafeShopManagementSystem
{
    class AdminAddUsersData
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""D:\FPTU Subject\PRN212\Project\CafeShopManagementSystem\Database\cafe.mdf"";Integrated Security=True;Connect Timeout=30");
        public int ID {  get; set; }
        public string Username {  get; set; }
        public string Password { get; set; }    
        public string Role {  get; set; }
        public string  Status { get; set; }
        public string Image {  get; set; }
        public string DataRegistered { get; set; }

        public List<AdminAddUsersData> usersListData()
        {
            List<AdminAddUsersData> listData = new List<AdminAddUsersData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM users";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            AdminAddUsersData userData = new AdminAddUsersData();
                            userData.ID = (int)reader["id"];
                            userData.Username = reader["username"].ToString();
                            userData.Password = reader["password"].ToString();
                            userData.Role = reader["role"].ToString();
                            userData.Status = reader["status"].ToString();
                            userData.Image = reader["profile_image"].ToString();
                            userData.DataRegistered = reader["date_reg"].ToString();

                            listData.Add(userData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection Failed: " + ex);
                }
                finally
                {
                    connect.Close();
                }
            }
            return listData;
        }
    }
}