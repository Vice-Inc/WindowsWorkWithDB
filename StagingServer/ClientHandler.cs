using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using System.Timers;

namespace StagingServer
{
    class ClientHandler
    {
        private Socket handler;
        private Thread thread;
        private int errorsCount;

        public bool finished;
        public long lastUsedTime;

        public ClientHandler(Socket socket)
        {
            handler = socket;

            finished = false;

            thread = new Thread(() => this.Start());
            thread.Start();

            lastUsedTime = DateTime.Now.Ticks;

            errorsCount = 0;
        }

        public void Stop()
        {
            thread.Abort();
        }

        public void Start()
        {
            while (true)
            {
                ///////////////////////////////////////
                //////////// Этап 1: принимаем размер сообщения
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байтов
                byte[] data = new byte[256]; // буфер для получаемых данных

                try
                {
                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);
                }
                catch (SocketException ex)
                {
                    finished = true;
                    return;
                }

                lastUsedTime = DateTime.Now.Ticks;
                int size = 0;

                try
                {
                    size = int.Parse(builder.ToString());
                    size++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": Не удалось получить размер сообщения!");
                    errorsCount++;

                    if(errorsCount > 10)
                    {
                        finished = true;
                        return;
                    }

                    continue;
                    //handler.Shutdown(SocketShutdown.Both);
                    //handler.Close();
                    //finished = true;
                    //return;
                }

                errorsCount = 0;
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(DateTime.Now.ToShortTimeString() + ": Размер сообщения = " + builder.ToString());

                ///////////////////////////////////////
                //////////// Этап 2: отправляем подтверждение
                string message = "GO";
                data = Encoding.Unicode.GetBytes(message);
                handler.Send(data);

                ///////////////////////////////////////
                //////////// Этап 3: прием SQL комманды
                builder = new StringBuilder();
                bytes = 0; // количество полученных байтов
                data = new byte[size]; // буфер для получаемых данных

                do
                {
                    bytes = handler.Receive(data);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (handler.Available > 0);

                DataForMessage dataForMessage = JsonSerializer.Deserialize<DataForMessage>(builder.ToString());
                Console.WriteLine(DateTime.Now.ToShortTimeString() + ": Получено : " + builder.ToString());
                Console.WriteLine(DateTime.Now.ToShortTimeString() + ": Получено : " + dataForMessage.ToString());

                ///////////////////////////////////////
                //////////// Этап 4: выполнение SQL комманды
                DataForMessage dataForResponse = Do(dataForMessage);
                string json = JsonSerializer.Serialize<DataForMessage>(dataForResponse);
                byte[] bytesdataForResponse = Encoding.Unicode.GetBytes(json);

                ///////////////////////////////////////
                //////////// Этап 5: отправка размера будущего сообщения
                message = bytesdataForResponse.Length.ToString();
                data = Encoding.Unicode.GetBytes(message);
                handler.Send(data);

                ///////////////////////////////////////
                //////////// Этап 6: получаем подтверждение
                data = new byte[256]; // буфер для ответа
                builder = new StringBuilder();
                bytes = 0; // количество полученных байт

                do
                {
                    bytes = handler.Receive(data);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (handler.Available > 0);


                if (builder.ToString() != "GO")
                {
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": Клиент не разрешил отправку!");
                    continue;
                    //handler.Shutdown(SocketShutdown.Both);
                    //handler.Close();
                    //finished = true;
                    //return;
                }

                ///////////////////////////////////////
                //////////// Этап 7: отправка ответа
                Console.WriteLine(DateTime.Now.ToShortTimeString() + ": Отправляю : " + json);
                Console.WriteLine(DateTime.Now.ToShortTimeString() + ": Отправляю " + dataForResponse.ToString());
                handler.Send(bytesdataForResponse);

                // закрываем сокет
                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
                //finished = true;
            }
        }







