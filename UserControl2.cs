using System;
using System.Windows.Forms;

namespace demo
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        // Этот метод мы будем вызывать из Form4 для каждой карточки
        public void Fill(int id, string status, string address, DateTime date, DateTime delivery)
        {
            // Используем те Name, которые соответсвуют элементам на скриншоте
            lblID.Text = "Артикул заказа: " + id.ToString();
            lblStatus.Text = "Статус заказа: " + status;
            lblAdress.Text = "Адрес пункта выдачи: " + address;
            lblDate.Text = "Дата заказа: " + date.ToShortDateString();

            // Дата доставки 
            lblDelivery.Text = "Дата доставки:\n" + delivery.ToShortDateString();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }
    }
}