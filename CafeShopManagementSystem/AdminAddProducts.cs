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
    public partial class AdminAddProducts : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""D:\FPTU Subject\PRN212\Project\CafeShopManagementSystem\Database\cafe.mdf"";Integrated Security=True;Connect Timeout=30");
        public AdminAddProducts()
        {
            InitializeComponent();

            displayData();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayData();
        }

        public bool emptyFields()
        {
            if(adminAddProducts_id.Text == "" || adminAddProducts_name.Text == ""
                || adminAddProducts_type.SelectedIndex == -1 || adminAddProducts_stock.Text == "" 
                || adminAddProducts_price.Text == "" || adminAddProducts_status.SelectedIndex == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public void displayData()
        {
            AdminAddProductsData prodData = new AdminAddProductsData();
            List<AdminAddProductsData> listData = prodData.productsListData();

            dataGridView1.DataSource = listData;
        }

        private const decimal MinStock = 1;
        private const decimal MinPrice = 1;

        private void adminAddProducts_addBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are required to be filled.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                decimal prodStock, prodPrice;

                // Validate the stock and price values
                if (!decimal.TryParse(adminAddProducts_stock.Text.Trim(), out prodStock) || prodStock < MinStock)
                {
                    MessageBox.Show("Product stock must be 1 or more.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(adminAddProducts_price.Text.Trim(), out prodPrice) || prodPrice < MinPrice)
                {
                    MessageBox.Show("Product price must be 1 or more.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();

                        // Check if the product id is already existing
                        string selectProdID = "SELECT * FROM products WHERE prod_id = @prodID";

                        using (SqlCommand selectPID = new SqlCommand(selectProdID, connect))
                        {
                            selectPID.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());

                            SqlDataAdapter adapter = new SqlDataAdapter(selectPID);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count >= 1)
                            {
                                MessageBox.Show("Product ID: " + adminAddProducts_id.Text.Trim() + " is taken already.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                string insertData = "INSERT INTO products (prod_id, prod_name, prod_type, prod_stock, prod_price," +
                                    " prod_status, prod_image, date_insert) VALUES (@prodID, @prodName, @prodType, @prodStock, " +
                                    "@prodPrice, @prodStatus, @prodImage, @dateInsert)";

                                DateTime today = DateTime.Today;

                                string path = Path.Combine(@"D:\FPTU Subject\PRN212\Project\CafeShopManagementSystem\CafeShopManagementSystem\Product_Directory\"
                                    + adminAddProducts_id.Text.Trim() + ".jpg");

                                string directoryPath = Path.GetDirectoryName(path);

                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }

                                File.Copy(adminAddProducts_imageView.ImageLocation, path, true);

                                using (SqlCommand cmd = new SqlCommand(insertData, connect))
                                {
                                    cmd.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());
                                    cmd.Parameters.AddWithValue("@prodName", adminAddProducts_name.Text.Trim());
                                    cmd.Parameters.AddWithValue("@prodType", adminAddProducts_type.Text.Trim());
                                    cmd.Parameters.AddWithValue("@prodStock", prodStock); // Use the validated stock
                                    cmd.Parameters.AddWithValue("@prodPrice", prodPrice); // Use the validated price
                                    cmd.Parameters.AddWithValue("@prodStatus", adminAddProducts_status.Text.Trim());
                                    cmd.Parameters.AddWithValue("@prodImage", path);
                                    cmd.Parameters.AddWithValue("@dateInsert", today);

                                    cmd.ExecuteNonQuery();
                                    clearFields();

                                    MessageBox.Show("Added successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    displayData();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed connection: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }


        private void adminAddProducts_importBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png";
                string imagePath = "";

                if(dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    adminAddProducts_imageView.ImageLocation = imagePath;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void clearFields()
        {
            adminAddProducts_id.Text = "";
            adminAddProducts_name.Text = "";
            adminAddProducts_type.SelectedIndex = -1;
            adminAddProducts_stock.Text = "";
            adminAddProducts_price.Text = "";
            adminAddProducts_status.SelectedIndex = -1;
            adminAddProducts_imageView.Image = null;

        }

        private void adminAddProducts_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                adminAddProducts_id.Text = row.Cells[1].Value.ToString();
                adminAddProducts_name.Text = row.Cells[2].Value.ToString();
                adminAddProducts_type.Text = row.Cells[3].Value.ToString();
                adminAddProducts_stock.Text = row.Cells[4].Value.ToString();
                adminAddProducts_price.Text = row.Cells[5].Value.ToString();
                adminAddProducts_status.Text = row.Cells[6].Value.ToString();

                string imagepath = row.Cells[7].Value.ToString();
                try
                {
                    if (imagepath != null)
                    {
                        adminAddProducts_imageView.Image = Image.FromFile(imagepath);
                    }
                    else
                    {
                        adminAddProducts_imageView.Image = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Image: " +ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void adminAddProducts_updateBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are required to be filled.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                decimal prodStock, prodPrice;

                // Validate the stock and price values
                if (!decimal.TryParse(adminAddProducts_stock.Text.Trim(), out prodStock) || prodStock < MinStock)
                {
                    MessageBox.Show("Product stock must be 1 or more.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(adminAddProducts_price.Text.Trim(), out prodPrice) || prodPrice < MinPrice)
                {
                    MessageBox.Show("Product price must be 1 or more.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult check = MessageBox.Show("Are you sure you want to Update Product ID: " + adminAddProducts_id.Text.Trim() + "?",
                    "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (check == DialogResult.Yes)
                {
                    if (connect.State != ConnectionState.Open)
                    {
                        try
                        {
                            connect.Open();

                            string updateData = "UPDATE products SET prod_name = @prodName, prod_type = @prodType, prod_stock = @prodStock," +
                                "prod_price = @prodPrice, prod_status = @prodStatus, date_update = @dateUpdate WHERE prod_id = @prodID";

                            DateTime today = DateTime.Today;

                            using (SqlCommand updateD = new SqlCommand(updateData, connect))
                            {
                                updateD.Parameters.AddWithValue("@prodName", adminAddProducts_name.Text.Trim());
                                updateD.Parameters.AddWithValue("@prodType", adminAddProducts_type.Text.Trim());
                                updateD.Parameters.AddWithValue("@prodStock", prodStock); // Use the validated stock
                                updateD.Parameters.AddWithValue("@prodPrice", prodPrice); // Use the validated price
                                updateD.Parameters.AddWithValue("@prodStatus", adminAddProducts_status.Text.Trim());
                                updateD.Parameters.AddWithValue("@dateUpdate", today);
                                updateD.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());

                                updateD.ExecuteNonQuery();
                                clearFields();

                                MessageBox.Show("Updated successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                displayData();
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

        private void adminAddProducts_deleteBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are required to be filled.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult check = MessageBox.Show("Are you sure you want to Delete Product ID: " + adminAddProducts_id.Text.Trim() + "?"
                    , "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (check == DialogResult.Yes)
                {
                    if (connect.State != ConnectionState.Open)
                    {
                        try
                        {
                            connect.Open();

                            string deleteData = "UPDATE products SET date_delete = @dateDelete WHERE prod_id = @prodID";

                            DateTime today = DateTime.Today;

                            using (SqlCommand deleteD = new SqlCommand(deleteData, connect))
                            { 
                                deleteD.Parameters.AddWithValue("@dateDelete", today);
                                deleteD.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());

                                deleteD.ExecuteNonQuery();
                                clearFields();

                                MessageBox.Show("Removed item successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                displayData();
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

        private void adminAddProducts_searchBtn_Click(object sender, EventArgs e)
        {
            // Get the search content from the textbox
            string searchContent = adminAddProducts_searchContent.Text.Trim();

            // If search box is empty, do nothing
            if (string.IsNullOrEmpty(searchContent))
            {
                MessageBox.Show("Please enter a search term.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Search query for products by name or type, excluding deleted products
            string query = "SELECT * FROM products WHERE (prod_name LIKE @search OR prod_type LIKE @search) AND date_delete IS NULL";

            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    // Add search parameter
                    cmd.Parameters.AddWithValue("@search", "%" + searchContent + "%");

                    // Execute query and load results into a DataTable
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = table;

                    adminAddProducts_searchContent.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed connection: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }

        private void adminAddProducts_refreshBtn_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM products WHERE date_delete IS NULL";

            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    // Execute query and load results into a DataTable
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = table;

                    adminAddProducts_searchContent.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed connection: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }
    }
}
