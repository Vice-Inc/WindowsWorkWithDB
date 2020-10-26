using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace LoginBd
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();          
        }

        ////////////////////Кнопка выхода
        private void exitLabel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        ////////////////////Перемещение окна
        Point lastPoint;
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        ////////////////////Вход
        private void enterButton_Click(object sender, EventArgs e)
        {
            Regex reg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (!reg.IsMatch(loginField.Text))
            {
                MessageBox.Show("В поле ЛОГИН требуется адрес электронной почты!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int ID = 0;
                string outString = DataBaseInterface.Enter(loginField.Text, passwordField.Text, out ID, adminCheckBox.Checked);

                if (outString == "Admin")
                {
                    this.Hide();

                    try
                    {
                        if (adminCheckBox.Checked)
                        {
                            var form = new AdminFormGraphics(loginField.Text, passwordField.Text);
                            passwordField.Text = "";
                            form.ShowDialog();
                        }
                        else
                        {
                            var form = new UserForm(loginField.Text, passwordField.Text);
                            passwordField.Text = "";
                            form.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        passwordField.Text = "";
                    }

                    this.Show();
                }
                else if (outString == "User")
                {
                    this.Hide();

                    try
                    {
                        var form = new UserForm(loginField.Text, passwordField.Text);
                        passwordField.Text = "";
                        form.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        passwordField.Text = "";
                    }

                    this.Show();
                }
                else
                {
                    MessageBox.Show(outString, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    passwordField.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                passwordField.Text = "";
            }
        }

        private void registationButton_Click(object sender, EventArgs e)
        {
            this.Hide();

            try
            {
                passwordField.Text = "";

                var form = new RegistrationForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Show();
        }

        private void restoreButton_Click(object sender, EventArgs e)
        {
            this.Hide();

            try
            {
                passwordField.Text = "";

                var form = new RestoreForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Show();
        }
    }
}
