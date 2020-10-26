using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginBd
{
    public partial class AdminFormGraphics : Form
    {
        private string clickNick = null;//ник в списке игроков, на который нажали
        private string clickItem = null;//предмет в списке пердметов, на который нажали
        private string clickChest = null;//сундук в списке сундуков, на который нажали
        private string infoItem = null;//предмет, информацию о котором смотрим
        private string infoChest = null;//сундук, информацию о котором смотрим

        private DataBaseInterface dataBaseInterface;
        private Timer timer20Sec;
        private Timer timer1Sec;
        private int secToUpdate = 0;
        private string page = "";

        public AdminFormGraphics(string _login, string _password)
        {
            InitializeComponent();

            HideAll();

            dataBaseInterface = new DataBaseInterface(_login, _password);

            pagePanel.Location = new System.Drawing.Point(200, 80);
            friendsPanel.Location = new System.Drawing.Point(200, 80);
            itemsPanel.Location = new System.Drawing.Point(200, 80);
            chestsPanel.Location = new System.Drawing.Point(200, 80);
            settingsPanel.Location = new System.Drawing.Point(200, 80);

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

            label1.Text = "Профиль " + dataBaseInterface.NowNick;

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

            itemRarityComboBox.Items.Add("Безделушка");
            itemRarityComboBox.Items.Add("Обычный");
            itemRarityComboBox.Items.Add("Редкий");
            itemRarityComboBox.Items.Add("Супер редкий");
            itemRarityComboBox.Items.Add("Уникальный");

            itemRarityComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            itemRarityComboBox.SelectedIndex = 1;
        }




        //////////////////////////////////////////////////////////////////
        ///             СОБЫТИЕ ТАЙМЕРА
        //////////////////////////////////////////////////////////////////

        private void Timer20Sec_Tick(object sender, EventArgs e)
        {

            HideAll();
            UpDate();

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

                    if (infoItem != null)
                    {
                        GetItemInfo(infoItem);
                    }
                }
                else if (page == "Сундуки")
                {
                    GetChests();

                    if(infoChest != null)
                    {
                        GetChestInfo(infoChest);
                    }
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

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            try
            {
                if (DataBaseInterface.BackUp(saveFileDialog1.FileName) == 0)
                {
                    MessageBox.Show("Не удалось сохранить!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MessageBox.Show("Сохранение прошло успешно", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void loadFromButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;

            try
            {
                if (DataBaseInterface.LoadFrom(openFileDialog1.FileName) == 0)
                {
                    MessageBox.Show("Не удалось восстановить!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MessageBox.Show("Восстановление прошло успешно! Вам потребуется войти заново.", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }


        ////////////////////Перемещение окна
        Point lastPoint;
        private void mainPanel_MouseMove(object sender, MouseEventArgs e)
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

        private void mainPanel_MouseDown(object sender, MouseEventArgs e)
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
        ///             ГЛАВНАЯ СТРАНИЦА
        //////////////////////////////////////////////////////////////////

        //Поиск игрока в списке игроков
        private void searchPageTextBox_TextChanged(object sender, EventArgs e)
        {
            searchPageListBox.Items.Clear();

            if (searchPageTextBox.Text == "")
            {
                GetPageInfo();
                return;
            }

            try
            {
                searchFriendsBox.Items.Clear();

                var list = dataBaseInterface.searchFriendsByNick(searchPageTextBox.Text);

                if (!(list is null))
                {
                    foreach (var item in list)
                    {
                        searchPageListBox.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Выбор игрока из списка игроков
        private void searchPageListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideAll();

            if (searchPageListBox.SelectedItem != null)
            {
                clickNick = searchPageListBox.SelectedItem.ToString();

                if (clickNick != dataBaseInterface.NowNick)
                {
                    editPlayerButton.Show();
                }
                else
                {
                    editPlayerButton.Hide();
                }
            }
        }

        //Выбрать игрока для редактирования
        private void editPlayerButton_Click(object sender, EventArgs e)
        {
            if (searchPageListBox.SelectedItem == null)
            {
                return;
            }

            HideAll();

            string _login = DataBaseInterface.GetLoginByNick(clickNick);

            if(_login is null)
            {
                return;
            }

            dataBaseInterface.NowLogin = (string)_login.Clone();

            pagePanel.Location = new System.Drawing.Point(200, 80);
            friendsPanel.Location = new System.Drawing.Point(200, 80);
            itemsPanel.Location = new System.Drawing.Point(200, 80);
            settingsPanel.Location = new System.Drawing.Point(200, 80);

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

            label1.Text = "Профиль " + dataBaseInterface.NowNick;
        }




        //////////////////////////////////////////////////////////////////
        ///             НАСТРОЙКИ
        //////////////////////////////////////////////////////////////////

        //Изменение логина
        private void changeLoginButton_Click(object sender, EventArgs e)
        {
            if(loginField.Text == "")
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
            if (!(creditField.Text is null || creditField.Text == "" || creditField.Text == "Добавьте карту"))
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

        //Изменение количества золота
        private void changeGoldButton_Click(object sender, EventArgs e)
        {
            if (goldField.Text == "")
            {
                MessageBox.Show("Поле золото должно быть заполнено!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetPageInfo();
                return;
            }

            int res;
            if (!int.TryParse(goldField.Text, out res))
            {
                MessageBox.Show("В поле золото можно вводить только цифры!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetPageInfo();
                return;
            }

            try
            {
                ChangeGold();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Изменение уровня
        private void changeLevelButton_Click(object sender, EventArgs e)
        {
            if (levelField.Text == "")
            {
                MessageBox.Show("Поле уровень должно быть заполнено!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetPageInfo();
                return;
            }

            int res;
            if (!int.TryParse(levelField.Text, out res))
            {
                MessageBox.Show("В поле уровень можно вводить только цифры!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GetPageInfo();
                return;
            }

            try
            {
                ChangeLevel();
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

            if(passwordFieldTwo.Text != passwordField.Text)
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

        //Изменение прав администратора
        private void adminCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                dataBaseInterface.ChangeRole(adminCheckBox.Checked);
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
            if (itemsListBox.SelectedItem != null)
            {
                HideAll();

                try
                {
                    clickItem = itemsListBox.SelectedItem.ToString();

                    GetItemInfo(itemsListBox.SelectedItem.ToString());
                    deleteItemButton.Show();
                    changeItemButton.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                allItemsListBox.ClearSelected();
            }
        }

        //Выбор предмета из списка всех предметов
        private void allItemsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allItemsListBox.SelectedItem != null)
            {
                try
                {
                    HideAll();

                    clickItem = allItemsListBox.SelectedItem.ToString();

                    GetItemInfo(allItemsListBox.SelectedItem.ToString());
                    if (!itemsListBox.Items.Contains(allItemsListBox.SelectedItem.ToString()))
                    {
                        addItemButton.Show();
                    }

                    changeItemButton.Show();
                    removeItemButton.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                itemsListBox.ClearSelected();
            }
        }

        //Удаление предмета у игрока
        private void deleteItemButton_Click(object sender, EventArgs e)
        {
            try
            {
                dataBaseInterface.DeleteItemFromPlayer(clickItem);

                itemsListBox.ClearSelected();
                HideAll();
                GetItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Изменение свойств предмета
        private void changeItemButton_Click(object sender, EventArgs e)
        {
            if(itemCostTextBox.Text == "" ||
                itemDamageTextBox.Text == "" ||
                itemAttackTextBox.Text == "" ||
                itemProtectionTextBox.Text == "" ||
                itemMagicTextBox.Text == "")
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(infoItem != itemNameTextBox.Text && allItemsListBox.Items.Contains(itemNameTextBox.Text))
            {
                MessageBox.Show("Такое название предмета уже есть!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string outString = dataBaseInterface.ChangeItemInfo(infoItem, itemNameTextBox.Text, itemCostTextBox.Text, itemDamageTextBox.Text,
                    itemAttackTextBox.Text, itemProtectionTextBox.Text, itemMagicTextBox.Text, itemRarityComboBox.Text);

                MessageBox.Show(outString, "Изменить", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                GetItems();
                GetItemInfo(itemNameTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Добавление предмета игроку
        private void addItemButton_Click(object sender, EventArgs e)
        {
            try
            {
                dataBaseInterface.AddItemToPlayer(clickItem);

                allItemsListBox.ClearSelected();
                HideAll();
                GetItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //
        private void createItemButton_Click(object sender, EventArgs e)
        {
            int i = 1;
            while(allItemsListBox.Items.Contains("Предмет" + i.ToString()))
            {
                ++i;
            }

            dataBaseInterface.CreateItem("Предмет" + i.ToString());

            GetItems();
        }

        private void removeItemButton_Click(object sender, EventArgs e)
        {
            HideAll();
            
            dataBaseInterface.RemoveItem(infoItem);
            infoItem = null;

            GetItems();
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
                    deleteChestButton.Show();
                    changeChestButton.Show();
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
                        addChestButton.Show();
                    }

                    changeChestButton.Show();
                    removeChestButton.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                chestsListBox.ClearSelected();
            }
        }

        //Удаление сундука у игрока
        private void deleteChestButton_Click(object sender, EventArgs e)
        {
            try
            {
                dataBaseInterface.DeleteChestFromPlayer(clickChest);

                chestsListBox.ClearSelected();
                HideAll();
                GetChests();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Изменение свойств сундука
        private void changeChestButton_Click(object sender, EventArgs e)
        {
            if (chestCostTextBox.Text == "" ||
                chestBaubleTextBox.Text == "" ||
                chestUsualTextBox.Text == "" ||
                chestRareTextBox.Text == "" ||
                chestSuperRareTextBox.Text == "" ||
                chestUniqueTextBox.Text == "")
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (infoChest != chestNameTextBox.Text && allChestsListBox.Items.Contains(chestNameTextBox.Text))
            {
                MessageBox.Show("Такое название сундука уже есть!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string outString = dataBaseInterface.ChangeChestInfo(infoChest, chestNameTextBox.Text, chestCostTextBox.Text, chestBaubleTextBox.Text,
                    chestUsualTextBox.Text, chestRareTextBox.Text, chestSuperRareTextBox.Text, chestUniqueTextBox.Text);

                MessageBox.Show(outString, "Изменить", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                GetChests();
                GetChestInfo(chestNameTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Добавление сундука игроку
        private void addChestButton_Click(object sender, EventArgs e)
        {
            try
            {
                dataBaseInterface.AddChestToPlayer(clickChest);

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

        //
        private void createChestButton_Click(object sender, EventArgs e)
        {
            int i = 1;
            while (allChestsListBox.Items.Contains("Сундук" + i.ToString()))
            {
                ++i;
            }

            dataBaseInterface.CreateChest("Сундук" + i.ToString());

            GetChests();
        }

        private void removeChestButton_Click(object sender, EventArgs e)
        {
            HideAll();

            dataBaseInterface.RemoveChest(infoChest);
            infoChest = null;

            GetChests();
        }








        //////////////////////////////////////////////////////////////////
        ///             МОИ ФУНКЦИИ
        //////////////////////////////////////////////////////////////////

        //Спрятать все элементы
        private void HideAll()
        {
            editPlayerButton.Hide();

            removeItemButton.Hide();
            removeChestButton.Hide();

            //
            presentButton.Hide();
            addFriendButton.Hide();
            addSearchFriendButton.Hide();

            //
            itemNameTextBox.Hide();
            itemCostLabel.Hide();
            itemDamageLabel.Hide();
            itemRarityLabel.Hide();
            itemAttackLabel.Hide();
            itemProtectionLabel.Hide();
            itemMagicLabel.Hide();

            itemCostTextBox.Hide();
            itemDamageTextBox.Hide();
            itemAttackTextBox.Hide();
            itemProtectionTextBox.Hide();
            itemMagicTextBox.Hide();
            itemRarityComboBox.Hide();

            deleteItemButton.Hide();
            changeItemButton.Hide();
            addItemButton.Hide();

            //
            helpLabel1.Hide();
            helpLabel2.Hide();

            chestNameTextBox.Hide();
            chestCostLabel.Hide();
            chestBaubleLabel.Hide();
            chestUsualLabel.Hide();
            chestRareLabel.Hide();
            chestSuperRareLabel.Hide();
            chestUniqueLabel.Hide();

            chestCostTextBox.Hide();
            chestBaubleTextBox.Hide();
            chestUsualTextBox.Hide();
            chestRareTextBox.Hide();
            chestSuperRareTextBox.Hide();
            chestUniqueTextBox.Hide();

            deleteChestButton.Hide();
            changeChestButton.Hide();
            addChestButton.Hide();
        }


        //Смена панелей из бокового меню
        private void PanelSetVisible(Panel panel)
        {
            pagePanel.Visible = false;
            friendsPanel.Visible = false;
            itemsPanel.Visible = false;
            chestsPanel.Visible = false;
            settingsPanel.Visible = false;

            panel.Visible = true;

            infoItem = null;
            infoChest = null;

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
            else if (outString == "Данные изменены!")
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
            else if (outString == "Данные изменены!")
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

        //Изменение количества золота
        private void ChangeGold()
        {
            string outString = dataBaseInterface.ChangeGold(goldField.Text);

            if (outString == "Не удалось изменить данные!")
            {
                MessageBox.Show(outString, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (outString == "Данные изменены!")
            {
                MessageBox.Show(outString, "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            GetPageInfo();
        }

        //Изменение уровня
        private void ChangeLevel()
        {
            string outString = dataBaseInterface.ChangeLevel(levelField.Text);

            if (outString == "Не удалось изменить данные!")
            {
                MessageBox.Show(outString, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (outString == "Данные изменены!")
            {
                MessageBox.Show(outString, "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            levelField.Text = pageInfoDictionary["Level"].ToString();
            countOfGoldLabel.Text = "Золото: " + pageInfoDictionary["CountOfGold"].ToString();
            goldField.Text = pageInfoDictionary["CountOfGold"].ToString();
            dateOfRegistrationLabel.Text = pageInfoDictionary["DateOfRegistration"];

            goldMainLabel.Text = pageInfoDictionary["CountOfGold"].ToString();

            searchPageListBox.Items.Clear();

            var playersList = dataBaseInterface.GetAllPlayers();
            foreach (var item in playersList)
            {
                searchPageListBox.Items.Add(item.Key);
            }

            if(creditField.Text == "Добавьте карту")
            {
                creditField.ForeColor = Color.Gray;
            }
            else
            {
                creditField.ForeColor = Color.Tomato;
            }

            if(pageInfoDictionary["Role"] == "admin")
            {
                adminCheckBox.Checked = true;
            }
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

            allItemsListBox.Items.Clear();
            itemsList = dataBaseInterface.GetAllItems();
            foreach (var item in itemsList)
            {
                allItemsListBox.Items.Add(item);
            }
        }

        //Получение информации о данном предмете
        private void GetItemInfo(string nameOfItem)
        {
            var dictionary = dataBaseInterface.GetItemInfo(nameOfItem);

            infoItem = (string)nameOfItem.Clone();
            itemNameTextBox.Text = nameOfItem;
            itemCostTextBox.Text = dictionary["Cost"];
            itemDamageTextBox.Text = dictionary["Damage"];
            itemAttackTextBox.Text = dictionary["AttackBonus"];
            itemProtectionTextBox.Text = dictionary["PtotectionBonus"];
            itemMagicTextBox.Text = dictionary["MagicBonus"];

            itemRarityComboBox.SelectedIndex = itemRarityComboBox.FindString(dictionary["Rarity"]);

            itemNameTextBox.Show();
            itemCostLabel.Show();
            itemDamageLabel.Show();
            itemRarityLabel.Show();
            itemAttackLabel.Show();
            itemProtectionLabel.Show();
            itemMagicLabel.Show();

            itemCostTextBox.Show();
            itemDamageTextBox.Show();
            itemRarityLabel.Show();
            itemAttackTextBox.Show();
            itemProtectionTextBox.Show();
            itemMagicTextBox.Show();
            itemRarityComboBox.Show();
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

            infoChest = (string)nameOfChest.Clone();
            chestNameTextBox.Text = nameOfChest;
            chestCostTextBox.Text = dictionary["Cost"];
            chestBaubleTextBox.Text = dictionary["Bauble"];
            chestUsualTextBox.Text = dictionary["Usual"];
            chestRareTextBox.Text = dictionary["Rare"];
            chestSuperRareTextBox.Text = dictionary["SuperRare"];
            chestUniqueTextBox.Text = dictionary["Unique"];

            helpLabel1.Show();
            helpLabel2.Show();

            chestNameTextBox.Show();
            chestCostLabel.Show();
            chestBaubleLabel.Show();
            chestUsualLabel.Show();
            chestRareLabel.Show();
            chestSuperRareLabel.Show();
            chestUniqueLabel.Show();

            chestCostTextBox.Show();
            chestBaubleTextBox.Show();
            chestUsualTextBox.Show();
            chestRareTextBox.Show();
            chestSuperRareTextBox.Show();
            chestUniqueTextBox.Show();
        }






        //////////////////////////////////////////////////////////////////
        ///             Ограничения на ввод
        //////////////////////////////////////////////////////////////////


        private void itemCostTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void itemDamageTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void itemCureTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void itemAttackTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void itemProtectionTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void itemMagicTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        
    }
}
