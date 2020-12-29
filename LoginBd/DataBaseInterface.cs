using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Windows.Forms;

namespace LoginBd
{
    class DataBaseInterface
    {
        private static string senderLogin;//Логин отправителя сообщений серверу
        private static string senderPassword;//Пароль отправителя сообщений серверу

        private static string nowLogin;//Логин до замены
        private static string nowNick;//Ник до замены
        private static int ID;//должен быть не изменен
        private Dictionary<string, int> dictionaryOfPlayerId = new Dictionary<string, int>();//индексы игроков(кроме поиска)
        private List<int> listOfPresentsId = new List<int>();//индексы в списке друзей, за которые положен подарок

        enum Role { Failed, Admin, User, Online, NetworkFailed, Error };

        public DataBaseInterface(string login, string password)
        {
            nowLogin = (string)login.Clone();

            senderLogin = (string)login.Clone();
            senderPassword = (string)password.Clone();
        }

        public string NowLogin
        {
            get => nowLogin;
            set => nowLogin = value;
        }

        public string NowNick
        {
            get => nowNick;
        }
        

        //////////////////////////////////////////////////////////////////
        ///             ВХОД
        //////////////////////////////////////////////////////////////////

        //Получение роли по логину и паролю
        private static Role GetRole(string login, string password, out int ID, bool adminChecked)
        {
            ID = 0;

            if(login is null || password is null)
            {
                return Role.Failed;
            }

            try
            {
                Role role = Role.Failed;
                DataForMessage dataForMessage = new DataForMessage();
                dataForMessage.SetDataType(DataForMessage.DataType.Command);
                dataForMessage.@string = "SP_GetRole";
                dataForMessage.dictionaryStringString.Add("@login", login);
                dataForMessage.dictionaryStringString.Add("@password", GetHash(password));

                bool roleBool = false;

                DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
                if (dataForResponse.datatype == DataForMessage.DataType.Error)
                {
                    return Role.NetworkFailed;
                }

                if (!dataForResponse.dictionaryStringString.ContainsKey("Role"))
                {
                    return Role.Failed;
                }

                if (!dataForResponse.dictionaryStringString.ContainsKey("Id"))
                {
                    return Role.Failed;
                }

                roleBool = bool.Parse(dataForResponse.dictionaryStringString["Role"]);
                ID = int.Parse(dataForResponse.dictionaryStringString["Id"]);

                //есть права админа
                if (roleBool)
                {
                    //входит как админ
                    if (adminChecked)
                    {
                        role = Role.Admin;
                    }
                    else//входит как пользователь
                    {
                        if (DataBaseInterface.PlayerOnline(ID))
                        {
                            role = Role.Online;
                        }
                        else
                        {
                            role = Role.Admin;
                            if (!DataBaseInterface.SetPlayerOnline(ID, true))
                            {
                                role = Role.NetworkFailed;
                            }
                        }
                    }
                }
                else//нет прав админа
                {
                    if (DataBaseInterface.PlayerOnline(ID))
                    {
                        role = Role.Online;
                    }
                    else
                    {
                        role = Role.User;
                        if (!DataBaseInterface.SetPlayerOnline(ID, true))
                        {
                            role = Role.NetworkFailed;
                        }
                    }
                }

                return role;
            }
            catch (Exception ex)
            {
                return Role.NetworkFailed;
            }
        }

        //Функция входа
        public static string Enter(string login, string password, out int ID, bool adminChecked)
        {
            if(login is null || password is null)
            {
                ID = 0;
                return "Пустой логин или пароль!";
            }

            if (login.Length >= 50 || GetHash(password).Length >= 50)
            {
                ID = 0;
                return "Слишком длинный логин или пароль!";
            }

            try
            {
                ID = 0;
                Role role = GetRole(login, password, out ID, adminChecked);

                //OnlineTable

                if (role == Role.Failed)
                {
                    return "Неверный логин или пароль!";
                }
                else if (role == Role.Online)
                {
                    return "Сначала выйдете на другом устройстве!";
                }
                else if (role == Role.NetworkFailed)
                {
                    return "Не удалось достучаться на сервер!";
                }
                else
                {
                    if (role == Role.Admin)
                    {
                        return "Admin";
                    }
                    else if (role == Role.User)
                    {
                        return "User";
                    }
                }
            }
            catch (Exception ex)
            {
                ID = 0;
                return ex.Message;
            }

            return "Не удалось достучаться на сервер!";
        }

