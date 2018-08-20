using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace AccountHelper.Utilities
{
    public static class Logger
    {
        public enum EventType
        {
            Start,
            Comment,
            Exception,
            Stop
        }

        private static ILogger _logger;
        public static ILogger implementation { get { return _logger ?? (_logger = new FileLogger()); } set { _logger = value; } }
        public static void Start()
        {
            implementation.Start();
        }
        public static void Log(string message, [CallerMemberName] string memberName = "")
        {
            implementation.Log(message, memberName);
        }
        public static void LogException(Exception ex, [CallerMemberName] string memberName = "")
        {
            implementation.LogException(ex, memberName);
        }
        public static void Stop()
        {
            implementation.Stop();
        }
    }

    public interface ILogger
    {
        void Start();
        void Log(string message, string callingMethod);
        void LogException(Exception ex, string callingMethod);
        void Stop();
        void Delete(int days);
    }

    public class FileLogger : ILogger
    {
        private static object _lock;
        private readonly string _file = string.Concat(DateTime.Today, ".txt");
        private string _saveDirectory;
        private string _saveFile;
        public FileLogger()
        {
            _saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(_saveDirectory);
            _saveFile = Path.Combine(_saveDirectory, string.Concat(DateTime.Today.ToString("yyyy-MM-dd"), ".txt"));
        }
        public FileLogger(string directory)
        {
            _saveDirectory = directory;
        }
        public void SetDirectory(string path, bool absolute)
        {
            if (absolute)
            {
                _saveDirectory = path;
            }
            else
            {
                _saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }
            _saveFile = Path.Combine(_saveDirectory, string.Concat(DateTime.Today.ToString("yyyy-MM-dd"), ".txt"));
            Directory.CreateDirectory(_saveDirectory);
        }
        public void Start()
        {
            using (StreamWriter writer = File.AppendText(_saveFile))
            {
                writer.WriteLineAsync(string.Concat(DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"), ", Beginning logging"));
            }

        }
        public void Log(string message, string memberName)
        {
            using (StreamWriter writer = File.AppendText(_saveFile))
            {
                writer.WriteLineAsync(string.Concat(DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"), ", ", memberName, ", ", message));
            }
        }
        public void LogException(Exception ex, string memberName)
        {
            using (StreamWriter writer = File.AppendText(_saveFile))
            {
                writer.WriteLineAsync(string.Concat(DateTime.Now.ToString("yyyy-MM-ddTHH-mm-s"), ", ", memberName, ", ", ex.ToString()));
            }
        }
        public void Stop()
        {
            using (StreamWriter writer = File.AppendText(_saveFile))
            {
                writer.WriteLineAsync(string.Concat(DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"), ", Ending logging"));
            }
        }
        public void Delete(int days)
        {
            foreach (string file in Directory.GetFiles(_saveDirectory))
            {
                if (File.GetCreationTime(file) < DateTime.Now.AddDays(-days))
                {
                    File.Delete(file);
                }
            }
        }
    }

    //public class SqlDatabaseLogger : ILogger
    //{
    //    public string ConnectionString { get; set; }
    //    public string LogTable { get; set; }
    //    public string MessageColumn { get; set; }
    //    public SqlDatabaseLogger(string messageColumn, string logTable, string connectionString)
    //    {
    //        MessageColumn = messageColumn;
    //        LogTable = logTable;
    //        ConnectionString = connectionString;
    //    }
    //    public void Start()
    //    {
    //        try
    //        {
    //            using (SqlConnection conn = new SqlConnection(ConnectionString))
    //            {
    //                conn.Open();
    //                string sql = $@"
    //                    INSERT INTO {LogTable}
    //                    ({MessageColumn})
    //                    VALUES
    //                    (@MESSAGE)";
    //                SqlCommand cmd = new SqlCommand(sql, conn);
    //                cmd.Parameters.Add("@MESSAGE", SqlDbType.VarChar).Value = string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), ":Logging started ");
    //                cmd.ExecuteNonQuery();
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            throw new Exception("Logger not set up properly.");
    //        }

    //    }
    //    public void WriteLine(string message)
    //    {
    //        try
    //        {
    //            using (SqlConnection conn = new SqlConnection(ConnectionString))
    //            {
    //                conn.Open();
    //                string sql = $@"
    //                    INSERT INTO {LogTable}
    //                    ({MessageColumn})
    //                    VALUES
    //                    (@MESSAGE)";
    //                SqlCommand cmd = new SqlCommand(sql, conn);
    //                cmd.Parameters.Add("@MESSAGE", SqlDbType.VarChar).Value = string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), ":", message);
    //                cmd.ExecuteNonQuery();
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            throw new Exception("Logger not set up properly.");
    //        }
    //    }
    //    public void WriteException(Exception ex)
    //    {
    //        try
    //        {
    //            using (SqlConnection conn = new SqlConnection(ConnectionString))
    //            {
    //                conn.Open();
    //                string sql = $@"
    //                    INSERT INTO {LogTable}
    //                    ({MessageColumn})
    //                    VALUES
    //                    (@MESSAGE)";
    //                SqlCommand cmd = new SqlCommand(sql, conn);
    //                cmd.Parameters.Add("@MESSAGE", SqlDbType.VarChar).Value = string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), ":", ex.ToString());
    //                cmd.ExecuteNonQuery();
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            throw new Exception("Logger not set up properly.");
    //        }
    //    }
    //    public void Stop()
    //    {
    //        try
    //        {
    //            using (SqlConnection conn = new SqlConnection(ConnectionString))
    //            {
    //                conn.Open();
    //                string sql = $@"
    //                    INSERT INTO {LogTable}
    //                    ({MessageColumn})
    //                    VALUES
    //                    (@MESSAGE)";
    //                SqlCommand cmd = new SqlCommand(sql, conn);
    //                cmd.Parameters.Add("@MESSAGE", SqlDbType.VarChar).Value = string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), ":Logging ended ");
    //                cmd.ExecuteNonQuery();
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            throw new Exception("Logger not set up properly.");
    //        }
    //    }
    //    public void Delete(int days)
    //    {
    //        try
    //        {
    //            using (SqlConnection conn = new SqlConnection(ConnectionString))
    //            {
    //                conn.Open();
    //                string sql = $@"
    //                    DELETE FROM {LogTable}
    //                    WHERE {MessageColumn} LIKE '{DateTime.Today.AddDays(-days).ToString("yyyy-MM-dd")}%";
    //                SqlCommand cmd = new SqlCommand(sql, conn);
    //                cmd.ExecuteNonQuery();
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            throw new Exception("Logger not set up properly.");
    //        }
    //    }
    //}
}
