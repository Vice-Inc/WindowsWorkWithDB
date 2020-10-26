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
    public partial class RegistrationForm : Form
    {
        private string registrationLogin = null;
        private string codeRegistrate = "";
        private string loginFieldEmpty = "Введите e-mail";

        public RegistrationForm()
        {
            InitializeComponent();

            checkButton.Hide();
            helpLabel1.Hide();
            helpLabel2.Hide();
            helpLabel3.Hide();

            loginField.Text = loginFieldEmpty;
            loginField.ForeColor = Color.Gray;

            passwordField.Text = "Введите пароль";
            passwordField.ForeColor = Color.Gray;
            passwordField.UseSystemPasswordChar = false;

            passwordTwoField.Text = "Повторите пароль";
            passwordTwoField.ForeColor = Color.Gray;
            passwordTwoField.UseSystemPasswordChar = false;

            nicknameField.Text = "Введите ник";
            nicknameField.ForeColor = Color.Gray;
        }

        ////////////////////Кнопка назад
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
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

        ////////////////////Подсказка nicknameField
        private void nicknameField_Enter(object sender, EventArgs e)
        {
            if (nicknameField.Text == "Введите ник")
            {
                nicknameField.Text = "";
                nicknameField.ForeColor = Color.Tomato;
            }
        }

        private void nicknameField_Leave(object sender, EventArgs e)
        {
            if (nicknameField.Text == "")
            {
                nicknameField.Text = "Введите ник";
                nicknameField.ForeColor = Color.Gray;
            }
        }

        //
        private void registationButton_Click(object sender, EventArgs e)
        {
            registrationLogin = (string)loginField.Text.Clone();

            if (loginField.Text == "Введите e-mail" || passwordField.Text == "Введите пароль" ||
               passwordTwoField.Text == "Повторите пароль" || nicknameField.Text == "Введите ник")
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Regex reg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (!reg.IsMatch(loginField.Text))
            {
                MessageBox.Show("В поле ЛОГИН требуется адрес электронной почты!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            reg = new Regex(@"^(?!.*\.\.)(?!\.)(?!.*\.$)(?!\d+$)[a-zA-Z0-9.]{5,50}$");
            if (!reg.IsMatch(nicknameField.Text))
            {
                MessageBox.Show("Ник не удовлетворяет требованиям!" + Environment.NewLine +
                    "- нельзя использовать только цифры" + Environment.NewLine + "" +
                    "- длина не менее 5 символов не более 50" + Environment.NewLine + "" +
                    "- нельзя точку в начале или конце, две точки подряд", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (passwordField.Text != passwordTwoField.Text)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string error = DataBaseInterface.CheckUser(loginField.Text, nicknameField.Text);

            if (error == "login")
            {
                MessageBox.Show("Такой логин уже есть!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (error == "nick")
            {
                MessageBox.Show("Такой ник уже есть!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Random random = new Random();
            codeRegistrate = random.Next(100000000, 999999999).ToString();

            sendEmail();
        }

        //
        private void checkButton_Click(object sender, EventArgs e)
        {
            if (loginField.Text == codeRegistrate)
            {
                Registrate();
                this.Close();
            }
            else
            {
                MessageBox.Show("Код не совпал!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                emailPage();
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
                MailAddress to = new MailAddress(loginField.Text);
                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = "Подтверждение регистрации";
                // текст письма
                m.Body = "<h2>Подтверждение электронного адреса</h2>\n" +
                    "<h4>Никому не сообщайте этот код!</h4>" +
                    "<h4>Код подтверждения: " + codeRegistrate + "</h4>";
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
                MessageBox.Show("Не удалось отправить письмо! Возможные причины:\n" + 
                    "- Нет соединения с интернетом." + Environment.NewLine +
                    "- Такого электронного адреса не существует.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Регистрация
        private void Registrate()
        {
            try
            {
                string outString = DataBaseInterface.Registation(registrationLogin, nicknameField.Text, passwordField.Text);

                if (outString == "Аккаунт создан!")
                {
                    MessageBox.Show(outString, "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(outString, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void emailPage()
        {
            pictureBox1.Show();
            pictureBox2.Show();
            pictureBox3.Show();
            pictureBox4.Show();

            loginFieldEmpty = "Введите e-mail";
            loginField.Show();
            nicknameField.Show();
            passwordField.Show();
            passwordTwoField.Show();

            cancelButton.Show();
            registationButton.Show();

            checkButton.Hide();
            helpLabel1.Hide();
            helpLabel2.Hide();
            helpLabel3.Hide();
        }


        private void codePage()
        {
            pictureBox1.Hide();
            pictureBox2.Hide();
            pictureBox3.Hide();
            pictureBox4.Hide();

            loginFieldEmpty = "Введите код";
            loginField.Text = "";
            nicknameField.Hide();
            passwordField.Hide();
            passwordTwoField.Hide();

            cancelButton.Hide();
            registationButton.Hide();

            checkButton.Show();
            helpLabel1.Show();
            helpLabel2.Show();
            helpLabel3.Show();
        }


    }
}
