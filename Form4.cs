using demo;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace demo
{
    public partial class Form4 : Form
    {
        string podkl = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UP;Integrated Security=True;TrustServerCertificate=True";
        int _role;

        public Form4(int role)
        {
            InitializeComponent();
            _role = role;
            LoadOrders();
        }
        public void LoadOrders()
        {
            using (SqlConnection conn = new SqlConnection(podkl))
            {
                try
                {
                    conn.Open();
                    string sql = @"SELECT o.Id, os.Name as StatusName, 
                         ISNULL(TRIM(p.City), '') + ', ' + ISNULL(TRIM(p.Street), '') as FullAddr, 
                         o.CreationDate, o.DeliveryDate 
                         FROM [Order] o
                         JOIN OrderStatus os ON o.StatusId = os.Id
                         JOIN PickUpPoint p ON o.PickUpPointId = p.Id";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader r = cmd.ExecuteReader();

                    while (r.Read())
                    {
                        // === Безопасное получение Id ===
                        int orderId;
                        if (r["Id"] == DBNull.Value)
                            orderId = 0;
                        else
                            orderId = Convert.ToInt32(r["Id"]);

                        // === Безопасное получение дат (через if-else для C# 7.3) ===
                        DateTime? creationDate;
                        if (r["CreationDate"] == DBNull.Value)
                            creationDate = null;
                        else
                            creationDate = Convert.ToDateTime(r["CreationDate"]);

                        DateTime? deliveryDate;
                        if (r["DeliveryDate"] == DBNull.Value)
                            deliveryDate = null;
                        else
                            deliveryDate = Convert.ToDateTime(r["DeliveryDate"]);

                        // === Создание карточки ===
                        UserControl1 card = new UserControl1();

                        // Подставляем DateTime.MinValue если дата null
                        card.Fill(
                            orderId,
                            r["StatusName"].ToString(),
                            r["FullAddr"].ToString(),
                            creationDate.HasValue ? creationDate.Value : DateTime.MinValue,
                            deliveryDate.HasValue ? deliveryDate.Value : DateTime.MinValue
                        );

                        card.Tag = orderId;

                        if (_role == 1)
                        {
                            // Используем локальную переменную orderId (замыкание)
                            card.Click += (s, e) => OpenEdit(orderId);
                            foreach (Control c in card.Controls)
                                c.Click += (s, e) => OpenEdit(orderId);
                        }
                        flowLayoutPanel1.Controls.Add(card);
                    }
                    r.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка БД: " + ex.Message);
                }
            }
        }
        void OpenEdit(int? orderId)
        {
            Form5 f5 = new Form5(orderId);
            if (f5.ShowDialog() == DialogResult.OK) LoadOrders();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenEdit(null);
        }
    }
}