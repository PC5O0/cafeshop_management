using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CafeShopManagementSystem
{
    public partial class AdminAddUsers : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""D:\FPTU Subject\PRN212\Project\CafeShopManagementSystem\Database\cafe.mdf"";Integrated Security=True;Connect Timeout=30");
        public AdminAddUsers()
        {
            InitializeComponent();

            displayAddUsersData();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayAddUsersData();
        }

        public void displayAddUsersData()
        {
            AdminAddUsersData userData = new AdminAddUsersData();
            List<AdminAddUsersData> listData = userData.usersListData();

            dataGridView1.DataSource = listData;
        }

        public bool emptyFields()
        {
            if(adminAddUsers_username.Text == "" || adminAddUsers_password.Text == ""
                || adminAddUsers_role.Text =="" || adminAddUsers_status.Text == ""
                || adminAddUsers_imageView.Image == null)
            {
                return true; 
            }
            else
            {
                return false;
            }
            
        }

        private void adminAddUsers_addBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are required to be filed", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();

                        //check username if existing already
                        string selectUsers = "SELECT * FROM users WHERE username = @usern";

                        using(SqlCommand checkUsern = new SqlCommand(selectUsers,connect))
                        {
                            checkUsern.Parameters.AddWithValue("@usern", adminAddUsers_username.Text.Trim());

                            SqlDataAdapter adapter = new SqlDataAdapter(checkUsern);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if(table.Rows.Count >= 1)
                            {
                                string usern = adminAddUsers_username.Text.Substring(0, 1).ToUpper() + adminAddUsers_username.Text.Substring(1);
                                MessageBox.Show(usern +" is already taken", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                string insertData = "INSERT INTO users (username, password, profile_image, role, status, date_reg)" +
                                    "VALUES (@usern, @pass, @image, @role, @status, @date)";

                                DateTime today = DateTime.Today;

                                string path = Path.Combine(@"D:\FPTU Subject\PRN212\Project\CafeShopManagementSystem\CafeShopManagementSystem\User_Directory\"
                                    + adminAddUsers_username.Text.Trim() + ".jpg");

                                string directoryPath = Path.GetDirectoryName(path);

                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }

                                File.Copy(adminAddUsers_imageView.ImageLocation, path, true);

                                using(SqlCommand cmd  = new SqlCommand(insertData, connect))
                                {
                                    cmd.Parameters.AddWithValue("@usern", adminAddUsers_username.Text.Trim());
                                    cmd.Parameters.AddWithValue("@pass", adminAddUsers_password.Text.Trim());
                                    cmd.Parameters.AddWithValue("@image", path);
                                    cmd.Parameters.AddWithValue("@role", adminAddUsers_role.Text.Trim());
                                    cmd.Parameters.AddWithValue("@status", adminAddUsers_status.Text.Trim());
                                    cmd.Parameters.AddWithValue("@date", today);

                                    cmd.ExecuteNonQuery();
                                    clearFields();

                                    MessageBox.Show("Added successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    displayAddUsersData();
                                }
                            }

                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Connetion failed: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }

        private void adminAddUsers_importBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png";
                string imagePath = "";

                if(dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    adminAddUsers_imageView.ImageLocation = imagePath;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int id = 0;

        private string currentImagePath = "";
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            id = (int)row.Cells[0].Value;
            adminAddUsers_username.Text = row.Cells[1].Value.ToString();
            adminAddUsers_password.Text = row.Cells[2].Value.ToString();
            adminAddUsers_role.Text = row.Cells[3].Value.ToString();
            adminAddUsers_status.Text = row.Cells[4].Value.ToString();

            // Store the current image path in the class-level variable
            currentImagePath = row.Cells[5].Value.ToString();  // Assuming `row.Cells[5]` contains the current image path

            try
            {
                if (currentImagePath != null && currentImagePath != "")
                {
                    adminAddUsers_imageView.Image = Image.FromFile(currentImagePath);
                }
                else
                {
                    adminAddUsers_imageView.Image = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No Image: ", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void adminAddUsers_updateBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are required to be filled.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure you want to Update Username: " + adminAddUsers_username.Text.Trim()
                    + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (connect.State != ConnectionState.Open)
                    {
                        try
                        {
                            connect.Open();

                            string updateData = "UPDATE users SET username = @usern, password = @pass, role = @role, status = @status, profile_image = @image WHERE id = @id";

                            // Use the class-level variable 'currentImagePath' to store the current image
                            string imagePath = currentImagePath;  // Using the class-level variable

                            // If a new image is selected
                            if (adminAddUsers_imageView.ImageLocation != null && adminAddUsers_imageView.ImageLocation != imagePath)
                            {
                                // Define the new image path
                                string newImagePath = Path.Combine(@"D:\FPTU Subject\PRN212\Project\CafeShopManagementSystem\CafeShopManagementSystem\User_Directory\",
                                    adminAddUsers_username.Text.Trim() + ".jpg");

                                // Dispose of the current image to release the file lock
                                if (adminAddUsers_imageView.Image != null)
                                {
                                    adminAddUsers_imageView.Image.Dispose();
                                    adminAddUsers_imageView.Image = null;
                                }

                                // Ensure the directory exists
                                string directoryPath = Path.GetDirectoryName(newImagePath);
                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }

                                // Copy the new image file
                                File.Copy(adminAddUsers_imageView.ImageLocation, newImagePath, true);

                                // Set the image path to be saved in the database
                                imagePath = newImagePath; // Update the image path to the new one
                            }

                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@usern", adminAddUsers_username.Text.Trim());
                                cmd.Parameters.AddWithValue("@pass", adminAddUsers_password.Text.Trim());
                                cmd.Parameters.AddWithValue("@role", adminAddUsers_role.Text.Trim());
                                cmd.Parameters.AddWithValue("@status", adminAddUsers_status.Text.Trim());
                                cmd.Parameters.AddWithValue("@image", imagePath);  // Save the image path
                                cmd.Parameters.AddWithValue("@id", id);

                                cmd.ExecuteNonQuery();
                                clearFields();

                                MessageBox.Show("Updated successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                displayAddUsersData();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("You cannot change the user's image.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                }
            }
        }


        public void clearFields()
        {
            adminAddUsers_username.Text = "";
            adminAddUsers_password.Text = "";
            adminAddUsers_role.SelectedIndex = -1;
            adminAddUsers_status.SelectedIndex = -1;
            adminAddUsers_imageView.Image = null;
        }

        private void adminAddUsers_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void adminAddUsers_deleteBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fiels are required to be filled.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure you want to Delete Username: " + adminAddUsers_username.Text.Trim()
                    + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (connect.State != ConnectionState.Open)
                    {
                        try
                        {
                            connect.Open();

                            string deleteData = "DELETE FROM users WHERE id = @id";

                            using (SqlCommand cmd = new SqlCommand(deleteData, connect))
                            {
     
                                cmd.Parameters.AddWithValue("@id", id);

                                cmd.ExecuteNonQuery();
                                clearFields();

                                MessageBox.Show("Delete successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                displayAddUsersData();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Connection failed: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                }
            }
        }
    }
}
