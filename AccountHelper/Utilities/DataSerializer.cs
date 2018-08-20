using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AccountHelper.Utilities
{
    static class DataSerializer
    {
        #region Byte Array
        public static byte[] SerializeToByteArray<T>(T obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
        public static byte[] DeserializeFromByteArray<T>(byte[] array)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            stream.Write(array, 0, array.Length);
            stream.Position = 0;
            return stream.ToArray();
        }
        #endregion

        #region JSON
        //internal static string SerializeToJson<T>(T obj)
        //{
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    return serializer.Serialize(obj);
        //}
        //internal static T DeserializeFromJson<T>(string data)
        //{
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    return (T)serializer.Deserialize(data, typeof(T));
        //}
        #endregion

        #region XML
        internal static string SerializeToXml<T>(T obj)
        {
            XDocument doc = new XDocument();
            using (XmlWriter writer = doc.CreateWriter())
            {
                writer.WriteStartElement("SaveData");
                new XmlSerializer(typeof(T)).Serialize(writer, obj);
            }
            return doc.ToString();
        }
        internal static T DeserializeFromXml<T>(string data) where T : class
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            XDocument doc = XDocument.Parse(data);
            return (T)deserializer.Deserialize(doc.Elements().Elements().First().CreateReader());
        }
        #endregion

        #region CSV
        internal static string SerializeToCsv<T>(IEnumerable<T> objects)
        {
            StringBuilder csvData = new StringBuilder();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                csvData.Append($"\"{property.Name}\",");
            }
            csvData.Append("\r\n");
            foreach (T writeObject in objects)
            {
                foreach (PropertyInfo property in properties)
                {
                    csvData.Append($"\"{property.GetValue(writeObject)}\",");
                }
                csvData.Append("\r\n");
            }
            return csvData.ToString();
        }
        internal static string SerializeToCsvUsingDisplayAttributes<T>(IEnumerable<T> objects)
        {
            StringBuilder csvData = new StringBuilder();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                MemberInfo propertyInfo = typeof(T).GetProperty(property.Name);
                DisplayNameAttribute attribute = propertyInfo.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                if (attribute != null)
                {
                    csvData.Append($"\"{attribute.DisplayName}\",");
                }
                else
                {
                    throw new NullReferenceException($"DisplayNameAttribute not set for {property.Name}");
                }
            }
            csvData.Append("\r\n");
            foreach (T writeObject in objects)
            {
                foreach (PropertyInfo property in properties)
                {
                    csvData.Append($"\"{property.GetValue(writeObject)}\";");
                }
                csvData.Append("\r\n");
            }
            return csvData.ToString();
        }
        public static IEnumerable<T> DeserializeFromCsv<T>(string data)
        {
            return null;
        }
        public static IEnumerable<T> DeserializeFromCsvUsingDisplayAttributes<T>(string text, char cellDelimiter = ',')
        {
            List<T> returnObjects = new List<T>();
            if (string.IsNullOrEmpty(text))
            {
                throw new NullReferenceException("Parameter 'text' does not point to an instance of an object.");
            }
            List<CsvPropertyHelper> propertyHelpers = new List<CsvPropertyHelper>();
            string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] header = lines[0].Split(cellDelimiter);
            //Match property display names to headers:
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                MemberInfo propertyDetails = typeof(T).GetProperty(property.Name);
                // Only work with writeable properties.
                if (propertyDetails.GetCustomAttribute(typeof(DisplayNameAttribute)) is DisplayNameAttribute attribute && property.CanWrite)
                {
                    CsvPropertyHelper helper = new CsvPropertyHelper();
                    helper.Property = property;
                    helper.DisplayName = attribute.DisplayName;
                    int colInd = 0;
                    bool found = false;
                    while (!found && colInd < header.Length)
                    {
                        if (helper.DisplayName == header[colInd])
                        {
                            helper.CsvColumn = colInd;
                            propertyHelpers.Add(helper);
                            found = true;
                        }
                        colInd++;
                    }
                }
                else
                {
                    Logger.Log($"DisplayNameAttribute not set for {property.Name}");
                }
            }
            for (int i = 1; i < lines.Length; i++)
            {
                string[] columns = lines[i].Split(',');
                T obj = Activator.CreateInstance<T>();
                foreach (CsvPropertyHelper helper in propertyHelpers)
                {
                    helper.Property.SetValue(obj, columns[helper.CsvColumn]);
                }
                returnObjects.Add(obj);
            }
            return returnObjects;
        }
        internal class CsvPropertyHelper
        {
            public string DisplayName { get; set; }
            public PropertyInfo Property { get; set; }
            public int CsvColumn { get; set; }
        }
        public static DataTable DeserializeCsvToDataTable(string text, char cellDelimiter = ',', string lineDelimiter = "\n")
        {
            DataTable dataTable = new DataTable();
            int headerCutoff = text.IndexOf(lineDelimiter);
            string[] columnNames = text.Substring(0, headerCutoff).Split(cellDelimiter);
            foreach (string columnName in columnNames)
            {
                dataTable.Columns.Add(columnName);
            }
            string data = text.Substring(headerCutoff + lineDelimiter.Length);
            string[] lines = data.Split(new string[] { lineDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                DataRow row = dataTable.NewRow();
                string[] columns = line.Split(cellDelimiter);
                int colInd = 0;
                foreach (string column in columns)
                {
                    row[colInd] = column;
                    colInd++;
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
        #endregion
    }
}