        //Получение логина по нику
        public static string GetLoginByNick(string nick)
        {
            if (nick is null)
            {
                return null;
            }

            if (nick.Length >= 50)
            {
                return "Слишком длинный логин или пароль!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetLoginByNick";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_Nickname", nick);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return null;
            }

            return dataForResponse.@string;
        }





        //////////////////////////////////////////////////////////////////
        ///             Регистрация
        //////////////////////////////////////////////////////////////////

        //Проверка на существование пользователя с таким логином или ником
        public static string CheckUser(string login, string nick)
        {
            if(login is null || nick is null)
            {
                return "null";
            }

            if (login.Length >= 50 || nick.Length >= 50)
            {
                return "Слишком длинный логин или ник!";
            }

            //Проверка логина
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_CheckUser";
            dataForMessage.dictionaryStringString.Add("@login", login);
            dataForMessage.dictionaryStringString.Add("@nick", nick);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "error";
            }

            return dataForResponse.@string;
        }

        //Функция регистрации
        public static string Registation(string login, string nick, string password)
        {
            if (login is null || nick is null || password is null)
            {
                return "Пустой логин, ник или пароль!";
            }

            if (login.Length >= 50 || nick.Length >= 50 || GetHash(password).Length >= 50)
            {
                return "Слишком длинный логин ник, или пароль!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_Registation";
            dataForMessage.dictionaryStringString.Add("@login", login);
            dataForMessage.dictionaryStringString.Add("@password", GetHash(password));
            dataForMessage.dictionaryStringString.Add("@nick", nick);
            dataForMessage.dictionaryStringString.Add("@dateOfRegistration", GetDate());
            dataForMessage.dictionaryStringString.Add("@dateOfLastOnline", GetDate());
            dataForMessage.dictionaryStringString.Add("@creditCard", "DBNull.Value");

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "error";
            }
            else
            {
                return dataForResponse.@string;
            }
        }


        //////////////////////////////////////////////////////////////////
        ///             Восстановление
        //////////////////////////////////////////////////////////////////

        //Проверка на существование данного ника
        public static bool IsLoginExist(string login)
        {
            if(login is null)
            {
                return false;
            }

            if(login == "")
            {
                return false;
            }

            if (login.Length >= 50)
            {
                return false;
            }

            if (CheckUser(login, "NICK") == "login")
            {
                return true;
            }

            return false;
        }







        //////////////////////////////////////////////////////////////////
        ///             ГЛАВНАЯ СТРАНИЦА
        //////////////////////////////////////////////////////////////////