        private static DataForMessage Do(DataForMessage dataForMessage)
        {
            if (dataForMessage.@string is null)
            {
                DataForMessage dataForResponse = new DataForMessage();
                dataForResponse.datatype = DataForMessage.DataType.Error;
                dataForResponse.@string = "@string is null";
                return dataForResponse;
            }

            try
            {
                ///             ВХОД
                if (dataForMessage.@string == "SP_GetRole")
                {
                    return DataBaseInterface.GetRole(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetLoginByNick")
                {
                    return DataBaseInterface.GetLoginByNick(dataForMessage);
                }
                ///             Регистрация
                else if (dataForMessage.@string == "SP_CheckUser")
                {
                    return DataBaseInterface.CheckUser(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_Registation")
                {
                    return DataBaseInterface.Registation(dataForMessage);
                }
                ///             ВЫСТАВЛЕНИЕ МЕТКИ ОНЛАЙНА
                else if (dataForMessage.@string == "SP_IsPlayerOnline")
                {
                    return DataBaseInterface.PlayerOnline(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_SetPlayerOnline")
                {
                    return DataBaseInterface.SetPlayerOnline(dataForMessage);
                }
                ///             ГЛАВНАЯ СТРАНИЦА
                else if (dataForMessage.@string == "SP_GetPageInfo")
                {
                    return DataBaseInterface.GetPageInfo(dataForMessage);
                }
                ///             ДРУЗЬЯ
                else if (dataForMessage.@string == "SP_AddFriend")
                {
                    return DataBaseInterface.AddFriend(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_AddSearchFriend")
                {
                    return DataBaseInterface.AddSearchFriend(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetTopPlayers")
                {
                    return DataBaseInterface.GetTopPlayers(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetAllPlayers")
                {
                    return DataBaseInterface.GetAllPlayers(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetFriends")
                {
                    return DataBaseInterface.GetFriends(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetPresentItem")
                {
                    return DataBaseInterface.GetPresentItem(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetPresent")
                {
                    return DataBaseInterface.GetPresent(dataForMessage);
                }
                ///             ПРЕДМЕТЫ
                else if (dataForMessage.@string == "SP_GetItems")
                {
                    return DataBaseInterface.GetItems(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetAllItems")
                {
                    return DataBaseInterface.GetAllItems(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetItemInfo")
                {
                    return DataBaseInterface.GetItemInfo(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_AddItemToPlayer")
                {
                    return DataBaseInterface.AddItemToPlayer(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_DeleteItemFromPlayer")
                {
                    return DataBaseInterface.DeleteItemFromPlayer(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_ChangeItemInfo")
                {
                    return DataBaseInterface.ChangeItemInfo(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_CreateItem")
                {
                    return DataBaseInterface.CreateItem(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_RemoveItem")
                {
                    return DataBaseInterface.RemoveItem(dataForMessage);
                }
                ///             СУНДУКИ
                else if (dataForMessage.@string == "SP_GetChests")
                {
                    return DataBaseInterface.GetChests(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetAllChests")
                {
                    return DataBaseInterface.GetAllChests(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetChestInfo")
                {
                    return DataBaseInterface.GetChestInfo(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_AddChestToPlayer")
                {
                    return DataBaseInterface.AddChestToPlayer(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_DeleteChestFromPlayer")
                {
                    return DataBaseInterface.DeleteChestFromPlayer(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_ChangeChestInfo")
                {
                    return DataBaseInterface.ChangeChestInfo(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetLootItem")
                {
                    return DataBaseInterface.GetLootItem(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetLootOfChest")
                {
                    return DataBaseInterface.GetLootOfChest(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_CreateChest")
                {
                    return DataBaseInterface.CreateChest(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_RemoveChest")
                {
                    return DataBaseInterface.RemoveChest(dataForMessage);
                }
                ///             НАСТРОЙКИ
                else if (dataForMessage.@string == "SP_IsErrorLogin")
                {
                    return DataBaseInterface.ErrorLogin(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_ChangeLogin")
                {
                    return DataBaseInterface.ChangeLogin(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_IsErrorNick")
                {
                    return DataBaseInterface.ErrorNick(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_ChangeNick")
                {
                    return DataBaseInterface.ChangeNick(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_ChangeCredit")
                {
                    return DataBaseInterface.ChangeCredit(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_ChangeGold")
                {
                    return DataBaseInterface.ChangeGold(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_ChangeLevel")
                {
                    return DataBaseInterface.ChangeLevel(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_ChangeRole")
                {
                    return DataBaseInterface.ChangeRole(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_ChangePassword")
                {
                    return DataBaseInterface.ChangePassword(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_ChangeDateOfLastOnline")
                {
                    return DataBaseInterface.ChangeDateOfLastOnline(dataForMessage);
                }
                ///             БЭКАП
                else if (dataForMessage.@string == "SP_GetDBConnectionString")
                {
                    return DataBaseInterface.GetDBConnectionString(dataForMessage);
                }
                else if (dataForMessage.@string == "SP_GetEmail")
                {
                    return DataBaseInterface.GetEmail(dataForMessage);
                }
                ///             ОШИБКА
                else
                {
                    DataForMessage dataForResponse = new DataForMessage();
                    dataForResponse.datatype = DataForMessage.DataType.Error;
                    dataForResponse.@string = "Invalid command";
                    return dataForResponse;
                }
            }
            catch (Exception ex)
            {
                DataForMessage dataForResponse = new DataForMessage();
                dataForResponse.SetDataType(DataForMessage.DataType.Error);
                dataForResponse.@string = ex.Message;
                return dataForResponse;
            }
        }






    }
}
