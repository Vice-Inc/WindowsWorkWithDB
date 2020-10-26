using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginBd
{
    [Serializable]
    public class DataForMessage
    {
        public enum DataType
        {
            dictionaryStringString, listString, dictionaryStringListPairStringBool, dictionaryStringInt,
            String, Command, Error
        };

        public Dictionary<string, string> dictionaryStringString { get; set; }
        public List<string> listString { get; set; }
        public Dictionary<string, List<KeyValuePair<string, bool>>> dictionaryStringListPairStringBool { get; set; }
        public Dictionary<string, int> dictionaryStringInt { get; set; }
        public string @string { get; set; }
        public DataType datatype { get; set; }

        public DataForMessage()
        {
            datatype = DataType.String;
        }

        public void SetDataType(DataType _datatype)
        {
            switch (_datatype)
            {
                case DataType.dictionaryStringString:
                    dictionaryStringString = new Dictionary<string, string>();
                    break;
                case DataType.listString:
                    listString = new List<string>();
                    break;
                case DataType.dictionaryStringListPairStringBool:
                    dictionaryStringListPairStringBool = new Dictionary<string, List<KeyValuePair<string, bool>>>();
                    break;
                case DataType.dictionaryStringInt:
                    dictionaryStringInt = new Dictionary<string, int>();
                    break;
                case DataType.String: break;
                case DataType.Command:
                    dictionaryStringString = new Dictionary<string, string>();
                    break;
                case DataType.Error: break;
            }

            datatype = _datatype;
        }

        public override string ToString()
        {
            switch (datatype)
            {
                case DataType.dictionaryStringString:
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append("Message: ");
                        if(!(@string is null)) builder.Append(@string);
                        builder.Append(Environment.NewLine);
                        builder.Append("Dictionary:");
                        builder.Append(Environment.NewLine);

                        if (!(dictionaryStringString is null))
                        {
                            foreach (var item in dictionaryStringString)
                            {
                                builder.Append("\t" + item.Key + " : " + item.Value + Environment.NewLine);
                            }
                        }

                        return builder.ToString();
                    }
                case DataType.listString:
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append("Message: ");
                        if (!(@string is null)) builder.Append(@string);
                        builder.Append(Environment.NewLine);
                        builder.Append("List:");
                        builder.Append(Environment.NewLine);

                        if (!(listString is null))
                        {
                            foreach (var item in listString)
                            {
                                builder.Append("\t" + item + Environment.NewLine);
                            }
                        }

                        return builder.ToString();
                    }
                case DataType.dictionaryStringListPairStringBool:
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append("Message: ");
                        if (!(@string is null)) builder.Append(@string);
                        builder.Append(Environment.NewLine);
                        builder.Append("Dictionary:");
                        builder.Append(Environment.NewLine);

                        if (!(dictionaryStringListPairStringBool is null))
                        {
                            foreach (var item in dictionaryStringListPairStringBool)
                            {
                                builder.Append("\t" + item.Key + Environment.NewLine);
                                foreach (var item_ in item.Value)
                                {
                                    builder.Append("\t\t" + item_.Key + " : " + item_.Value + Environment.NewLine);
                                }
                            }
                        }

                        return builder.ToString();
                    }
                case DataType.dictionaryStringInt:
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append("Message: ");
                        if (!(@string is null)) builder.Append(@string);
                        builder.Append(Environment.NewLine);
                        builder.Append("Dictionary:");
                        builder.Append(Environment.NewLine);

                        if (!(dictionaryStringInt is null))
                        {
                            foreach (var item in dictionaryStringInt)
                            {
                                builder.Append("\t" + item.Key + " : " + item.Value.ToString() + Environment.NewLine);
                            }
                        }

                        return builder.ToString();
                    }
                case DataType.String: return @string;
                case DataType.Command:
                    {
                        StringBuilder builder = new StringBuilder();

                        if (!(dictionaryStringString is null) && dictionaryStringString.ContainsKey("@loginFrom"))
                        {
                            builder.Append("Sender of message: ");
                            builder.Append(dictionaryStringString["@loginFrom"]);
                            builder.Append(Environment.NewLine);
                        }

                        builder.Append("Command: ");
                        if (!(@string is null)) builder.Append(@string);
                        builder.Append(Environment.NewLine);
                        builder.Append("Args:");
                        builder.Append(Environment.NewLine);

                        if (!(dictionaryStringString is null))
                        {
                            foreach (var item in dictionaryStringString)
                            {
                                builder.Append("\t" + item.Key + " : " + item.Value + Environment.NewLine);
                            }
                        }

                        return builder.ToString();
                    }
                case DataType.Error: return @string;
            }

            return "<>";
        }



    }
}