        //Получение информации для главной страницы
        public Dictionary<string, string> GetPageInfo()
        {
            if (nowLogin is null)
            {
                return null;
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetPageInfo";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@login", nowLogin);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return null;
            }
            else
            {
                nowNick = (string)dataForResponse.dictionaryStringString["Nickname"].ToString().Clone();
                ID = int.Parse(dataForResponse.dictionaryStringString["Id"]);
                return dataForResponse.dictionaryStringString;
            }

        }







        //////////////////////////////////////////////////////////////////
        ///             ДРУЗЬЯ
        //////////////////////////////////////////////////////////////////

        //Добавление в друзья из подписчиков
        public string AddFriend(string clickNick)
        {
            if (clickNick is null)
            {
                return "Не удалось добавить в друзья!";
            }

            if (!dictionaryOfPlayerId.ContainsKey(clickNick))
            {
                return "Не удалось добавить в друзья!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_AddFriend";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_IdOfFirstFriend", ID.ToString());
            dataForMessage.dictionaryStringString.Add("@_IdOfSecondFriend", dictionaryOfPlayerId[clickNick].ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось добавить в друзья!";
            }
            else
            {
                return "OK";
            }
        }

        //Добавление в друзья из поиска
        public string AddSearchFriend(string clickNick)
        {
            if (clickNick is null)
            {
                return "Не удалось добавить в друзья!";
            }

            if (!dictionaryOfPlayerId.ContainsKey(clickNick))
            {
                return "Не удалось добавить в друзья!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_AddSearchFriend";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_IdOfFirstFriend", ID.ToString());
            dataForMessage.dictionaryStringString.Add("@_IdOfSecondFriend", dictionaryOfPlayerId[clickNick].ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось добавить в друзья!";
            }
            else
            {
                return "OK";
            }
        }


        //поиск игрока по нику
        public List<string> searchFriendsByNick(string searchNick)
        {
            if (searchNick is null)
            {
                return null;
            }

            if (searchNick.Length >= 50)
            {
                return null;
            }

            List<string> outList = new List<string>();

            foreach (var item in dictionaryOfPlayerId)
            {
                if (item.Key.Contains(searchNick))
                {
                    outList.Add((string)item.Key.Clone());
                }
            }

            return outList;
        }

        //Получение топа игроков по уровню
        public List<string> GetTopPlayers()
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetTopPlayers";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return null;
            }

            return dataForResponse.listString;
        }


        //Получение всех игроков
        public Dictionary<string, int> GetAllPlayers()
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetAllPlayers";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));

            dictionaryOfPlayerId.Clear();

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return null;
            }

            dictionaryOfPlayerId = dataForResponse.dictionaryStringInt;

            return dictionaryOfPlayerId;
        }

        //Получение друзей и подписчиков
        public Dictionary<string, List<KeyValuePair<string, bool>>> GetFriends(out int countOfPresents)
        {

            countOfPresents = 0;

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetFriends";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_Id", ID.ToString());

            listOfPresentsId.Clear();

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return null;
            }

            if(dataForResponse.dictionaryStringListPairStringBool is null ||
                !dataForResponse.dictionaryStringListPairStringBool.ContainsKey("friends") ||
                !dataForResponse.dictionaryStringListPairStringBool.ContainsKey("maybeFriends"))
            {
                return null;
            }

            int maxI = dataForResponse.dictionaryStringListPairStringBool["friends"].Count;

            for(int i = 0; i < maxI; ++i)
            {
                if(dataForResponse.dictionaryStringListPairStringBool["friends"][i].Value)
                {
                    listOfPresentsId.Add(i);
                    ++countOfPresents;
                }
            }

            return dataForResponse.dictionaryStringListPairStringBool;
        }


        private KeyValuePair<string, int> GetPresentItem()
        {
            var itemsList = GetItems();

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetPresentItem";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));

            List<int> listOfPresentsIdLocal = new List<int>();
            List<string> listOfPresentsNameLocal = new List<string>();

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return new KeyValuePair<string, int>("", -1);
            }

            if(dataForResponse.dictionaryStringInt is null)
            {
                return new KeyValuePair<string, int>("", -1);
            }

            foreach(var item in dataForResponse.dictionaryStringInt)
            {
                if (!itemsList.Contains(item.Key))
                {
                    listOfPresentsNameLocal.Add(item.Key);
                    listOfPresentsIdLocal.Add(item.Value);
                }
            }

            if (listOfPresentsIdLocal.Count == 0)
            {
                return new KeyValuePair<string, int>("", -100);
            }

            Random random = new Random();
            int value = random.Next(0, listOfPresentsId.Count);

            return new KeyValuePair<string, int>(listOfPresentsNameLocal[value], listOfPresentsIdLocal[value]);
        }


        //Получение подарка
        public string GetPresent(string clickNick)
        {
            if (clickNick is null)
            {
                return "Не удалось получить подарок!";
            }

            var item = GetPresentItem();
            if (item.Value == -100)
            {
                return "Все возможные подарки уже получены!";
            }
            else if (item.Value == -1)
            {
                return "Не удалось получить подарок!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetPresent";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_IdOfFirstFriend", ID.ToString());
            dataForMessage.dictionaryStringString.Add("@_IdOfSecondFriend", dictionaryOfPlayerId[clickNick].ToString());
            dataForMessage.dictionaryStringString.Add("@_IdOfItem", item.Value.ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось получить подарок!";
            }
            else
            {
                return "Вы получили " + item.Key + "!";
            }
        }

        //Нужно ли за этого игрока давать подарок
        public bool IsPresentForPlayer(int id)
        {
            return listOfPresentsId.Contains(id);
        }






        //////////////////////////////////////////////////////////////////
        ///             ВЫСТАВЛЕНИЕ ОНЛАЙНА
        //////////////////////////////////////////////////////////////////

        //Проверка в сети ли пользователь
        public static bool PlayerOnline(int id)
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_IsPlayerOnline";
            dataForMessage.dictionaryStringString.Add("@_id", id.ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return false;
            }

            return bool.Parse(dataForResponse.@string);
        }



        public static bool SetPlayerOnline(int id, bool online)
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_SetPlayerOnline";
            dataForMessage.dictionaryStringString.Add("@_isOnline", online.ToString());
            dataForMessage.dictionaryStringString.Add("@_IdOfPlayer", id.ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            return dataForResponse.datatype != DataForMessage.DataType.Error;
        }


        public bool SetPlayerOnline(bool online)
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_SetPlayerOnline";
            dataForMessage.dictionaryStringString.Add("@_isOnline", online.ToString());
            dataForMessage.dictionaryStringString.Add("@_IdOfPlayer", ID.ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            return dataForResponse.datatype != DataForMessage.DataType.Error;
        }








        //////////////////////////////////////////////////////////////////
        ///             ПРЕДМЕТЫ
        //////////////////////////////////////////////////////////////////

        //Получение предметов
        public List<string> GetItems()
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetItems";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_Id", ID.ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return null;
            }

            return dataForResponse.listString;
        }

        //Получение всех предметов
        public List<string> GetAllItems()
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetAllItems";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return null;
            }

            return dataForResponse.listString;
        }

        //Получение информации о предмете
        public Dictionary<string, string> GetItemInfo(string itemName)
        {
            if (itemName is null)
            {
                return null;
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetItemInfo";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_Name", itemName);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return null;
            }

            return dataForResponse.dictionaryStringString;
        }

        //Добавление предмета игроку
        public string AddItemToPlayer(string itemName)
        {
            if (itemName is null)
            {
                return "Не удалось добавить предмет!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_AddItemToPlayer";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_name", itemName);
            dataForMessage.dictionaryStringString.Add("@_IdOfPlayer", ID.ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось добавить предмет!";
            }

            return "OK";
        }

        //Удаление предмета у игрока
        public string DeleteItemFromPlayer(string itemName)
        {
            if (itemName is null)
            {
                return "Не удалось удалить предмет!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_DeleteItemFromPlayer";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_name", itemName);
            dataForMessage.dictionaryStringString.Add("@_IdOfPlayer", ID.ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось удалить предмет!";
            }

            return "OK";
        }

        //Изменение информации о предмете
        public string ChangeItemInfo(string name, string newName, string cost, string damage,
            string attackBonus, string ptotectionBonus, string magicBonus, string rarity)
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_ChangeItemInfo";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_newName", newName);
            dataForMessage.dictionaryStringString.Add("@_name", name);
            dataForMessage.dictionaryStringString.Add("@_cost", cost);
            dataForMessage.dictionaryStringString.Add("@_damage", damage);
            dataForMessage.dictionaryStringString.Add("@_rarity", rarity);
            dataForMessage.dictionaryStringString.Add("@_attackBonus", attackBonus);
            dataForMessage.dictionaryStringString.Add("@_ptotectionBonus", ptotectionBonus);
            dataForMessage.dictionaryStringString.Add("@_magicBonus", magicBonus);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось изменить данные!";
            }

            return "Данные изменены!";
        }

        public void CreateItem(string name)
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_CreateItem";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_name", name);

            StagingServerInterface.Send(dataForMessage);
        }

        //
        public void RemoveItem(string name)
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_RemoveItem";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_name", name);

            StagingServerInterface.Send(dataForMessage);
        }





        //////////////////////////////////////////////////////////////////
        ///             СУНДУКИ
        //////////////////////////////////////////////////////////////////

        //Получение сундуков пользователя
        public List<string> GetChests()
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetChests";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_Id", ID.ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return null;
            }

            return dataForResponse.listString;
        }

        //Получение всех сундуков
        public List<string> GetAllChests()
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetAllChests";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return null;
            }

            return dataForResponse.listString;
        }

        //Получение информации о сундуке
        public Dictionary<string, string> GetChestInfo(string chestName)
        {
            if (chestName is null)
            {
                return null;
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetChestInfo";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_Name", chestName);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return null;
            }

            return dataForResponse.dictionaryStringString;
        }

        //Добавление сундука игроку
        public string AddChestToPlayer(string chestName)
        {
            if (chestName is null)
            {
                return "Не удалось добавить сундук!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_AddChestToPlayer";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_name", chestName);
            dataForMessage.dictionaryStringString.Add("@_IdOfPlayer", ID.ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось добавить сундук!";
            }

            return "OK";
        }

        //Удаление сундука у игрока
        public string DeleteChestFromPlayer(string chestName)
        {
            if (chestName is null)
            {
                return "Не удалось удалить сундук!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_DeleteChestFromPlayer";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_name", chestName);
            dataForMessage.dictionaryStringString.Add("@_IdOfPlayer", ID.ToString());

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось удалить сундук!";
            }

            return "OK";
        }

        //Изменение информации о сундуке
        public string ChangeChestInfo(string name, string newName, string cost, string bauble,
            string usual, string rare, string superRare, string unique)
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_ChangeChestInfo";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_newName", newName);
            dataForMessage.dictionaryStringString.Add("@_name", name);
            dataForMessage.dictionaryStringString.Add("@_cost", cost);
            dataForMessage.dictionaryStringString.Add("@_bauble", bauble);
            dataForMessage.dictionaryStringString.Add("@_usual", usual);
            dataForMessage.dictionaryStringString.Add("@_rare", rare);
            dataForMessage.dictionaryStringString.Add("@_superRare", superRare);
            dataForMessage.dictionaryStringString.Add("@_unique", unique);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось изменить данные!";
            }

            return "Данные изменены!";
        }

        private KeyValuePair<string, int> GetLootItem(string rarity, ListBox.ObjectCollection havingItems)
        {
            var itemsList = GetItems();

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetLootItem";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_rarity", rarity);

            List<int> listOfPresentsId = new List<int>();
            List<string> listOfPresentsNames = new List<string>();

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                new KeyValuePair<string, int>("error", -1);
            }

            if(dataForResponse.dictionaryStringInt is null)
            {
                new KeyValuePair<string, int>("error", -1);
            }

            foreach(var item in dataForResponse.dictionaryStringInt)
            {
                if (!havingItems.Contains(item.Key))
                {
                    listOfPresentsNames.Add(item.Key);
                    listOfPresentsId.Add(item.Value);
                }
            }

            if (listOfPresentsNames.Count == 0)
            {
                return new KeyValuePair<string, int>("error", -100);
            }

            Random random = new Random();
            int value = random.Next(0, listOfPresentsId.Count);

            return new KeyValuePair<string, int>(listOfPresentsNames[value], listOfPresentsId[value]);
        }

        //Открытие сундука
        public string GetLootOfChest(string rarity, string clickChest, ListBox.ObjectCollection havingItems)
        {
            if (rarity is null || clickChest is null)
            {
                return "Не удалось получить предмет!";
            }

            var pair = GetLootItem(rarity, havingItems);

            if (pair.Value == -1)
            {
                return "Не удалось получить предмет!";
            }
            if (pair.Value == -100)
            {
                return "Все возможные предметы уже получены!";
            }
            else
            {
                DataForMessage dataForMessage = new DataForMessage();
                dataForMessage.SetDataType(DataForMessage.DataType.Command);
                dataForMessage.@string = "SP_GetLootOfChest";
                dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
                dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
                dataForMessage.dictionaryStringString.Add("@_IdOfPlayer", ID.ToString());
                dataForMessage.dictionaryStringString.Add("@_IdOfItem", pair.Value.ToString());
                dataForMessage.dictionaryStringString.Add("@_name", clickChest);

                DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
                if (dataForResponse.datatype == DataForMessage.DataType.Error)
                {
                    return "Не удалось получить предмет!";
                }
                else
                {
                    return "Вы получили " + pair.Key + " (" + rarity + ")!";
                }
            }
        }

        public void CreateChest(string name)
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_CreateChest";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_name", name);

            StagingServerInterface.Send(dataForMessage);
        }

        //
        public void RemoveChest(string name)
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_RemoveChest";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_name", name);

            StagingServerInterface.Send(dataForMessage);
        }








        //////////////////////////////////////////////////////////////////
        ///             НАСТРОЙКИ
        //////////////////////////////////////////////////////////////////

        //Проверка логина
        private bool ErrorLogin(string newLogin)
        {
            if (newLogin != nowLogin)
            {
                //Проверка логина
                DataForMessage dataForMessage = new DataForMessage();
                dataForMessage.SetDataType(DataForMessage.DataType.Command);
                dataForMessage.@string = "SP_IsErrorLogin";
                dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
                dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
                dataForMessage.dictionaryStringString.Add("@login", newLogin);

                DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
                if (dataForResponse.datatype == DataForMessage.DataType.Error)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        //Изменение логина
        public string ChangeLogin(string newLogin)
        {
            if (newLogin is null)
            {
                return "Пустой логин!";
            }

            if (newLogin.Length >= 50)
            {
                return " Слишком длинный логин!";
            }

            if (ErrorLogin(newLogin))
            {
                return "Такой логин уже есть!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_ChangeLogin";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@login", newLogin);
            dataForMessage.dictionaryStringString.Add("@nowLogin", nowLogin);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось изменить данные!";
            }
            else
            {
                if(nowLogin == senderLogin)
                {
                    senderLogin = (string)newLogin.Clone();
                }
                nowLogin = (string)newLogin.Clone();
                
                return "Данные изменены!";
            }
        }

        //Проверка ника
        private bool ErrorNick(string newNick)
        {
            if (newNick != nowNick)
            {
                //Проверка ника
                DataForMessage dataForMessage = new DataForMessage();
                dataForMessage.SetDataType(DataForMessage.DataType.Command);
                dataForMessage.@string = "SP_IsErrorNick";
                dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
                dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
                dataForMessage.dictionaryStringString.Add("@nick", newNick);

                DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
                if (dataForResponse.datatype == DataForMessage.DataType.Error)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        //Изменение ника
        public string ChangeNick(string newNick)
        {
            if (newNick is null)
            {
                return "Пустой ник!";
            }

            if (newNick.Length >= 50)
            {
                return " Слишком длинный ник!";
            }

            if (ErrorNick(newNick))
            {
                return "Такой ник уже есть!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_ChangeNick";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@nick", newNick);
            dataForMessage.dictionaryStringString.Add("@nowLogin", nowLogin);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось изменить данные!";
            }
            else
            {
                nowNick = (string)newNick.Clone();
                return "Данные изменены!";
            }
        }

        //Изменение кредитной карты
        public string ChangeCredit(string newCredit)
        {
            Regex reg = new Regex(@"^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$");
            if (!reg.IsMatch(newCredit))
            {
               return "Введите верный номер карты!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_ChangeCredit";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));

            if (newCredit is null || newCredit == "" || newCredit == "Добавьте карту")
            {
                dataForMessage.dictionaryStringString.Add("@_newCredit", "DBNull.Value");
                dataForMessage.dictionaryStringString.Add("@nowLogin", nowLogin);
            }
            else
            {
                dataForMessage.dictionaryStringString.Add("@_newCredit", newCredit);
                dataForMessage.dictionaryStringString.Add("@nowLogin", nowLogin);
            }

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось изменить данные!";
            }
            else
            {
                return "Данные изменены!";
            }
        }

        //Изменение количества золота
        public string ChangeGold(string countOfGold)
        {
            if (countOfGold is null)
            {
                return "Пустое поле!";
            }

            foreach(var c in countOfGold)
            {
                if(!Char.IsNumber(c))
                {
                    return "Требуется число!";
                }
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_ChangeGold";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_countOfGold", countOfGold);
            dataForMessage.dictionaryStringString.Add("@nowLogin", nowLogin);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось изменить данные!";
            }
            else
            {
                return "Данные изменены!";
            }
        }


        //Изменение уровня
        public string ChangeLevel(string level)
        {
            if (level is null)
            {
                return "Пустое поле!";
            }

            foreach (var c in level)
            {
                if (!Char.IsNumber(c))
                {
                    return "Требуется число!";
                }
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_ChangeLevel";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_level", level);
            dataForMessage.dictionaryStringString.Add("@nowLogin", nowLogin);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось изменить данные!";
            }
            else
            {
                return "Данные изменены!";
            }
        }


        //Изменение прав администратора
        public string ChangeRole(bool role)
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_ChangeRole";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_role", role.ToString());
            dataForMessage.dictionaryStringString.Add("@nowLogin", nowLogin);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось изменить данные!";
            }
            else
            {
                return "Данные изменены!";
            }
        }


        //Изменение пароля
        public string ChangePassword(string newPassword)
        {
            if (newPassword is null)
            {
                return "Пустое поле!";
            }

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_ChangePassword";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@_newPassword", GetHash(newPassword));
            dataForMessage.dictionaryStringString.Add("@nowLogin", nowLogin);

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return "Не удалось изменить данные!";
            }
            else
            {
                if (nowLogin == senderLogin)
                {
                    senderPassword = (string)newPassword.Clone();
                }
                return "Данные изменены!";
            }
        }


        //Изменение даты последнего онлайна
        public void ChangeDateOfLastOnline()
        {
            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_ChangeDateOfLastOnline";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));
            dataForMessage.dictionaryStringString.Add("@dateOfLastOnline", GetDate());
            dataForMessage.dictionaryStringString.Add("@nowLogin", nowLogin);

            StagingServerInterface.Send(dataForMessage);
        }


        


        

        //////////////////////////////////////////////////////////////////
        ///             БЭКАП
        //////////////////////////////////////////////////////////////////

        public static int BackUp(string backupFileName)
        {
            FileStream fileStream = new FileStream(backupFileName, FileMode.Create);
            fileStream.Close();

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetDBConnectionString";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return 0;
            }

            var sqlConStrBuilder = new SqlConnectionStringBuilder(dataForResponse.@string);

            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                var query = String.Format("BACKUP DATABASE {0} TO DISK='{1}'",
                    sqlConStrBuilder.InitialCatalog, backupFileName);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }


        public static int LoadFrom(string backupFileName)
        {
            FileStream fileStream = new FileStream(backupFileName, FileMode.Open);
            fileStream.Close();

            DataForMessage dataForMessage = new DataForMessage();
            dataForMessage.SetDataType(DataForMessage.DataType.Command);
            dataForMessage.@string = "SP_GetDBConnectionString";
            dataForMessage.dictionaryStringString.Add("@loginFrom", senderLogin);
            dataForMessage.dictionaryStringString.Add("@passwordFrom", GetHash(senderPassword));

            DataForMessage dataForResponse = StagingServerInterface.Send(dataForMessage);
            if (dataForResponse.datatype == DataForMessage.DataType.Error)
            {
                return 0;
            }

            var sqlConStrBuilder = new SqlConnectionStringBuilder(dataForResponse.@string);

            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                connection.Open();

                SqlCommand cmd1 = new SqlCommand("ALTER DATABASE [Game] SET SINGLE_USER WITH ROLLBACK IMMEDIATE ", connection);
                if(cmd1.ExecuteNonQuery() == 0)
                {
                    return 0;
                }

                SqlCommand cmd2 = new SqlCommand("USE MASTER RESTORE DATABASE [Game] FROM DISK='" + backupFileName + "' WITH REPLACE", connection);
                if (cmd2.ExecuteNonQuery() == 0)
                {
                    return 0;
                }

                SqlCommand cmd3 = new SqlCommand("ALTER DATABASE [Game] SET MULTI_USER", connection);
                if (cmd3.ExecuteNonQuery() == 0)
                {
                    return 0;
                }

                return 1;
            }
        }

        //////////////////////////////////////////////////////////////////
        ///             ХЭШИРОВАНИЕ
        //////////////////////////////////////////////////////////////////

        private static string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var ret = Convert.ToBase64String(hash);
            return ret;
        }

        private static string GetDate()
        {
            string date = DateTime.Now.ToString("d");

            String[] words = date.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            date = words[2] + words[1] + words[0];

            return date;
        }


    }
}
