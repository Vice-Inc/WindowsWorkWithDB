using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions;
using System.Net.Sockets;

namespace LoginBd
{
    public partial class RestoreForm : Form
    {
        private string restoreLogin = null;
        private string codeRestore = "Код";
        private string loginFieldEmpty = "Введите e-mail";

        public RestoreForm()
        {
            InitializeComponent();

            loginField.Text = loginFieldEmpty;
            loginField.ForeColor = Color.Gray;

            passwordField.Text = "Введите пароль";
            passwordField.ForeColor = Color.Gray;
            passwordField.UseSystemPasswordChar = false;

            passwordTwoField.Text = "Повторите пароль";
            passwordTwoField.ForeColor = Color.Gray;
            passwordTwoField.UseSystemPasswordChar = false;

            emailPage();
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
            if (e.Button == MouseButtons.Left)
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

        ////////////////////Подсказка loginField
        private void loginField_Enter(object sender, EventArgs e)
        {
            if (loginField.Text == loginFieldEmpty)
            {
                loginField.Text = "";
                loginField.ForeColor = Color.Tomato;
            }
        }

        private void loginField_Leave(object sender, EventArgs e)
        {
            if (loginField.Text == "")
            {
                loginField.Text = loginFieldEmpty;
                loginField.ForeColor = Color.Gray;
            }
        }

        ////////////////////Подсказка passwordField
        private void passwordField_Enter(object sender, EventArgs e)
        {
            if (passwordField.Text == "Введите пароль")
            {
                passwordField.Text = "";
                passwordField.ForeColor = Color.Tomato;
                passwordField.UseSystemPasswordChar = true;
            }
        }

        private void passwordField_Leave(object sender, EventArgs e)
        {
            if (passwordField.Text == "")
            {
                passwordField.Text = "Введите пароль";
                passwordField.ForeColor = Color.Gray;
                passwordField.UseSystemPasswordChar = false;
            }
        }

        ////////////////////Подсказка passwordTwoField
        private void passwordTwoField_Enter(object sender, EventArgs e)
        {
            if (passwordTwoField.Text == "Повторите пароль")
            {
                passwordTwoField.Text = "";
                passwordTwoField.ForeColor = Color.Tomato;
                passwordTwoField.UseSystemPasswordChar = true;
            }
        }

        private void passwordTwoField_Leave(object sender, EventArgs e)
        {
            if (passwordTwoField.Text == "")
            {
                passwordTwoField.Text = "Повторите пароль";
                passwordTwoField.ForeColor = Color.Gray;
                passwordTwoField.UseSystemPasswordChar = false;
            }
        }






        ////////////////////Кнопка отправить
        private void sendButton_Click(object sender, EventArgs e)
        {
            restoreLogin = (string)loginField.Text.Clone();

            sendButton.Hide();
            cancelButton.Hide();

            Regex reg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (!reg.IsMatch(restoreLogin))
            {
                loginField.Text = "";
                sendButton.Show();
                cancelButton.Show();
                MessageBox.Show("В поле ЛОГИН требуется адрес электронной почты!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(!DataBaseInterface.IsLoginExist(restoreLogin))
            {
                loginField.Text = "";
                sendButton.Show();
                cancelButton.Show();
                MessageBox.Show("Такого логина не существует!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Random random = new Random();
            codeRestore = random.Next(100000000, 999999999).ToString();

            sendEmail();
        }

        ////////////////////Кнопка назад
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        ////////////////////Кнопка Подтвердить
        private void checkButton_Click(object sender, EventArgs e)
        {
            if (loginField.Text == codeRestore)
            {
                passwordPage();
            }
            else
            {
                MessageBox.Show("Код не совпал!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                emailPage();
            }
        }

        ////////////////////Кнопка сохранить
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (passwordField.Text == "" || passwordTwoField.Text == "")
            {
                MessageBox.Show("Оба поля для паролей должны быть заполнены!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (passwordField.Text != passwordTwoField.Text)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                DataBaseInterface dataBaseInterface = new DataBaseInterface(restoreLogin, "pass");
                string outString = dataBaseInterface.ChangePassword(passwordField.Text);

                if (outString == "Не удалось изменить данные!")
                {
                    emailPage();
                    MessageBox.Show(outString, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (outString == "Данные изменены!")
                {
                    MessageBox.Show(outString, "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }










        //Отправка сообщения с кодом
        async private void sendEmail()
        {
            try
            {
                DataForMessage dataForMessage = new DataForMessage();
                dataForMessage.SetDataType(DataForMessage.DataType.Command);
                dataForMessage.@string = "SP_GetEmail";

                DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
                if (dataForResponse.datatype == DataForMessage.DataType.Error)
                {
                    MessageBox.Show("Сервер запретил отправку пиьсма.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }



                // отправитель - устанавливаем адрес и отображаемое в письме имя
                MailAddress from = new MailAddress("ViceInc@mail.ru", "Vice");
                // кому отправляем
                MailAddress to = new MailAddress(restoreLogin);
                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = "Восстановление";
                // текст письма
                m.Body = "<h2>Восстановление доступа к аккаунту</h2>\n" +
                    "<h4>Никому не сообщайте этот код!</h4>" +
                    "<h4>Код восстановления доступа: " + codeRestore + "</h4>";
                // письмо представляет код html
                m.IsBodyHtml = true;
                // адрес smtp-сервера и порт, с которого будем отправлять письмо
                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 2525);
                // логин и пароль
                smtp.Credentials = new System.Net.NetworkCredential("ViceInc@mail.ru", dataForResponse.@string);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(m);
                codePage();
            }
            catch (Exception ex)
            {
                loginField.Text = "";
                sendButton.Show();
                cancelButton.Show();
                MessageBox.Show("Не удалось отправить письмо! Возможные причины:\n" +
                    "- Нет соединения с интернетом." + Environment.NewLine +
                    "- Такого электронного адреса не существует.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Переход на страницу ввода адреса
        private void emailPage()
        {
            saveButton.Hide();
            checkButton.Hide();

            pictureBox2.Hide();
            pictureBox3.Hide();

            passwordField.Hide();
            passwordTwoField.Hide();

            helpLabel1.Hide();
            helpLabel2.Hide();
            helpLabel3.Hide();

            pictureBox1.Show();
            loginField.Show();
            sendButton.Show();
            cancelButton.Show();

            loginFieldEmpty = "Введите e-mail";

            if (loginField.Text != loginFieldEmpty)
            {
                loginField.Text = "";
            }
        }

        //Переход на страницу подтверждения кода
        private void codePage()
        {
            pictureBox1.Hide();
            restoreLogin = (string)loginField.Text.Clone();
            loginFieldEmpty = "Введите код";
            loginField.Text = "";

            checkButton.Show();
            pictureBox2.Show();

            helpLabel1.Show();
            helpLabel2.Show();
            helpLabel3.Show();
        }

        //Переход на страницу восстановления пароля
        private void passwordPage()
        {
            helpLabel1.Hide();
            helpLabel2.Hide();
            helpLabel3.Hide();

            checkButton.Hide();
            loginField.Hide();

            pictureBox2.Show();
            pictureBox3.Show();
            passwordField.Show();
            passwordTwoField.Show();

            saveButton.Show();
        }
    }
}
