using demo;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace demo
{
    public partial class Form2 : Form
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UP;Integrated Security=True;TrustServerCertificate=True";

        public Form2(int role, string fio)
        {
            InitializeComponent();
            label1.Text = fio;
            btnAdd.Visible = btnEdit.Visible = btnDelete.Visible = (role == 1);
            txtSearch.TextChanged += (s, e) => LoadProducts();
            cmbSort.SelectedIndexChanged += (s, e) => LoadProducts();
            LoadProducts();
        }

        public void LoadProducts()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Tag = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string sql = @"SELECT p.*, pr.Name as prN, prov.Name as pvN 
                                   FROM Product p
                                   LEFT JOIN Producer pr ON p.ProducerId = pr.Id
                                   LEFT JOIN Provider prov ON p.ProviderId = prov.Id
                                   WHERE p.Name LIKE @s";

                    if (cmbSort.SelectedIndex == 1) sql += " ORDER BY AmountInStock ASC";
                    else if (cmbSort.SelectedIndex == 2) sql += " ORDER BY AmountInStock DESC";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@s", "%" + txtSearch.Text + "%");

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var card = new Producties();
                            card.FillData(
                                r["Name"].ToString(),
                                r["Description"].ToString(),
                                r["prN"]?.ToString() ?? "",
                                r["pvN"]?.ToString() ?? "",
                                r["Price"] != DBNull.Value ? Convert.ToDecimal(r["Price"]) : 0,
                                r["Discount"] != DBNull.Value ? Convert.ToInt32(r["Discount"]) : 0,
                                "шт.",
                                r["AmountInStock"] != DBNull.Value ? Convert.ToInt32(r["AmountInStock"]) : 0,
                                r["Photo"]?.ToString()
                            );

                            card.Tag = r["Id"];
                            card.Click += Card_Click;
                            foreach (Control child in card.Controls) child.Click += Card_Click;
                            flowLayoutPanel1.Controls.Add(card);
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void Card_Click(object sender, EventArgs e)
        {
            Control clicked = sender as Control;
            while (clicked != null && !(clicked is Producties)) clicked = clicked.Parent;

            if (clicked != null)
            {
                foreach (Control ct in flowLayoutPanel1.Controls) ct.BackColor = Color.White;
                clicked.BackColor = Color.LightGray;
                flowLayoutPanel1.Tag = clicked.Tag;
            }
        }

        private void OpenEdit(int? id)
        {
            if (Application.OpenForms["Form3"] != null) return;
            Form3 f = new Form3(id);
            if (f.ShowDialog() == DialogResult.OK) LoadProducts();
        }

        private void btnAdd_Click(object sender, EventArgs e) => OpenEdit(null);

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Tag is int id) OpenEdit(id);
            else MessageBox.Show("Выберите товар!");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!(flowLayoutPanel1.Tag is int id)) return;

            if (MessageBox.Show("Удалить?", "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        con.Open();
                        new SqlCommand($"DELETE FROM Product WHERE Id = {id}", con).ExecuteNonQuery();
                        LoadProducts();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            new Form1().Show();
            this.Close();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int role = 3; // По умолчанию гость

            if (label1.Text.Contains("Администратор")) role = 1;
            else if (label1.Text.Contains("Менеджер")) role = 2;

            // Открываем форму со списком заказов (Form4)
            Form4 f4 = new Form4(role);
            f4.Show();
        }
    }
}