using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoginBd
{
    public static class StagingServerInterface
    {
        // адрес и порт сервера, к которому будем подключаться
        static int port = 8005; // порт сервера
        static string address = "127.0.0.1"; // адрес сервера

        //////////////////////////////////////////////////////////////////
        ///             ОТПРАВКА СООБЩЕНИЙ ПРОМЕЖУТОЧНОМУ ПО
        //////////////////////////////////////////////////////////////////

        public static DataForMessage Send(DataForMessage dataForMessage)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);

                string json = JsonSerializer.Serialize<DataForMessage>(dataForMessage);
                byte[] bytesDataForMessage = Encoding.Unicode.GetBytes(json);

                ///////////////////////////////////////
                //////////// Этап 1: отправка размера сообщения
                string message = bytesDataForMessage.Length.ToString();
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);

                ///////////////////////////////////////
                //////////// Этап 2: получаем подтверждение
                data = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);


                if(builder.ToString() != "GO")
                {
                    dataForMessage.datatype = DataForMessage.DataType.Error;
                    dataForMessage.@string = "Сервер не разрешил отправку ему комманды!";
                    return dataForMessage;
                }

                ///////////////////////////////////////
                //////////// Этап 3: отправка SQL комманды
                socket.Send(bytesDataForMessage);

                ///////////////////////////////////////
                //////////// Этап 4: получаем размер будущего сообщения
                data = new byte[256]; // буфер для ответа
                builder = new StringBuilder();
                bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);


                int size = 0;
                dataForMessage = new DataForMessage();

                try
                {
                    size = int.Parse(builder.ToString());
                    size++;
                }
                catch (Exception ex)
                {
                    dataForMessage.datatype = DataForMessage.DataType.Error;
                    dataForMessage.@string = "Не удалось получить размер сообщения!";
                    return dataForMessage;
                }

                ///////////////////////////////////////
                //////////// Этап 5: отправляем сообщение о готовсти принять ответ сервера
                json = "GO";
                data = Encoding.Unicode.GetBytes(json);
                socket.Send(data);

                ///////////////////////////////////////
                //////////// Этап 6: получаем ответ с сервера
                data = new byte[size]; // буфер для ответа
                builder = new StringBuilder();
                bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);

                dataForMessage = JsonSerializer.Deserialize<DataForMessage>(builder.ToString());

                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                return dataForMessage;
            }
            catch (Exception ex)
            {
                dataForMessage.datatype = DataForMessage.DataType.Error;
                dataForMessage.@string = ex.Message;
                return dataForMessage;
            }
        }



    }
}
