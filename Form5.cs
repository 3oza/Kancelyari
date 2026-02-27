using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace demo
{
    public partial class Form5 : Form
    {
        // строка подключения
        string podkl = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UP;Integrated Security=True;TrustServerCertificate=True";
        int? _id; // Сюда передается ID заказа для редактирования (или null для нового)

        public Form5(int? id)
        {
            InitializeComponent();
            _id = id;

            // Загружаем списки из БД сразу при открытии
            LoadLists();

            if (_id != null)
            {
                this.Text = "Редактирование заказа";
                LoadOrderData(); // Загружаем данные заказа, если это редактирование
                btnDelete.Visible = true;
            }
            else
            {
                this.Text = "Добавление заказа";
                btnDelete.Visible = false; // Удалять еще нечего
            }
        }

        // Загрузка статусов и пунктов выдачи в ComboBox из БД
        void LoadLists()
        {
            using (SqlConnection conn = new SqlConnection(podkl))
            {
                try
                {
                    conn.Open();
                    // Загрузка статусов из таблицы OrderStatus
                    SqlDataAdapter da1 = new SqlDataAdapter("SELECT Id, Name FROM OrderStatus", conn);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    cmbStatus.DataSource = dt1;
                    cmbStatus.DisplayMember = "Name";
                    cmbStatus.ValueMember = "Id";

                    // Загрузка пунктов выдачи из таблицы PickUpPoint
                    SqlDataAdapter da2 = new SqlDataAdapter("SELECT Id, City + ', ' + Street as Addr FROM PickUpPoint", conn);
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);
                    cmbPoint.DataSource = dt2;
                    cmbPoint.DisplayMember = "Addr";
                    cmbPoint.ValueMember = "Id";
                }
                catch (Exception ex) { MessageBox.Show("Ошибка загрузки списков: " + ex.Message); }
            }
        }

        // Подгрузка данных существующего заказа для полей формы
        void LoadOrderData()
        {
            using (SqlConnection conn = new SqlConnection(podkl))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM [Order] WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", _id);
                    SqlDataReader r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        cmbStatus.SelectedValue = r["StatusId"];
                        cmbPoint.SelectedValue = r["PickUpPointId"];
                        dtpOrder.Value = Convert.ToDateTime(r["CreationDate"]);
                        dtpDelivery.Value = Convert.ToDateTime(r["DeliveryDate"]);
                    }
                }
                catch (Exception ex) { MessageBox.Show("Ошибка загрузки данных заказа: " + ex.Message); }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(podkl))
            {
                try
                {
                    conn.Open();
                    string sql = (_id == null)
                        ? "INSERT INTO [Order] (CreationDate, DeliveryDate, PickUpPointId, StatusId) VALUES (@d1, @d2, @p, @s)"
                        : "UPDATE [Order] SET CreationDate=@d1, DeliveryDate=@d2, PickUpPointId=@p, StatusId=@s WHERE Id=@id";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (_id != null) cmd.Parameters.AddWithValue("@id", _id);
                    cmd.Parameters.AddWithValue("@d1", dtpOrder.Value);
                    cmd.Parameters.AddWithValue("@d2", dtpDelivery.Value);
                    cmd.Parameters.AddWithValue("@p", cmbPoint.SelectedValue);
                    cmd.Parameters.AddWithValue("@s", cmbStatus.SelectedValue);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Данные успешно сохранены!", "Успех");
                    this.DialogResult = DialogResult.OK; // Закрывает форму и возвращает сигнал для обновления списка в Form4
                }
                catch (Exception ex) { MessageBox.Show("Ошибка сохранения: " + ex.Message); }
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить заказ?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(podkl))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM [Order] WHERE Id = @id", conn);
                        cmd.Parameters.AddWithValue("@id", _id);
                        cmd.ExecuteNonQuery();
                        this.DialogResult = DialogResult.OK;
                    }
                    catch (Exception ex) { MessageBox.Show("Ошибка удаления: " + ex.Message); }
                }
            }
        }
    }
}