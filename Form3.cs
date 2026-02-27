using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace demo
{
    public partial class Form3 : Form
    {
        string cn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UP;Integrated Security=True;TrustServerCertificate=True";
        int? _id;
        string _img = "";

        public Form3(int? id)
        {
            InitializeComponent();
            _id = id;
            if (_id != null) LoadProductData();
            this.Text = _id == null ? "Добавление" : "Редактирование";
        }

        private void LoadProductData()
        {
            using (SqlConnection c = new SqlConnection(cn))
            {
                try
                {
                    c.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Product WHERE Id = @id", c);
                    cmd.Parameters.AddWithValue("@id", _id);
                    SqlDataReader r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        txtTitle.Text = r["Name"].ToString();
                        txtPrice.Text = r["Price"].ToString();
                        txtStock.Text = r["AmountInStock"].ToString();
                        txtDescription.Text = r["Description"].ToString();
                        txtDiscount.Text = r["Discount"].ToString();
                        _img = r["Photo"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(_img) && File.Exists(_img))
                            pbPhoto.Image = Image.FromFile(_img);
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void btnSelectPhoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _img = ofd.FileName;
                pbPhoto.Image = Image.FromFile(_img);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (decimal.Parse(txtPrice.Text) < 0 || int.Parse(txtStock.Text) < 0)
                {
                    MessageBox.Show("Отрицательные значения недопустимы!");
                    return;
                }

                using (SqlConnection c = new SqlConnection(cn))
                {
                    c.Open();
                    string sql = (_id == null)
                        ? "INSERT INTO Product (Name, Price, AmountInStock, Photo, Description, Discount) VALUES (@n, @p, @s, @ph, @d, @dsc)"
                        : "UPDATE Product SET Name=@n, Price=@p, AmountInStock=@s, Photo=@ph, Description=@d, Discount=@dsc WHERE Id=@id";

                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.AddWithValue("@n", txtTitle.Text);
                    cmd.Parameters.AddWithValue("@p", decimal.Parse(txtPrice.Text));
                    cmd.Parameters.AddWithValue("@s", int.Parse(txtStock.Text));
                    cmd.Parameters.AddWithValue("@d", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@dsc", string.IsNullOrEmpty(txtDiscount.Text) ? 0 : int.Parse(txtDiscount.Text));
                    cmd.Parameters.AddWithValue("@ph", _img);
                    if (_id != null) cmd.Parameters.AddWithValue("@id", _id);

                    cmd.ExecuteNonQuery();
                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex) { MessageBox.Show("Заполните все поля корректно! " + ex.Message); }
        }

        private void btnCancel_Click(object sender, EventArgs e) => this.Close();
    }
}