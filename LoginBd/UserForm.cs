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

namespace LoginBd
{
    public partial class UserForm : Form
    {
        private string clickNick = null;//ник в списке игроков, на который нажали
        private string clickChest = null;//сундук в списке сундуков, на который нажали

        private int clickChestCost = 0;//стомость выбранного сундука
        private int clickChestBauble = 0;
        private int clickChestUsual = 0;
        private int clickChestRare = 0;
        private int clickChestSuperRare = 0;
        private int clickChestUnique = 0;

        private DataBaseInterface dataBaseInterface;
        private Timer timer20Sec;
        private Timer timer1Sec;
        private int secToUpdate = 0;
        private string page = "";

        //Конструктор окна
        public UserForm(string _login, string _password)
        {
            InitializeComponent();

            HideAll();

            dataBaseInterface = new DataBaseInterface(_login, _password);

            pagePanel.Location = new System.Drawing.Point(200, 80);
            friendsPanel.Location = new System.Drawing.Point(200, 80);
            itemsPanel.Location = new System.Drawing.Point(200, 80);
            chestsPanel.Location = new System.Drawing.Point(200, 80);
            settingsPanel.Location = new System.Drawing.Point(200, 80);
            topPanel.Location = new System.Drawing.Point(200, 80);

            PanelSetVisible(pagePanel);

            loginField.Text = (string)_login.Clone();

            passwordFieldTwo.Text = "Повторите пароль";
            passwordFieldTwo.ForeColor = Color.Gray;
            passwordFieldTwo.UseSystemPasswordChar = false;

            passwordField.Text = "Новый пароль";
            passwordField.ForeColor = Color.Gray;
            passwordField.UseSystemPasswordChar = false;

            creditField.Text = "Добавьте карту";
            creditField.ForeColor = Color.Gray;
            creditField.UseSystemPasswordChar = false;

            GetPageInfo();

            GetItems();

            GetFriends();

            GetTopPlayers();

            page = "Профиль";

            timer20Sec = new Timer();
            timer20Sec.Interval = 20000;
            timer20Sec.Tick += Timer20Sec_Tick;
            timer20Sec.Enabled = true;

            secToUpdate = 20;

            timer1Sec = new Timer();
            timer1Sec.Interval = 1000;
            timer1Sec.Tick += Timer1Sec_Tick;
            timer1Sec.Enabled = true;

            updateCheckBox.Checked = true;
        }





        //////////////////////////////////////////////////////////////////
        ///             СОБЫТИЕ ТАЙМЕРА
        //////////////////////////////////////////////////////////////////

        private void Timer20Sec_Tick(object sender, EventArgs e)
        {
            if (updateCheckBox.Checked)
            {
                UpDate();
                HideAll();
            }
            
            secToUpdate = 20;
        }

        private void Timer1Sec_Tick(object sender, EventArgs e)
        {
            secToUpdate--;

            updateLabel.Text = "До обновления: " + secToUpdate.ToString() + " сек";
        }

