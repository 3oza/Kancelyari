using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
namespace demo
{
    public partial class Producties : UserControl
    {
        public Producties()
        {
            InitializeComponent();
            this.Font = new Font("Times New Roman", 9);
        }
        public void FillData(string title, string desc, string prod, string prov, decimal price, int discount, string unit, int stock, string photo)
        {
            lblTitle.Text = title;
            lblDesc.Text = desc;
            lblProducer.Text = "Производитель: " + prod;
            lblProvider.Text = "Поставщик: " + prov;
            lblUnit.Text = "Ед. измерения: " + unit;
            lblStock.Text = "На складе: " + stock;
            lblDiscount.Text = discount > 0 ? $"{discount}%" : "";
            // ЛОГИКА ЦЕНЫ
            if (discount > 0)
            {
                decimal newPrice = price * (1 - (decimal)discount / 100);
                lblPrice.Text = $"Цена: {newPrice:N2} руб.";
                try
                {
                    Control[] found = this.Controls.Find("lblOldPrice", true);
                    if (found.Length > 0)
                    {
                        found[0].Text = price.ToString("N2");
                        found[0].Visible = true;
                        ((Label)found[0]).Font = new Font("Times New Roman", 9, FontStyle.Strikeout);
                        ((Label)found[0]).ForeColor = Color.Red;
                    }
                }
                catch { }
            }
            else
            {
                lblPrice.Text = $"Цена: {price:N2} руб.";
                try
                {
                    Control[] found = this.Controls.Find("lblOldPrice", true);
                    if (found.Length > 0) found[0].Visible = false;
                }
                catch { }
            }
            // ЗАГРУЗКА ФОТО (у меня в папке: bin/Debug, проверьте, чтобы у вас фотографиибыли по такому же пути)
            try
            {
                if (!string.IsNullOrEmpty(photo))
                {
                    // Соединяем путь Debug и путь из базы (Res/1.jpg)
                    string photoPath = Path.Combine(Application.StartupPath, photo.Trim());
                    if (File.Exists(photoPath))
                    {
                        pbPhoto.Image = Image.FromFile(photoPath);
                    }
                    else
                    {
                        // Если основного фото нет
                        string stub = Path.Combine(Application.StartupPath, "Res", "picture.png");
                        if (File.Exists(stub)) pbPhoto.Image = Image.FromFile(stub);
                        else pbPhoto.Image = null;
                    }
                }
            }
            catch { pbPhoto.Image = null; }
            pbPhoto.SizeMode = PictureBoxSizeMode.Zoom;
            // ЦВЕТ ФОНА ПО ТЗ
            if (stock == 0) this.BackColor = Color.LightBlue;
            else if (discount > 15) this.BackColor = ColorTranslator.FromHtml("#2E8B57");
            else this.BackColor = Color.White;
        }

        private void Producties_Load(object sender, EventArgs e)
        {

        }

        private void lblOldPrice_Click(object sender, EventArgs e)
        {

        }
    }
}