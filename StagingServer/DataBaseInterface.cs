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

namespace StagingServer
{
    public static class DataBaseInterface
    {
        public static string DBConnectionString = "server=DESKTOP-RFI8Q0J;Trusted_Connection=Yes;DataBase=Game;";

        

        //////////////////////////////////////////////////////////////////
        ///             ВХОД
        //////////////////////////////////////////////////////////////////

        //Получение роли по логину и паролю
        public static DataForMessage GetRole(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage.dictionaryStringString is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@login") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@password"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пусты поля логина или пароля";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@login"].Length >= 50 
                || dataForMessage.dictionaryStringString["@password"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль!";
                return dataForResponse;
            }

            try
            {
                dataForResponse.SetDataType(DataForMessage.DataType.dictionaryStringString);

                using (var sqlConnection = new SqlConnection(DBConnectionString))
                {
                    sqlConnection.Open();

                    var command = new SqlCommand("SP_GetRole", sqlConnection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@login", dataForMessage.dictionaryStringString["@login"]);
                    command.Parameters.AddWithValue("@password", dataForMessage.dictionaryStringString["@password"]);

                    using (var dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            dataForResponse.dictionaryStringString.Add("Role", dataReader["Role"].ToString());
                            dataForResponse.dictionaryStringString.Add("Id", dataReader["Id"].ToString());
                        }
                    }
                }

                return dataForResponse;
            }
            catch (Exception ex)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = ex.Message;
                return dataForResponse;
            }
        }

        //Получение логина по нику
        public static DataForMessage GetLoginByNick(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_Nickname"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@_Nickname"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный ник!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                var command = new SqlCommand("SP_GetLoginByNick", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_Nickname", dataForMessage.dictionaryStringString["@_Nickname"]);

                using (var dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        dataForResponse.@string = dataReader["LoginEmail"].ToString();
                    }
                }

                return dataForResponse;
            }
        }





        //////////////////////////////////////////////////////////////////
        ///             Регистрация
        //////////////////////////////////////////////////////////////////

        //Проверка на существование пользователя с таким логином или ником
        public static DataForMessage CheckUser(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@login") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nick"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@login"].Length >= 50
                || dataForMessage.dictionaryStringString["@nick"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или ник!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                //Проверка логина
                var command = new SqlCommand("SP_CheckUser_CheckLogin", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@login", dataForMessage.dictionaryStringString["@login"]);

                using (var dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        dataForResponse.@string = "login";
                        return dataForResponse;
                    }
                }


                //Проверка ника
                command = new SqlCommand("SP_CheckUser_CheckNick", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@nick", dataForMessage.dictionaryStringString["@nick"]);

                using (var dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        dataForResponse.@string = "nick";
                        return dataForResponse;
                    }
                }


                dataForResponse.@string = "ok";
                return dataForResponse;
            }
        }

        //Функция регистрации
        public static DataForMessage Registation(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@login") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nick") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@password"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@login"].Length >= 50
                || dataForMessage.dictionaryStringString["@nick"].Length >= 50
                || dataForMessage.dictionaryStringString["@password"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин, ник или пароль!";
                return dataForResponse;
            }

            DataForMessage dataForError = CheckUser(dataForMessage);
            if (dataForError.datatype == DataForMessage.DataType.Error)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "error";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            if (dataForError.@string == "login")
            {
                dataForResponse.@string = "Такой логин уже есть!";
                return dataForResponse;
            }
            else if (dataForError.@string == "nick")
            {
                dataForResponse.@string = "Такой ник уже есть!";
                return dataForResponse;
            }

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                //Проверка логина
                var command = new SqlCommand("SP_Registation", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@login", dataForMessage.dictionaryStringString["@login"]);
                command.Parameters.AddWithValue("@password", dataForMessage.dictionaryStringString["@password"]);
                command.Parameters.AddWithValue("@nick", dataForMessage.dictionaryStringString["@nick"]);
                command.Parameters.AddWithValue("@dateOfRegistration", GetDate());
                command.Parameters.AddWithValue("@dateOfLastOnline", GetDate());
                command.Parameters.AddWithValue("@creditCard", DBNull.Value);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "Аккаунт создан!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.@string = "Не удалось создать аккаунт!";
                    return dataForResponse;
                }
            }
        }





        //////////////////////////////////////////////////////////////////
        ///             ГЛАВНАЯ СТРАНИЦА
        //////////////////////////////////////////////////////////////////

        //Получение информации для главной страницы
        public static DataForMessage GetPageInfo(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@login"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@login"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.dictionaryStringString);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                if (sqlConnection is null)
                {
                    return null;
                }

                var command = new SqlCommand("SP_GetPageInfo", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@login", dataForMessage.dictionaryStringString["@login"]);

                using (var dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        dataForResponse.dictionaryStringString.Add("Nickname", (string)dataReader["Nickname"]);

                        dataForResponse.dictionaryStringString.Add("Level", dataReader["Level"].ToString());
                        dataForResponse.dictionaryStringString.Add("DateOfRegistration", "Дата регистрации: " + dataReader["DateOfRegistration"].ToString());
                        dataForResponse.dictionaryStringString.Add("CountOfGold", dataReader["CountOfGold"].ToString());
                        dataForResponse.dictionaryStringString.Add("Id", dataReader["Id"].ToString());

                        if (dataReader["CreditCard"] is DBNull)
                        {
                            dataForResponse.dictionaryStringString.Add("CreditCard", "Добавьте карту");
                        }
                        else
                        {
                            dataForResponse.dictionaryStringString.Add("CreditCard", dataReader["CreditCard"].ToString());
                        }

                        if ((bool)dataReader["Role"])
                        {
                            dataForResponse.dictionaryStringString.Add("Role", "admin");
                        }
                        else
                        {
                            dataForResponse.dictionaryStringString.Add("Role", "user");
                        }
                    }

                    return dataForResponse;
                }
            }
        }







        //////////////////////////////////////////////////////////////////
        ///             ДРУЗЬЯ
        //////////////////////////////////////////////////////////////////

        //Добавление в друзья из подписчиков
        public static DataForMessage AddFriend(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfFirstFriend") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfSecondFriend"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_AddFriend", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_BothSide", "Друг");
                command.Parameters.AddWithValue("@_IdOfFirstFriend", dataForMessage.dictionaryStringString["@_IdOfFirstFriend"]);
                command.Parameters.AddWithValue("@_IdOfSecondFriend", dataForMessage.dictionaryStringString["@_IdOfSecondFriend"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    command = new SqlCommand("SP_AddFriend", sqlConnection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                    command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                    command.Parameters.AddWithValue("@_BothSide", "Подарок");
                    command.Parameters.AddWithValue("@_IdOfFirstFriend", dataForMessage.dictionaryStringString["@_IdOfSecondFriend"]);
                    command.Parameters.AddWithValue("@_IdOfSecondFriend", dataForMessage.dictionaryStringString["@_IdOfFirstFriend"]);

                    if (command.ExecuteNonQuery() != 1)
                    {
                        dataForResponse.SetDataType(DataForMessage.DataType.Error);
                        dataForResponse.@string = "Не удалось добавить в друзья!";
                        return dataForResponse;
                    }

                    dataForResponse.@string = "OK";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось добавить в друзья!";
                    return dataForResponse;
                }
            }
        }

        //Добавление в друзья из поиска
        public static DataForMessage AddSearchFriend(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfFirstFriend") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfSecondFriend"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_AddSearchFriend", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_BothSide", "Ожидание");
                command.Parameters.AddWithValue("@_IdOfFirstFriend", dataForMessage.dictionaryStringString["@_IdOfFirstFriend"]);
                command.Parameters.AddWithValue("@_IdOfSecondFriend", dataForMessage.dictionaryStringString["@_IdOfSecondFriend"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    command = new SqlCommand("SP_AddSearchFriend", sqlConnection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                    command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                    command.Parameters.AddWithValue("@_BothSide", "Подписчик");
                    command.Parameters.AddWithValue("@_IdOfFirstFriend", dataForMessage.dictionaryStringString["@_IdOfSecondFriend"]);
                    command.Parameters.AddWithValue("@_IdOfSecondFriend", dataForMessage.dictionaryStringString["@_IdOfFirstFriend"]);

                    if (command.ExecuteNonQuery() != 1)
                    {
                        dataForResponse.SetDataType(DataForMessage.DataType.Error);
                        dataForResponse.@string = "Не удалось добавить в друзья!";
                        return dataForResponse;
                    }

                    dataForResponse.@string = "OK";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось добавить в друзья!";
                    return dataForResponse;
                }
            }
        }

        //Получение топа игроков по уровню
        public static DataForMessage GetTopPlayers(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null || 
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.listString);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                if (sqlConnection is null)
                {
                    return null;
                }

                var command = new SqlCommand("SP_GetTopPlayers", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        dataForResponse.listString.Add
                            ((string)dataReader["Nickname"] + "(" + dataReader["Level"].ToString() + ")");
                    }
                }

                return dataForResponse;
            }
        }


        //Получение всех игроков
        public static DataForMessage GetAllPlayers(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null || 
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.dictionaryStringInt);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                if (sqlConnection is null)
                {
                    return null;
                }

                var command = new SqlCommand("SP_GetAllPlayers", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (dataReader["Id"] is DBNull)
                        {
                            return dataForResponse;
                        }

                        dataForResponse.dictionaryStringInt.Add((string)dataReader["Nickname"], (int)dataReader["Id"]);
                    }
                }

                return dataForResponse;
            }
        }

        //Получение друзей и подписчиков
        public static DataForMessage GetFriends(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_Id"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.dictionaryStringListPairStringBool);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                if (sqlConnection is null)
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Пустой запрос";
                    return dataForResponse;
                }

                dataForResponse.dictionaryStringListPairStringBool.Add("friends", new List<KeyValuePair<string, bool>>());
                dataForResponse.dictionaryStringListPairStringBool.Add("maybeFriends", new List<KeyValuePair<string, bool>>());
                
                var command = new SqlCommand("SP_GetFriends", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_Id", dataForMessage.dictionaryStringString["@_Id"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (dataReader["BothSide"].ToString() == "Подписчик")
                        {
                            dataForResponse.dictionaryStringListPairStringBool["maybeFriends"].
                                Add(new KeyValuePair<string, bool>((string)dataReader["Nickname"], false));
                        }
                        else if (dataReader["BothSide"].ToString() == "Друг" || dataReader["BothSide"].ToString() == "Ожидание")
                        {
                            dataForResponse.dictionaryStringListPairStringBool["friends"].
                                Add(new KeyValuePair<string, bool>((string)dataReader["Nickname"], false));
                        }
                        else if (dataReader["BothSide"].ToString() == "Подарок")
                        {
                            dataForResponse.dictionaryStringListPairStringBool["friends"].
                                Add(new KeyValuePair<string, bool>((string)dataReader["Nickname"], true));
                        }
                    }
                }

                return dataForResponse;
            }
        }


        public static DataForMessage GetPresentItem(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null || 
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.dictionaryStringInt);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                var command = new SqlCommand("SP_GetPresentItem", sqlConnection);
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        dataForResponse.dictionaryStringInt.Add((string)dataReader["Name"], (int)dataReader["Id"]);
                    }
                }
            }

            return dataForResponse;
        }


        //Получение подарка
        public static DataForMessage GetPresent(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfFirstFriend") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfSecondFriend") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfItem"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_AddFriend", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_BothSide", "Друг");
                command.Parameters.AddWithValue("@_IdOfFirstFriend", dataForMessage.dictionaryStringString["@_IdOfFirstFriend"]);
                command.Parameters.AddWithValue("@_IdOfSecondFriend", dataForMessage.dictionaryStringString["@_IdOfSecondFriend"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    command = new SqlCommand("SP_GetPresent", sqlConnection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                    command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                    command.Parameters.AddWithValue("@_IdOfPlayer", dataForMessage.dictionaryStringString["@_IdOfFirstFriend"]);
                    command.Parameters.AddWithValue("@_IdOfItem", dataForMessage.dictionaryStringString["@_IdOfItem"]);

                    if (command.ExecuteNonQuery() == 1)
                    {
                        dataForResponse.@string = "Вы получили подарок!";
                        return dataForResponse;
                    }
                    else
                    {
                        dataForResponse.SetDataType(DataForMessage.DataType.Error);
                        dataForResponse.@string = "Не удалось получить подарок!";
                        return dataForResponse;
                    }
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось получить подарок!";
                    return dataForResponse;
                }
            }
        }





        //////////////////////////////////////////////////////////////////
        ///             ВЫСТАВЛЕНИЕ МЕТКИ ОНЛАЙНА
        //////////////////////////////////////////////////////////////////

        //Проверка в сети ли пользователь
        public static DataForMessage PlayerOnline(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_id"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                var command = new SqlCommand("SP_IsPlayerOnline", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@_id", dataForMessage.dictionaryStringString["@_id"]);

                using (var dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        if ((bool)dataReader["isOnline"])
                        {
                            dataForResponse.@string = true.ToString();
                            return dataForResponse;
                        }
                        else
                        {
                            dataForResponse.@string = false.ToString();
                            return dataForResponse;
                        }
                    }
                    else
                    {
                        dataReader.Close();

                        command = new SqlCommand("SP_AddPlayerOnline", sqlConnection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@_IdOfPlayer", dataForMessage.dictionaryStringString["@_id"]);

                        command.ExecuteNonQuery();

                        dataForResponse.@string = false.ToString();
                        return dataForResponse;
                    }
                }
            }
        }



        public static DataForMessage SetPlayerOnline(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_isOnline") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfPlayer"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                var command = new SqlCommand("SP_SetPlayerOnline", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@_isOnline", dataForMessage.dictionaryStringString["@_isOnline"]);
                command.Parameters.AddWithValue("@_IdOfPlayer", dataForMessage.dictionaryStringString["@_IdOfPlayer"]);

                dataForResponse.@string = (command.ExecuteNonQuery() == 1).ToString();
                return dataForResponse;
            }
        }





        //////////////////////////////////////////////////////////////////
        ///             ПРЕДМЕТЫ
        //////////////////////////////////////////////////////////////////

        //Получение предметов
        public static DataForMessage GetItems(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_Id"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.listString);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                if (sqlConnection is null)
                {
                    return null;
                }

                var command = new SqlCommand("SP_GetItems", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_Id", dataForMessage.dictionaryStringString["@_Id"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        dataForResponse.listString.Add((string)dataReader["Name"]);
                    }
                }

                return dataForResponse;
            }
        }

        //Получение всех предметов
        public static DataForMessage GetAllItems(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null || 
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.listString);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                if (sqlConnection is null)
                {
                    return null;
                }

                var command = new SqlCommand("SP_GetAllItems", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        dataForResponse.listString.Add((string)dataReader["Name"]);
                    }
                }

                return dataForResponse;
            }
        }

        //Получение информации о предмете
        public static DataForMessage GetItemInfo(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_Name"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.dictionaryStringString);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                if (sqlConnection is null)
                {
                    return null;
                }

                var command = new SqlCommand("SP_GetItemInfo", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_Name", dataForMessage.dictionaryStringString["@_Name"]);

                using (var dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        dataForResponse.dictionaryStringString.Add("Cost", dataReader["Cost"].ToString());
                        dataForResponse.dictionaryStringString.Add("Damage", dataReader["Damage"].ToString());
                        dataForResponse.dictionaryStringString.Add("AttackBonus", dataReader["AttackBonus"].ToString());
                        dataForResponse.dictionaryStringString.Add("PtotectionBonus", dataReader["PtotectionBonus"].ToString());
                        dataForResponse.dictionaryStringString.Add("MagicBonus", dataReader["MagicBonus"].ToString());
                        dataForResponse.dictionaryStringString.Add("Rarity", dataReader["Rarity"].ToString());
                    }
                }

                sqlConnection.Close();

                return dataForResponse;
            }
        }

        //Добавление предмета игроку
        public static DataForMessage AddItemToPlayer(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfPlayer"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;
                string idOfItem = "";

                command = new SqlCommand("SP_GetItemIdByName", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_name", dataForMessage.dictionaryStringString["@_name"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        idOfItem = dataReader["Id"].ToString();
                    }
                }

                command = new SqlCommand("SP_AddItemToPlayer", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_IdOfPlayer", dataForMessage.dictionaryStringString["@_IdOfPlayer"]);
                command.Parameters.AddWithValue("@_IdOfItem", idOfItem);

                if (command.ExecuteNonQuery() != 1)
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось добавить предмет!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.@string = "Успешно!";
                    return dataForResponse;
                }
            }
        }

        //Удаление предмета у игрока
        public static DataForMessage DeleteItemFromPlayer(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfPlayer"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;
                string idOfItem = "";

                command = new SqlCommand("SP_GetItemIdByName", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_name", dataForMessage.dictionaryStringString["@_name"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        idOfItem = dataReader["Id"].ToString();
                    }
                }

                command = new SqlCommand("SP_DeleteItemFromPlayer", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_IdOfPlayer", dataForMessage.dictionaryStringString["@_IdOfPlayer"]);
                command.Parameters.AddWithValue("@_IdOfItem", idOfItem);

                if (command.ExecuteNonQuery() != 1)
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось удалить предмет!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.@string = "Успешно!";
                    return dataForResponse;
                }
            }
        }

        //Изменение информации о предмете
        public static DataForMessage ChangeItemInfo(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_newName") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_cost") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_damage") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_rarity") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_attackBonus") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_ptotectionBonus") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_magicBonus"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_ChangeItemInfo", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_newName", dataForMessage.dictionaryStringString["@_newName"]);
                command.Parameters.AddWithValue("@_name", dataForMessage.dictionaryStringString["@_name"]);
                command.Parameters.AddWithValue("@_cost", dataForMessage.dictionaryStringString["@_cost"]);
                command.Parameters.AddWithValue("@_damage", dataForMessage.dictionaryStringString["@_damage"]);
                command.Parameters.AddWithValue("@_rarity", dataForMessage.dictionaryStringString["@_rarity"]);
                command.Parameters.AddWithValue("@_attackBonus", dataForMessage.dictionaryStringString["@_attackBonus"]);
                command.Parameters.AddWithValue("@_ptotectionBonus", dataForMessage.dictionaryStringString["@_ptotectionBonus"]);
                command.Parameters.AddWithValue("@_magicBonus", dataForMessage.dictionaryStringString["@_magicBonus"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "Данные изменены!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось изменить данные!";
                    return dataForResponse;
                }
            }
        }

        public static DataForMessage CreateItem(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_CreateItem", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_name", dataForMessage.dictionaryStringString["@_name"]);

                if(command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "OK";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Пустой запрос";
                    return dataForResponse;
                }
            }
        }

        //
        public static DataForMessage RemoveItem(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_RemoveItem", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_name", dataForMessage.dictionaryStringString["@_name"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "OK";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Пустой запрос";
                    return dataForResponse;
                }
            }
        }





        //////////////////////////////////////////////////////////////////
        ///             СУНДУКИ
        //////////////////////////////////////////////////////////////////

        //Получение сундуков пользователя
        public static DataForMessage GetChests(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_Id"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.listString);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                if (sqlConnection is null)
                {
                    return null;
                }

                List<string> outList = new List<string>();

                var command = new SqlCommand("SP_GetChests", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_Id", dataForMessage.dictionaryStringString["@_Id"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        dataForResponse.listString.Add((string)dataReader["Name"]);
                    }
                }

                return dataForResponse;
            }
        }

        //Получение всех сундуков
        public static DataForMessage GetAllChests(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null || 
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.listString);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                if (sqlConnection is null)
                {
                    return null;
                }

                List<string> outList = new List<string>();

                var command = new SqlCommand("SP_GetAllChests", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        dataForResponse.listString.Add((string)dataReader["Name"]);
                    }
                }

                return dataForResponse;
            }
        }

        //Получение информации о сундуке
        public static DataForMessage GetChestInfo(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_Name"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.dictionaryStringString);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                if (sqlConnection is null)
                {
                    return null;
                }

                var command = new SqlCommand("SP_GetChestInfo", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_Name", dataForMessage.dictionaryStringString["@_Name"]);

                using (var dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        dataForResponse.dictionaryStringString.Add("Cost", dataReader["Cost"].ToString());
                        dataForResponse.dictionaryStringString.Add("Bauble", dataReader["Bauble"].ToString());
                        dataForResponse.dictionaryStringString.Add("Usual", dataReader["Usual"].ToString());
                        dataForResponse.dictionaryStringString.Add("Rare", dataReader["Rare"].ToString());
                        dataForResponse.dictionaryStringString.Add("SuperRare", dataReader["SuperRare"].ToString());
                        dataForResponse.dictionaryStringString.Add("Unique", dataReader["Unique"].ToString());
                    }
                }

                sqlConnection.Close();

                return dataForResponse;
            }
        }

        //Добавление сундука игроку
        public static DataForMessage AddChestToPlayer(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfPlayer"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;
                string idOfChest = "";

                command = new SqlCommand("SP_GetChestIdByName", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_name", dataForMessage.dictionaryStringString["@_name"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        idOfChest = dataReader["Id"].ToString();
                    }
                }

                command = new SqlCommand("SP_AddChestToPlayer", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_IdOfPlayer", dataForMessage.dictionaryStringString["@_IdOfPlayer"]);
                command.Parameters.AddWithValue("@_IdOfChest", idOfChest);

                if (command.ExecuteNonQuery() != 1)
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось добавить сундук!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.@string = "Успешно!";
                    return dataForResponse;
                }
            }
        }

        //Удаление сундука у игрока
        public static DataForMessage DeleteChestFromPlayer(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfPlayer"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;
                string idOfChest = "";

                command = new SqlCommand("SP_GetChestIdByName", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_name", dataForMessage.dictionaryStringString["@_name"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        idOfChest = dataReader["Id"].ToString();
                    }
                }

                if(idOfChest == "")
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Сундук не найден";
                    return dataForResponse;
                }

                command = new SqlCommand("SP_DeleteChestFromPlayer", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_IdOfPlayer", dataForMessage.dictionaryStringString["@_IdOfPlayer"]);
                command.Parameters.AddWithValue("@_IdOfChest", idOfChest);

                if (command.ExecuteNonQuery() != 1)
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось удалить сундук!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.@string = "Успешно!";
                    return dataForResponse;
                }
            }
        }

        //Изменение информации о сундуке
        public static DataForMessage ChangeChestInfo(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_newName") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_cost") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_bauble") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_usual") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_rare") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_superRare") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_unique"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_ChangeChestInfo", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_newName", dataForMessage.dictionaryStringString["@_newName"]);
                command.Parameters.AddWithValue("@_name", dataForMessage.dictionaryStringString["@_name"]);
                command.Parameters.AddWithValue("@_cost", dataForMessage.dictionaryStringString["@_cost"]);
                command.Parameters.AddWithValue("@_bauble", dataForMessage.dictionaryStringString["@_bauble"]);
                command.Parameters.AddWithValue("@_usual", dataForMessage.dictionaryStringString["@_usual"]);
                command.Parameters.AddWithValue("@_rare", dataForMessage.dictionaryStringString["@_rare"]);
                command.Parameters.AddWithValue("@_superRare", dataForMessage.dictionaryStringString["@_superRare"]);
                command.Parameters.AddWithValue("@_unique", dataForMessage.dictionaryStringString["@_unique"]);

                if (command.ExecuteNonQuery() != 1)
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось изменить данные!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.@string = "Данные изменены!";
                    return dataForResponse;
                }
            }
        }

        public static DataForMessage GetLootItem(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_rarity"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.dictionaryStringInt);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                var command = new SqlCommand("SP_GetLootItem", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_rarity", dataForMessage.dictionaryStringString["@_rarity"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        dataForResponse.dictionaryStringInt.Add((string)dataReader["Name"], (int)dataReader["Id"]);
                    }
                }
            }

            return dataForResponse;
        }

        //Открытие сундука
        public static DataForMessage GetLootOfChest(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfPlayer") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_IdOfItem") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;
                string idOfChest = "";

                command = new SqlCommand("SP_GetChestIdByName", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_name", dataForMessage.dictionaryStringString["@_name"]);

                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        idOfChest = dataReader["Id"].ToString();
                    }
                }

                if (idOfChest == "")
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Сундук не найден";
                    return dataForResponse;
                }

                command = new SqlCommand("SP_DeleteChestFromPlayer", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_IdOfPlayer", dataForMessage.dictionaryStringString["@_IdOfPlayer"]);
                command.Parameters.AddWithValue("@_IdOfChest", idOfChest);

                if (command.ExecuteNonQuery() != 1)
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось удалить сундук!";
                    return dataForResponse;
                }

                command = new SqlCommand("SP_AddItemToPlayer", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_IdOfPlayer", dataForMessage.dictionaryStringString["@_IdOfPlayer"]);
                command.Parameters.AddWithValue("@_IdOfItem", dataForMessage.dictionaryStringString["@_IdOfItem"]);

                if (command.ExecuteNonQuery() != 1)
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось получить предмет!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.@string = "Данные изменены!";
                    return dataForResponse;
                }
            }
        }

        public static DataForMessage CreateChest(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_CreateChest", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_name", dataForMessage.dictionaryStringString["@_name"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "OK";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Пустой запрос";
                    return dataForResponse;
                }
            }
        }

        //
        public static DataForMessage RemoveChest(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_name"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_RemoveChest", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_name", dataForMessage.dictionaryStringString["@_name"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "OK";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Пустой запрос";
                    return dataForResponse;
                }
            }
        }








        //////////////////////////////////////////////////////////////////
        ///             НАСТРОЙКИ
        //////////////////////////////////////////////////////////////////

        //Проверка логина
        public static DataForMessage ErrorLogin(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@login"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@login"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                //Проверка логина
                var command = new SqlCommand("SP_IsErrorLogin", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@login", dataForMessage.dictionaryStringString["@login"]);

                using (var dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        dataForResponse.@string = "";
                        return dataForResponse;
                    }
                }

                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "";
                return dataForResponse;
            }
        }

        //Изменение логина
        public static DataForMessage ChangeLogin(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@login") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nowLogin"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@login"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_ChangeLogin", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@login", dataForMessage.dictionaryStringString["@login"]);
                command.Parameters.AddWithValue("@nowLogin", dataForMessage.dictionaryStringString["@nowLogin"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "Данные изменены!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось изменить данные!";
                    return dataForResponse;
                }
            }
        }

        //Проверка ника
        public static DataForMessage ErrorNick(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nick"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@nick"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный ник!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                //Проверка ника
                var command = new SqlCommand("SP_IsErrorNick", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@nick", dataForMessage.dictionaryStringString["@nick"]);

                using (var dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        dataForResponse.@string = "";
                        return dataForResponse;
                    }
                }

                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "";
                return dataForResponse;
            }
        }

        //Изменение ника
        public static DataForMessage ChangeNick(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nick") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nowLogin"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@nick"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный ник!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_ChangeNick", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@nick", dataForMessage.dictionaryStringString["@nick"]);
                command.Parameters.AddWithValue("@nowLogin", dataForMessage.dictionaryStringString["@nowLogin"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "Данные изменены!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось изменить данные!";
                    return dataForResponse;
                }
            }
        }

        //Изменение кредитной карты
        public static DataForMessage ChangeCredit(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_newCredit") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nowLogin"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            Regex reg = new Regex(@"^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$");
            if (!reg.IsMatch(dataForMessage.dictionaryStringString["@_newCredit"]))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Введите верный номер карты!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_ChangeCredit", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);

                if (dataForMessage.dictionaryStringString["@_newCredit"] == "DBNull.Value")
                {
                    command.Parameters.AddWithValue("@_newCredit", DBNull.Value);
                    command.Parameters.AddWithValue("@nowLogin", dataForMessage.dictionaryStringString["@nowLogin"]);
                }
                else
                {
                    command.Parameters.AddWithValue("@_newCredit", dataForMessage.dictionaryStringString["@_newCredit"]);
                    command.Parameters.AddWithValue("@nowLogin", dataForMessage.dictionaryStringString["@nowLogin"]);
                }

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "Данные изменены!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось изменить данные!";
                    return dataForResponse;
                }
            }
        }

        //Изменение количества золота
        public static DataForMessage ChangeGold(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_countOfGold") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nowLogin"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            foreach (var c in dataForMessage.dictionaryStringString["@_countOfGold"])
            {
                if (!Char.IsNumber(c))
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Требуется число!";
                    return dataForResponse;
                }
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_ChangeGold", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_countOfGold", dataForMessage.dictionaryStringString["@_countOfGold"]);
                command.Parameters.AddWithValue("@nowLogin", dataForMessage.dictionaryStringString["@nowLogin"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "Данные изменены!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось изменить данные!";
                    return dataForResponse;
                }
            }
        }

        //Изменение уровня
        public static DataForMessage ChangeLevel(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_level") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nowLogin"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            foreach (var c in dataForMessage.dictionaryStringString["@_level"])
            {
                if (!Char.IsNumber(c))
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Требуется число!";
                    return dataForResponse;
                }
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_ChangeLevel", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_level", dataForMessage.dictionaryStringString["@_level"]);
                command.Parameters.AddWithValue("@nowLogin", dataForMessage.dictionaryStringString["@nowLogin"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "Данные изменены!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось изменить данные!";
                    return dataForResponse;
                }
            }
        }

        //Изменение прав администратора
        public static DataForMessage ChangeRole(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_role") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nowLogin"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_ChangeRole", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_role", dataForMessage.dictionaryStringString["@_role"]);
                command.Parameters.AddWithValue("@nowLogin", dataForMessage.dictionaryStringString["@nowLogin"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "Данные изменены!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось изменить данные!";
                    return dataForResponse;
                }
            }
        }

        //Изменение пароля
        public static DataForMessage ChangePassword(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@_newPassword") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nowLogin"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@_newPassword"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный пароль!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_ChangePassword", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@_newPassword", dataForMessage.dictionaryStringString["@_newPassword"]);
                command.Parameters.AddWithValue("@nowLogin", dataForMessage.dictionaryStringString["@nowLogin"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "Данные изменены!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось изменить данные!";
                    return dataForResponse;
                }
            }
        }

        //Изменение даты последнего онлайна
        public static DataForMessage ChangeDateOfLastOnline(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@dateOfLastOnline") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@nowLogin"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();
                SqlCommand command;

                command = new SqlCommand("SP_ChangeDateOfLastOnline", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);
                command.Parameters.AddWithValue("@dateOfLastOnline", dataForMessage.dictionaryStringString["@dateOfLastOnline"]);
                command.Parameters.AddWithValue("@nowLogin", dataForMessage.dictionaryStringString["@nowLogin"]);

                if (command.ExecuteNonQuery() == 1)
                {
                    dataForResponse.@string = "Данные изменены!";
                    return dataForResponse;
                }
                else
                {
                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Не удалось изменить данные!";
                    return dataForResponse;
                }
            }
        }







        //////////////////////////////////////////////////////////////////
        ///             БЭКАП
        //////////////////////////////////////////////////////////////////

        public static DataForMessage GetDBConnectionString(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null ||
                !dataForMessage.dictionaryStringString.ContainsKey("@loginFrom") ||
                !dataForMessage.dictionaryStringString.ContainsKey("@passwordFrom"))
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            if (dataForMessage.dictionaryStringString["@loginFrom"].Length >= 50
                || dataForMessage.dictionaryStringString["@passwordFrom"].Length >= 50)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Слишком длинный логин или пароль отправителя!";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);

            using (var sqlConnection = new SqlConnection(DBConnectionString))
            {
                sqlConnection.Open();

                var command = new SqlCommand("SP_GetDBConnectionString", sqlConnection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@loginFrom", dataForMessage.dictionaryStringString["@loginFrom"]);
                command.Parameters.AddWithValue("@passwordFrom", dataForMessage.dictionaryStringString["@passwordFrom"]);

                using (var dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        if((bool) dataReader["Role"])
                        {
                            dataForResponse.@string = (string)DBConnectionString.Clone();
                            return dataForResponse;
                        }
                        else
                        {
                            dataForResponse.SetDataType(DataForMessage.DataType.Error);
                            dataForResponse.@string = "У вас нет прав администратора!";
                            return dataForResponse;
                        }
                    }

                    dataForResponse.SetDataType(DataForMessage.DataType.Error);
                    dataForResponse.@string = "Такого пользователя нет!";
                    return dataForResponse;
                }
            }
        }










        public static DataForMessage GetEmail(DataForMessage dataForMessage)
        {
            DataForMessage dataForResponse = new DataForMessage();

            if (dataForMessage is null)
            {
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = "Пустой запрос";
                return dataForResponse;
            }

            dataForResponse.SetDataType(DataForMessage.DataType.String);
            dataForResponse.@string = "Ops2nRGC";

            return dataForResponse;
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