        private void UpDate()
        {
            try
            {
                GetPageInfo();

                if (page == "Друзья")
                {
                    GetFriends();
                }
                else if (page == "Предметы")
                {
                    GetItems();
                }
                else if (page == "Сундуки")
                {
                    GetChests();
                }
                else if (page == "Топ")
                {
                    GetTopPlayers();
                }
                else if (page == "Настройки")
                {
                    GetPageInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }







        //////////////////////////////////////////////////////////////////
        ///             СОБЫТИЯ ОКНА
        //////////////////////////////////////////////////////////////////

        private void updateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (updateCheckBox.Checked)
            {
                updateLabel.Visible = true;
                refreshButton.Visible = false;
            }
            else
            {
                updateLabel.Visible = false;
                refreshButton.Visible = true;
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            HideAll();
            UpDate();

            secToUpdate = 20;
        }

        //Выход из бд
        private void UserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dataBaseInterface.ChangeDateOfLastOnline();
            dataBaseInterface.SetPlayerOnline(false);
            StagingServerInterface.CloseConnection();
        }

        //Кнопка выхода
        private void exitLabel_Click(object sender, EventArgs e)
        {
            timer20Sec.Stop();
            timer1Sec.Stop();

            this.Close();
        }

        //Кнопка выхода
        private void exitButton_Click(object sender, EventArgs e)
        {
            timer20Sec.Stop();
            timer1Sec.Stop();

            this.Close();
        }


        //Кнопки меню
        private void myPageButton_Click(object sender, EventArgs e)
        {
            PanelSetVisible(pagePanel);
            page = "Профиль";

            try
            {
                GetPageInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void friendsButton_Click(object sender, EventArgs e)
        {
            PanelSetVisible(friendsPanel);
            page = "Друзья";

            try
            {
                GetFriends();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void itemsButton_Click(object sender, EventArgs e)
        {
            PanelSetVisible(itemsPanel);
            page = "Предметы";

            try
            {
                GetItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chestsButton_Click(object sender, EventArgs e)
        {
            PanelSetVisible(chestsPanel);
            page = "Сундуки";

            try
            {
                GetChests();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void topButton_Click(object sender, EventArgs e)
        {
            PanelSetVisible(topPanel);
            page = "Топ";

            try
            {
                GetTopPlayers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            PanelSetVisible(settingsPanel);
            page = "Настройки";

            try
            {
                GetPageInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        ////////////////////Подсказка passwordField
        private void passwordField_Enter(object sender, EventArgs e)
        {
            if (passwordFieldTwo.Text == "Повторите пароль")
            {
                passwordFieldTwo.Text = "";
                passwordFieldTwo.ForeColor = Color.Tomato;
                passwordFieldTwo.UseSystemPasswordChar = true;
            }
        }

        private void passwordField_Leave(object sender, EventArgs e)
        {
            if (passwordFieldTwo.Text == "")
            {
                passwordFieldTwo.Text = "Повторите пароль";
                passwordFieldTwo.ForeColor = Color.Gray;
                passwordFieldTwo.UseSystemPasswordChar = false;
            }
        }

        ////////////////////Подсказка passwordFieldNew
        private void passwordFieldNew_Enter(object sender, EventArgs e)
        {
            if (passwordField.Text == "Новый пароль")
            {
                passwordField.Text = "";
                passwordField.ForeColor = Color.Tomato;
                passwordField.UseSystemPasswordChar = true;
            }
        }

        private void passwordFieldNew_Leave(object sender, EventArgs e)
        {
            if (passwordField.Text == "")
            {
                passwordField.Text = "Новый пароль";
                passwordField.ForeColor = Color.Gray;
                passwordField.UseSystemPasswordChar = false;
            }
        }

        ////////////////////Подсказка creditField
        private void creditField_Enter(object sender, EventArgs e)
        {
            if (creditField.Text == "Добавьте карту")
            {
                creditField.Text = "";
                creditField.ForeColor = Color.Tomato;
                creditField.UseSystemPasswordChar = true;
            }
        }

        private void creditField_Leave(object sender, EventArgs e)
        {
            if (creditField.Text == "")
            {
                creditField.Text = "Добавьте карту";
                creditField.ForeColor = Color.Gray;
                creditField.UseSystemPasswordChar = false;
            }
        }





        //////////////////////////////////////////////////////////////////
        ///             НАСТРОЙКИ
        //////////////////////////////////////////////////////////////////

        //Изменение логина
        private void changeLoginButton_Click(object sender, EventArgs e)
        {
            if (loginField.Text == "")
            {
                MessageBox.Show("Поле логин должно быть заполнено!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetPageInfo();
                return;
            }

            Regex reg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (!reg.IsMatch(loginField.Text))
            {
                MessageBox.Show("В поле ЛОГИН требуется адрес электронной почты!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetPageInfo();
                return;
            }

            try
            {
                ChangeLogin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Изменение ника
        private void changeNickButton_Click(object sender, EventArgs e)
        {
            if (nicknameField.Text == "")
            {
                MessageBox.Show("Поле ник должно быть заполнено!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetPageInfo();
                return;
            }

            Regex reg = new Regex(@"^(?!.*\.\.)(?!\.)(?!.*\.$)(?!\d+$)[a-zA-Z0-9.]{5,50}$");
            if (!reg.IsMatch(nicknameField.Text))
            {
                MessageBox.Show("Ник не удовлетворяет требованиям!" + Environment.NewLine +
                    "- нельзя использовать только цифры" + Environment.NewLine + "" +
                    "- длина не менее 5 символов не более 50" + Environment.NewLine + "" +
                    "- нельзя точку в начале или конце, две точки подряд", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetPageInfo();
                return;
            }

            try
            {
                ChangeNick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Изменение данных кредитной карты
        private void changeCreditButton_Click(object sender, EventArgs e)
        {
            if(!(creditField.Text is null || creditField.Text == "" || creditField.Text == "Добавьте карту"))
            {
                Regex reg = new Regex(@"^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$");
                if (!reg.IsMatch(creditField.Text))
                {
                    MessageBox.Show("Введите верный номер карты!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GetPageInfo();
                    return;
                }
            }

            try
            {
                ChangeCredit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Изменение пароля
        private void changePasswordButton_Click(object sender, EventArgs e)
        {
            if (passwordFieldTwo.Text == "" || passwordField.Text == "")
            {
                MessageBox.Show("Оба поля для паролей должны быть заполнены!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetPageInfo();
                return;
            }

            if (passwordFieldTwo.Text != passwordField.Text)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetPageInfo();
                return;
            }

            try
            {
                ChangePassword();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }







        //////////////////////////////////////////////////////////////////
        ///             ДРУЗЬЯ
        //////////////////////////////////////////////////////////////////

        //Выбор друга из списка друзей
        private void friendsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideAll();

            if (friendsListBox.SelectedItem != null)
            {
                clickNick = friendsListBox.SelectedItem.ToString();

                if (dataBaseInterface.IsPresentForPlayer(friendsListBox.SelectedIndex))
                {
                    presentButton.Show();
                }

                maybeFriendsBox.ClearSelected();
                searchFriendsBox.ClearSelected();
            }
        }

        //Выбор подписчика из списка подписчиков
        private void maybeFriendsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideAll();

            if (maybeFriendsBox.SelectedItem != null)
            {
                clickNick = maybeFriendsBox.SelectedItem.ToString();

                addFriendButton.Show();

                friendsListBox.ClearSelected();
                searchFriendsBox.ClearSelected();
            }
        }

        //Выбор игрока из списка поиска
        private void searchFriendsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideAll();

            if (searchFriendsBox.SelectedItem != null)
            {
                clickNick = searchFriendsBox.SelectedItem.ToString();


                if (!friendsListBox.Items.Contains(clickNick) && !maybeFriendsBox.Items.Contains(clickNick)
                    && dataBaseInterface.NowNick != clickNick)
                {
                    addSearchFriendButton.Show();
                }

                friendsListBox.ClearSelected();
                maybeFriendsBox.ClearSelected();
            }
        }

        //Получение подарка
        private void presentButton_Click(object sender, EventArgs e)
        {
            try
            {
                presentButton.Hide();
                
                MessageBox.Show(dataBaseInterface.GetPresent(clickNick), "Получение подарка.", MessageBoxButtons.OK, MessageBoxIcon.Information);

                GetFriends();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Добавление в друзья (кнопка)
        private void addFriendButton_Click(object sender, EventArgs e)
        {
            try
            {
                addFriendButton.Hide();

                string outString = dataBaseInterface.AddFriend(clickNick);
                if (outString != "OK")
                {
                    MessageBox.Show(outString, "Добавление в друзья.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                GetFriends();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Добавление друга из поиска (кнопка)
        private void addSearchFriendButton_Click(object sender, EventArgs e)
        {
            try
            {
                addSearchFriendButton.Hide();

                string outString = dataBaseInterface.AddSearchFriend(clickNick);
                if (outString != "OK")
                {
                    MessageBox.Show(outString, "Добавление в друзья.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                GetFriends();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Поиск игроков по нику
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                searchFriendsBox.Items.Clear();

                var list = dataBaseInterface.searchFriendsByNick(searchTextBox.Text);

                if (!(list is null))
                {
                    foreach (var item in list)
                    {
                        searchFriendsBox.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }







        //////////////////////////////////////////////////////////////////
        ///             ПРЕДМЕТЫ
        //////////////////////////////////////////////////////////////////

        //Выбор предмета из списка предметов
        private void itemsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetItemInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        //////////////////////////////////////////////////////////////////
        ///             СУНДУКИ
        //////////////////////////////////////////////////////////////////

        //Выбор сундука из списка сундуков
        private void chestsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chestsListBox.SelectedItem != null)
            {
                HideAll();

                try
                {
                    clickChest = chestsListBox.SelectedItem.ToString();

                    GetChestInfo(chestsListBox.SelectedItem.ToString());
                    openChestButton.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                allChestsListBox.ClearSelected();
            }
        }

        //Выбор сундука из списка всех сундуков
        private void allChestsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allChestsListBox.SelectedItem != null)
            {
                try
                {
                    HideAll();

                    clickChest = allChestsListBox.SelectedItem.ToString();

                    GetChestInfo(allChestsListBox.SelectedItem.ToString());
                    if (!chestsListBox.Items.Contains(allChestsListBox.SelectedItem.ToString()))
                    {
                        if (int.Parse(goldMainLabel.Text) >= clickChestCost)
                        {
                            buyChestButton.ForeColor = Color.Tomato;
                        }
                        else
                        {
                            buyChestButton.ForeColor = Color.Gray;
                        }
                        buyChestButton.Show();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                chestsListBox.ClearSelected();
            }
        }

        //Удаление сундука у игрока
        private void openChestButton_Click(object sender, EventArgs e)
        {
            try
            {
                int allRarities = clickChestBauble + clickChestUsual + clickChestRare + clickChestSuperRare + clickChestUnique;
                if(allRarities == 0)
                {
                    MessageBox.Show("Хах! Пустой сундук", "Открытие сундука!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataBaseInterface.DeleteChestFromPlayer(clickChest);
                    chestsListBox.ClearSelected();
                    HideAll();
                    GetChests();
                    return;
                }

                GetItems();

                Random random = new Random();
                int value = random.Next(1, allRarities);
                string rarity = "";

                if(value >= clickChestBauble + clickChestUsual + clickChestRare + clickChestSuperRare)
                {
                    rarity = "Уникальный";
                }
                else if(value >= clickChestBauble + clickChestUsual + clickChestRare)
                {
                    rarity = "Супер редкий";
                }
                else if (value >= clickChestBauble + clickChestUsual)
                {
                    rarity = "Редкий";
                }
                else if (value >= clickChestBauble)
                {
                    rarity = "Обычный";
                }
                else
                {
                    rarity = "Безделушка";
                }

                string outString = dataBaseInterface.GetLootOfChest(rarity, clickChest, itemsListBox.Items);

                if(outString == "Не удалось получить предмет!")
                {
                    MessageBox.Show(outString, "Открытие сундука!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if(outString == "Все возможные предметы уже получены!")
                {
                    MessageBox.Show("Вы должны были получить предмет с редкостью <" + rarity  + 
                        ">, но все предметы такой редкости у вас уже есть!", "Открытие сундука!", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(outString, "Открытие сундука!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                chestsListBox.ClearSelected();
                HideAll();
                GetChests();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Добавление сундука игроку
        private void buyChestButton_Click(object sender, EventArgs e)
        {
            try
            {
                GetPageInfo();

                if (int.Parse(goldMainLabel.Text) >= clickChestCost)
                {
                    dataBaseInterface.AddChestToPlayer(clickChest);
                }

                allChestsListBox.ClearSelected();
                HideAll();
                GetChests();
                GetPageInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }











        //////////////////////////////////////////////////////////////////
        ///             МОИ ФУНКЦИИ
        //////////////////////////////////////////////////////////////////

        //Спрятать все элементы
        private void HideAll()
        {
            presentButton.Hide();
            addFriendButton.Hide();
            addSearchFriendButton.Hide();

            itemNameLabel.Hide();
            itemCostLabel.Hide();
            itemDamageLabel.Hide();
            itemRarityLabel.Hide();
            itemAttackLabel.Hide();
            itemProtectionLabel.Hide();
            itemMagicLabel.Hide();

            helpLabel1.Hide();
            helpLabel2.Hide();

            chestNameLabel.Hide();
            chestCostLabel.Hide();
            chestBaubleLabel.Hide();
            chestUsualLabel.Hide();
            chestRareLabel.Hide();
            chestSuperRareLabel.Hide();
            chestUniqueLabel.Hide();

            openChestButton.Hide();
            buyChestButton.Hide();
        }


        //Смена панелей из бокового меню
        private void PanelSetVisible(Panel panel)
        {
            pagePanel.Visible = false;
            friendsPanel.Visible = false;
            itemsPanel.Visible = false;
            chestsPanel.Visible = false;
            settingsPanel.Visible = false;
            topPanel.Visible = false;

            panel.Visible = true;

            HideAll();
        }


        //Изменение логина
        private void ChangeLogin()
        {
            string outString = dataBaseInterface.ChangeLogin(loginField.Text);

            if (outString == "Такой логин уже есть!" || outString == "Не удалось изменить данные!")
            {
                MessageBox.Show(outString, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loginField.Text = (string)dataBaseInterface.NowLogin.Clone();
            }
            else if(outString == "Данные изменены!")
            {
                MessageBox.Show(outString, "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Изменение ника
        private void ChangeNick()
        {
            string outString = dataBaseInterface.ChangeNick(nicknameField.Text);

            if (outString == "Такой ник уже есть!" || outString == "Не удалось изменить данные!")
            {
                MessageBox.Show(outString, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                nicknameField.Text = (string)dataBaseInterface.NowNick.Clone();
            }
            else if(outString == "Данные изменены!")
            {
                MessageBox.Show(outString, "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                nickNameLabel.Text = dataBaseInterface.NowNick;
            }
        }

        //Изменение кредитной карты
        private void ChangeCredit()
        {
            string outString = dataBaseInterface.ChangeCredit(creditField.Text);

            if (outString == "Не удалось изменить данные!")
            {
                MessageBox.Show(outString, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                nicknameField.Text = (string)dataBaseInterface.NowNick.Clone();
            }
            else if (outString == "Данные изменены!")
            {
                MessageBox.Show(outString, "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                nickNameLabel.Text = dataBaseInterface.NowNick;
            }

            GetPageInfo();
        }

        //Изменение пароля
        private void ChangePassword()
        {
            string outString = dataBaseInterface.ChangePassword(passwordField.Text);

            if (outString == "Не удалось изменить данные!")
            {
                MessageBox.Show(outString, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (outString == "Данные изменены!")
            {
                MessageBox.Show(outString, "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            passwordFieldTwo.Text = "Повторите пароль";
            passwordFieldTwo.ForeColor = Color.Gray;
            passwordFieldTwo.UseSystemPasswordChar = false;

            passwordField.Text = "Новый пароль";
            passwordField.ForeColor = Color.Gray;
            passwordField.UseSystemPasswordChar = false;
        }

        //Получение данных для главной страницы или настроек
        private void GetPageInfo()
        {
            var pageInfoDictionary = dataBaseInterface.GetPageInfo();
            nicknameField.Text = (string)pageInfoDictionary["Nickname"].Clone();
            nickNameLabel.Text = (string)pageInfoDictionary["Nickname"].Clone();
            creditField.Text = (string)pageInfoDictionary["CreditCard"].Clone();
            levelLabel.Text = "Уровень: " + pageInfoDictionary["Level"].ToString();
            countOfGoldLabel.Text = "Золото: " + pageInfoDictionary["CountOfGold"].ToString();
            dateOfRegistrationLabel.Text = pageInfoDictionary["DateOfRegistration"];

            goldMainLabel.Text = pageInfoDictionary["CountOfGold"].ToString();
        }

        //Получение списков друзей
        private void GetFriends()
        {
            friendsListBox.Items.Clear();
            maybeFriendsBox.Items.Clear();
            searchTextBox.Text = "";
            int countOfPresents = 0;
            var friendsDictionary = dataBaseInterface.GetFriends(out countOfPresents);
            foreach (var item in friendsDictionary["friends"])
            {
                friendsListBox.Items.Add(item.Key);
            }
            foreach (var item in friendsDictionary["maybeFriends"])
            {
                maybeFriendsBox.Items.Add(item.Key);
            }
            if (countOfPresents > 0)
            {
                myFriendsLabel.Text = "Мои друзья (" + countOfPresents.ToString() + ")";
            }
            else
            {
                myFriendsLabel.Text = "Мои друзья";
            }

            dataBaseInterface.GetAllPlayers();
        }

        //Получение списка предметов
        private void GetItems()
        {
            itemsListBox.Items.Clear();
            var itemsList = dataBaseInterface.GetItems();
            foreach (var item in itemsList)
            {
                itemsListBox.Items.Add(item);
            }
        }

        //Получение информации о данном предмете
        private void GetItemInfo()
        {
            var dictionary = dataBaseInterface.GetItemInfo(itemsListBox.SelectedItem.ToString());
            itemNameLabel.Text = itemsListBox.SelectedItem.ToString();

            itemCostLabel.Text = "Стоимость: " + dictionary["Cost"];
            itemDamageLabel.Text = "Урон: " + dictionary["Damage"];
            itemAttackLabel.Text = "Бонус к атаке: " + dictionary["AttackBonus"];
            itemProtectionLabel.Text = "Бонус к защите: " + dictionary["PtotectionBonus"];
            itemMagicLabel.Text = "Бонус к магии: " + dictionary["MagicBonus"];
            itemRarityLabel.Text = "Редкость: " + dictionary["Rarity"];


            itemNameLabel.Show();
            itemCostLabel.Show();
            itemDamageLabel.Show();
            itemRarityLabel.Show();
            itemAttackLabel.Show();
            itemProtectionLabel.Show();
            itemMagicLabel.Show();
        }

        //Получение списка сундуков
        private void GetChests()
        {
            chestsListBox.Items.Clear();
            var chestsList = dataBaseInterface.GetChests();
            foreach (var item in chestsList)
            {
                chestsListBox.Items.Add(item);
            }

            allChestsListBox.Items.Clear();
            chestsList = dataBaseInterface.GetAllChests();
            foreach (var item in chestsList)
            {
                allChestsListBox.Items.Add(item);
            }
        }

        //Получение информации о данном сундуке
        private void GetChestInfo(string nameOfChest)
        {
            var dictionary = dataBaseInterface.GetChestInfo(nameOfChest);
            chestNameLabel.Text = nameOfChest;

            double Bauble = int.Parse(dictionary["Bauble"]);
            double Usual = int.Parse(dictionary["Usual"]);
            double Rare = int.Parse(dictionary["Rare"]);
            double SuperRare = int.Parse(dictionary["SuperRare"]);
            double Unique = int.Parse(dictionary["Unique"]);

            double all = Bauble + Usual + Rare + SuperRare + Unique;

            if (all > 0)
            {
                chestCostLabel.Text = "Стоимость: " + dictionary["Cost"];
                chestBaubleLabel.Text = "Безделушка: " + ((Bauble / all) * 100).ToString("#.#") + "%";
                chestUsualLabel.Text = "Обычный: " + ((Usual / all) * 100).ToString("#.#") + "%";
                chestRareLabel.Text = "Редкий: " + ((Rare / all) * 100).ToString("#.#") + "%";
                chestSuperRareLabel.Text = "Супер редкий: " + ((SuperRare / all) * 100).ToString("#.#") + "%";
                chestUniqueLabel.Text = "Уникальный: " + ((Unique / all) * 100).ToString("#.#") + "%";
            }
            else
            {
                chestCostLabel.Text = "Стоимость: " + dictionary["Cost"];
                chestBaubleLabel.Text = "Безделушка: " + "0%";
                chestUsualLabel.Text = "Обычный: " + "0%";
                chestRareLabel.Text = "Редкий: " + "0%";
                chestSuperRareLabel.Text = "Супер редкий: " + "0%";
                chestUniqueLabel.Text = "Уникальный: " + "0%";
            }

            //chestCostLabel.Text = "Стоимость: " + dictionary["Cost"];
            //chestBaubleLabel.Text = "Безделушка: " + dictionary["Bauble"];
            //chestUsualLabel.Text = "Обычный: " + dictionary["Usual"];
            //chestRareLabel.Text = "Редкий: " + dictionary["Rare"];
            //chestSuperRareLabel.Text = "Супер редкий: " + dictionary["SuperRare"];
            //chestUniqueLabel.Text = "Уникальный: " + dictionary["Unique"];

            buyChestButton.Text = "Купить (" + dictionary["Cost"] + ")";
            clickChestCost = int.Parse(dictionary["Cost"]);

            clickChestBauble = int.Parse(dictionary["Bauble"]);
            clickChestUsual = int.Parse(dictionary["Usual"]);
            clickChestRare = int.Parse(dictionary["Rare"]);
            clickChestSuperRare = int.Parse(dictionary["SuperRare"]);
            clickChestUnique = int.Parse(dictionary["Unique"]);

            helpLabel1.Show();
            helpLabel2.Show();

            chestNameLabel.Show();
            chestCostLabel.Show();
            chestBaubleLabel.Show();
            chestUsualLabel.Show();
            chestRareLabel.Show();
            chestSuperRareLabel.Show();
            chestUniqueLabel.Show();
        }

        //Получение топа игроков
        private void GetTopPlayers()
        {
            topListBox.Items.Clear();
            var topPlayersList = dataBaseInterface.GetTopPlayers();
            foreach (var item in topPlayersList)
            {
                topListBox.Items.Add(item);
            }
        }

        
    }
}
