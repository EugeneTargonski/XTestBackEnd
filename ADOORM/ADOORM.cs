using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ADOORM
{
    public class ORMLight<T>
    {
        private readonly string connectionString;
        private readonly Type objectType;
        private readonly string baseName;
        private readonly string tableName;
        private readonly ILogger logger;

        public ORMLight(string _connectionString, string _baseName, ILogger _logger = null)
        {
            connectionString = _connectionString;
            logger = _logger;
            objectType = typeof(T);
            tableName = objectType.Name.ToString()+"Table";
            baseName = _baseName;
            CreateBase();
            CreateFields();
        }
        //Parameterization not used. User havn't access to property names.
        public List<T> GetRecords()
        {
            List<T> ReadedList = new List<T>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = "SELECT * FROM " + tableName;
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        object Readed = Activator.CreateInstance(objectType);
                        var properties = objectType.GetProperties();
                        foreach (var property in properties)
                        {
                            ConvertFromSQLType(property, Readed, reader[property.Name]);
                        }
                        ReadedList.Add((T)Readed);
                    }
                }
                reader.Close();
            }
            return ReadedList;
        }
        //TODO non-ended
        public List<T> GetRecordsByStringValue(string propertyName, string value)
        {
            List<T> ReadedList = new List<T>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = "SELECT * FROM " + tableName;
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        object Readed = Activator.CreateInstance(objectType);
                        var properties = objectType.GetProperties();
                        foreach (var property in properties)
                        {
                            ConvertFromSQLType(property, Readed, reader[property.Name]);
                        }
                        ReadedList.Add((T)Readed);
                    }
                }
                reader.Close();
            }
            return ReadedList;
        }
        public Dictionary<object, int> GetAllRecordsGroupBy(string propertyName)
        {
            Dictionary<object, int> ReadedList = new Dictionary<object, int>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = "SELECT " + propertyName + ", COUNT(*) AS CNT FROM " + tableName
                                        + " GROUP BY " + propertyName;
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ReadedList.Add(reader[propertyName], (int)reader["CNT"]);
                    }
                }
                reader.Close();
            }
            return ReadedList;
        }
        public Dictionary<object, int> GetAllRecordsGroupBy(string propertyName1, string propertyName2)
        {
            Dictionary<object, int> ReadedList = new Dictionary<object, int>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = "SELECT " + propertyName1 + "," + propertyName2
                                        + ", COUNT(*) AS CNT FROM " + tableName
                                        + " GROUP BY " + propertyName1 + "," + propertyName2;
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ReadedList.Add((reader[propertyName1], reader[propertyName2]), (int)reader["CNT"]);
                    }
                }
                reader.Close();
            }
            return ReadedList;
        }
        public object Agregate(string propertyName, string agrFunction)
        {
            object Readed = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = "SELECT " + agrFunction + "(" + propertyName + ") FROM " + tableName;
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                Readed = command.ExecuteScalar();
            }
            return Readed;
        }
        //TODO refactor, sql inj is posible
        public object AgregateWithRestrictions(string propertyName, string agrFunction,
                                                    string restrictionName, object restriction11,
                                                    object restriction12,
                                                    string restrictionName2, object restriction21,
                                                    object restriction22, bool number = false)
        {
            object Readed = null;
            string quote = "'";
            if (number)
                quote = "";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = "SELECT " + agrFunction + "(" + propertyName + ") FROM " + tableName
                                     + " WHERE " + restrictionName + ">=" + quote + restriction11.ToString() + quote
                                     + " AND " + restrictionName + "<=" + quote + restriction12.ToString() + quote
                                     + " AND " + restrictionName2 + ">=" + quote + restriction21.ToString() + quote
                                     + " AND " + restrictionName2 + "<=" + quote + restriction22.ToString() + quote;
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                Readed = command.ExecuteScalar();
            }
            return Readed;
        }
        //TODO refactor, sql inj is posible
        public object AgregateWithRestrictions(string propertyName, string agrFunction,
                                                            string restrictionName, object restriction1,
                                                            object restriction2, bool number = false)
        {
            object Readed = null;
            string quote = "'";
            if (number)
                quote = "";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = "SELECT " + agrFunction + "(" + propertyName + ") FROM " + tableName
                                     + " WHERE " + restrictionName + ">=" + quote + restriction1.ToString() + quote
                                     + " AND " + restrictionName + "<=" + quote + restriction2.ToString() + quote;
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                Readed = command.ExecuteScalar();
            }
            return Readed;
        }
        public T Top1(string propertyName, bool asc = true)
        {
            object Readed = null;
            string order = " ASC";
            if (!asc)
                order = " DESC";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = "SELECT TOP(1) * FROM " + tableName + " ORDER BY " + propertyName + order;
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    Readed = Activator.CreateInstance(objectType);
                    while (reader.Read())
                    {
                        var properties = objectType.GetProperties();
                        foreach (var property in properties)
                        {
                            ConvertFromSQLType(property, Readed, reader[property.Name]);
                        }
                    }
                }
                reader.Close();
            }
            return (T)Readed;
        }
        public T GetRecords(int id)
        {
            object Readed = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = "SELECT * FROM " + tableName + " WHERE Id = @id";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlParameter idParam = new SqlParameter("@id", id);
                command.Parameters.Add(idParam);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    Readed = Activator.CreateInstance(objectType);
                    while (reader.Read())
                    {
                        var properties = objectType.GetProperties();
                        foreach (var property in properties)
                        {
                            ConvertFromSQLType(property, Readed, reader[property.Name]);
                        }
                    }
                }
                reader.Close();
            }
            return (T)Readed;
        }
        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = "DELETE " + tableName + " WHERE Id = @id";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlParameter idParam = new SqlParameter("@id", id);
                command.Parameters.Add(idParam);
                int number = command.ExecuteNonQuery();
            }
        }
        public void Save(object SavedObject)
        {
            int id = 0;
            try
            {
                id = (int)(SavedObject.GetType().GetProperty("Id").GetValue(SavedObject));
            }
            catch { }
            if (id == 0)
            { NewRecordSave(SavedObject); }
            else
            { ExistingRecordSave(SavedObject, id); }
        }
        private void ExistingRecordSave(object SavedObject, int id)
        {
            string sqlExpression = "UPDATE " + tableName + " SET ";
            string comma = "";
            var properties = SavedObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                sqlExpression = sqlExpression + comma + property.Name + "=@" + property.Name;
                comma = ", ";
            }
            sqlExpression += " WHERE Id=@id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlParameter AddParam;
                foreach (var property in properties)
                {
                    AddParam = new SqlParameter("@" + property.Name, property.GetValue(SavedObject) ?? DBNull.Value);
                    command.Parameters.Add(AddParam);
                }
                AddParam = new SqlParameter("@id", id);
                command.Parameters.Add(AddParam);
                command.ExecuteNonQuery();
            }
        }
        private void NewRecordSave(object SavedObject)
        {
            string sqlExpression = "INSERT INTO " + tableName + " (";
            string comma = "";
            var properties = SavedObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.Name == "Id") continue;
                sqlExpression = sqlExpression + comma + property.Name;
                comma = ",";
            }
            sqlExpression += ") VALUES (";
            comma = "@";
            foreach (var property in properties)
            {
                if (property.Name == "Id") continue;
                sqlExpression = sqlExpression + comma + property.Name;
                comma = ",@";
            }
            sqlExpression += ");SET @Id=SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlParameter AddParam;
                foreach (var property in properties)
                {
                    if (property.Name == "Id") continue;
                    AddParam = new SqlParameter("@" + property.Name, property.GetValue(SavedObject) ?? DBNull.Value);
                    command.Parameters.Add(AddParam);
                }
                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output // Output parameter
                };
                command.Parameters.Add(idParam);
                command.ExecuteNonQuery();
                try
                {
                    SavedObject.GetType().GetProperty("Id").SetValue(SavedObject, idParam.Value);
                }
                catch
                {
                }
            }
        }
        public void CreateFields()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //Create a structure the properties of a class in SQL 
                CreateTable();
                foreach (PropertyInfo propinfo in objectType.GetProperties())
                {
                    AddColumns(propinfo.Name, propinfo.PropertyType);
                }
                //END //Create a structure the properties of a class in SQL 
            }
        }
        private void CreateBase()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlExpression = "CREATE DATABASE " + baseName;
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                try
                {
                    command.ExecuteNonQuery();
                    logger?.LogInformation("Base " + baseName + " created");
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    logger?.LogError(e.Message);
                }
            }
        }
        private void ConvertFromSQLType(PropertyInfo property, object ChangedObject, object NewValue)
        {
            var PropertyType = property.PropertyType;
            if (PropertyType == typeof(string))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, Convert.ToString(NewValue));
            }
            else if (PropertyType == typeof(int?))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, Convert.ToInt32(NewValue));
            }
            else if (PropertyType == typeof(int))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, 0);
                else
                    property.SetValue(ChangedObject, Convert.ToInt32(NewValue));
            }
            else if (PropertyType == typeof(long?))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, Convert.ToInt64(NewValue));
            }
            else if (PropertyType == typeof(long))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, 0);
                else
                    property.SetValue(ChangedObject, Convert.ToInt64(NewValue));
            }
            else if (PropertyType == typeof(decimal?))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, Convert.ToDecimal(NewValue));
            }
            else if (PropertyType == typeof(decimal))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, 0.0m);
                else
                    property.SetValue(ChangedObject, Convert.ToDecimal(NewValue));
            }
            else if (PropertyType == typeof(DateTime?))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, Convert.ToDateTime(NewValue));
            }
            else if (PropertyType == typeof(DateTime))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, DateTime.MinValue);
                else
                    property.SetValue(ChangedObject, Convert.ToDateTime(NewValue));
            }
            else if (PropertyType == typeof(double?))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, Convert.ToDouble(NewValue));
            }
            else if (PropertyType == typeof(double))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, 0.0);
                else
                    property.SetValue(ChangedObject, Convert.ToDouble(NewValue));
            }
            else if (PropertyType == typeof(float?))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, Convert.ToSingle(NewValue));
            }
            else if (PropertyType == typeof(float))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, 0f);
                else
                    property.SetValue(ChangedObject, Convert.ToSingle(NewValue));
            }
            else if (PropertyType == typeof(short?))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, Convert.ToInt16(NewValue));
            }
            else if (PropertyType == typeof(short))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, 0);
                else
                    property.SetValue(ChangedObject, Convert.ToInt16(NewValue));
            }
            else if (PropertyType == typeof(byte?))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, Convert.ToByte(NewValue));
            }
            else if (PropertyType == typeof(byte))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, 0);
                else
                    property.SetValue(ChangedObject, Convert.ToByte(NewValue));
            }
            else if (PropertyType == typeof(bool?))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, Convert.ToBoolean(NewValue));
            }
            else if (PropertyType == typeof(bool))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, false);
                else
                    property.SetValue(ChangedObject, Convert.ToBoolean(NewValue));
            }
            else if (PropertyType == typeof(char?))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, Convert.ToChar(NewValue));
            }
            else if (PropertyType == typeof(char))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, ' ');
                else
                    property.SetValue(ChangedObject, Convert.ToChar(NewValue));
            }
            else if (PropertyType == typeof(byte[]))
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, null);
                else
                    property.SetValue(ChangedObject, (byte[])NewValue);
            }
            else
            {
                if (NewValue.GetType() == typeof(DBNull))
                    property.SetValue(ChangedObject, "");
                else
                    property.SetValue(ChangedObject, Convert.ToString(NewValue));
            }
        }
        private string GetSQLType(Type PropertyType)
        {
            if (PropertyType == typeof(string)) { return "nvarchar(MAX)"; }
            else if (PropertyType == typeof(int?)) { return "int"; }
            else if (PropertyType == typeof(int)) { return "int"; }
            else if (PropertyType == typeof(long?)) { return "bigint"; }
            else if (PropertyType == typeof(long)) { return "bigint"; }
            else if (PropertyType == typeof(decimal?)) { return "decimal"; }
            else if (PropertyType == typeof(decimal)) { return "decimal"; }
            else if (PropertyType == typeof(DateTime?)) { return "datetime"; }
            else if (PropertyType == typeof(DateTime)) { return "datetime"; }
            else if (PropertyType == typeof(double?)) { return "float"; }
            else if (PropertyType == typeof(double)) { return "float"; }
            else if (PropertyType == typeof(float?)) { return "real"; }
            else if (PropertyType == typeof(float)) { return "real"; }
            else if (PropertyType == typeof(short)) { return "smallint"; }
            else if (PropertyType == typeof(short)) { return "smallint"; }
            else if (PropertyType == typeof(byte?)) { return "tinyint"; }
            else if (PropertyType == typeof(byte)) { return "tinyint"; }
            else if (PropertyType == typeof(bool?)) { return "bit"; }
            else if (PropertyType == typeof(bool)) { return "bit"; }
            else if (PropertyType == typeof(char?)) { return "nchar(1)"; }
            else if (PropertyType == typeof(char)) { return "nchar(1)"; }
            else if (PropertyType == typeof(byte[])) { return "varbinary(max)"; }
            else return "nvarchar(MAX)";
        }
        private void AddColumns(string propertyName, Type PropertyType, ILogger logger = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //Type comparison
                string sqltype = GetSQLType(PropertyType);
                //End Type comparison
                try
                {
                    string sqlExpression = "ALTER TABLE " + tableName + " " +
                        "ADD " +
                        propertyName + " " + sqltype + " NULL";
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                    logger?.LogInformation("Field " + propertyName + " of type " + sqltype + " added");
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    logger?.LogError(e.Message);
                }
                catch (Exception e) { throw e; }
            }
        }
        private void CreateTable(ILogger logger = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    string sqlExpression = "CREATE TABLE " + tableName + " " +
                        "(" +
                        "id int IDENTITY(1,1) NOT FOR REPLICATION, " +
                        "CONSTRAINT " + tableName + "_Id PRIMARY KEY (id)" +
                        ")";
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                    logger?.LogInformation("Table " + tableName + " added");
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    logger?.LogError(e.Message);
                }
                catch (Exception e) { throw e; }
            }
        }
    }
}
