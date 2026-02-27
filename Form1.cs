using demo;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace demo
{
    public partial class Form1 : Form
    {
        // НИЖЕ СТРОКА ДЛЯ ПОДКЛЮЧЕНИЯ БД, КОТОРУЮ МЫ СОЗДАЛИ РАНЕЕ, НАЗВАНИЕ СЕРВЕРА ПОСМОТРИТЕ В СВОЙСТВАХ В SSMS 22(У МЕНЯ ТАКОЕ - WIN-PUURG92IVC5\SQLEXPRESS У ВАС МОЖЕТ НАЗЫВАТЬСЯ ИНАЧЕ!!!!)
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UP;Integrated Security=True;TrustServerCertificate=True";

        public Form1()
        {
            InitializeComponent();
            // ТУТ ШРИФТ ПО ЗАДАНИЮ, МОЖЕТЕ УБРАТЬ ЭТУ СТРОЧКУ КОДА И НАЗНАЧИТЬ В СВОЙСТВАЗ ФОРМЫ И Т.П.
            this.Font = new System.Drawing.Font("Times New Roman", 9);
        }

        // НИЖЕ КОД ДЛЯ Кнопки ВОЙТИ
        private void button1_Click(object sender, EventArgs e)
        {
            // Проверка на пустые поля
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Запрос - получаем роль и ФИО пользователя с БД
                    string query = "SELECT RoleId, Surname, Name, Patronymic FROM [User] WHERE Login = @log AND Password = @pass";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@log", textBox1.Text);
                    cmd.Parameters.AddWithValue("@pass", textBox2.Text);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int roleId = reader.GetInt32(0);
                        // Формируем ФИО
                        string fio = $"{reader.GetString(1)} {reader.GetString(2)} {reader.GetString(3)}";

                        MessageBox.Show("Авторизация прошла успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Переход на вторую форму, к ней приступим ниже в данной методичке
                        Form2 f = new Form2(roleId, fio);
                        f.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(3, "Гость");
            f.Show();
            this.Hide(); // Скрываем окно авторизации
        }
    }
}