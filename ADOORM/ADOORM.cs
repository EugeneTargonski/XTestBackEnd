﻿using Microsoft.Extensions.Logging;
using System;
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
        private readonly ILogger logger;

        public ORMLight(string _connectionString, string _baseName, ILogger _logger = null)
        {
            connectionString = _connectionString;
            logger = _logger;
            objectType = typeof(T);
            baseName = _baseName;
            CreateBase();
            CreateFields();
        }
        //Parameterization not used. User havn't access to property names.
        public void ObjectList()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string TableName = objectType.Name.ToString();
                string sqlExpression = "SELECT * FROM " + TableName;
                SqlCommand command = new SqlCommand(sqlExpression, connection);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read()) //read dataset by string
                    {
                        var properties = objectType.GetProperties();
                        foreach (var property in properties)
                        {
                            Console.Write(reader[property.Name] + "\t");
                        }
                        Console.WriteLine(reader["Id"]);
                    }
                }
                reader.Close();
            }
        }
        public object ObjectRead(int id)
        {
            object Readed = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string TableName = objectType.Name.ToString();
                string sqlExpression = "SELECT * FROM " + TableName + " WHERE Id = @id";
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
                        var ObjectId = objectType.GetField("id");
                        ObjectId.SetValue(Readed, (int)reader["id"]);
                    }
                }
                reader.Close();
            }
            return Readed;
        }
        public void ObjectDelete(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string TableName = objectType.Name.ToString();
                string sqlExpression = "DELETE " + TableName + " WHERE Id = @id";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlParameter idParam = new SqlParameter("@id", id);
                command.Parameters.Add(idParam);
                int number = command.ExecuteNonQuery();
            }
        }
        public void ObjectSave(object SavedObject)
        {
            //Objects with id = 0 considered new, and the rest - existing
            int id = (int)(SavedObject.GetType().GetField("id").GetValue(SavedObject));
            if (id == 0)
            { NewObjectSave(SavedObject); }
            else
            { OldObjectSave(SavedObject, id); }
        }
        public void OldObjectSave(object SavedObject, int id)
        {
            string TableName = SavedObject.GetType().Name.ToString();
            TableName += "Table";
            string sqlExpression = "UPDATE " + TableName + " SET ";
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
        private void NewObjectSave(object SavedObject)
        {
            string TableName = SavedObject.GetType().Name.ToString();
            string sqlExpression = "INSERT INTO " + TableName + " (";
            string comma = "";
            var properties = SavedObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                sqlExpression = sqlExpression + comma + property.Name;
                comma = ",";
            }
            sqlExpression += ") VALUES (";
            comma = "@";
            foreach (var property in properties)
            {
                sqlExpression = sqlExpression + comma + property.Name;
                comma = ",@";
            }
            sqlExpression += ");SET @id=SCOPE_IDENTITY()";
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
                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output // Output parameter
                };
                command.Parameters.Add(idParam);
                command.ExecuteNonQuery();
            }
        }
        public void CreateFields()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //Create a structure the properties of a class in SQL 
                string tableName = objectType.Name.ToString();
                CreateTable(tableName);
                foreach (PropertyInfo propinfo in objectType.GetProperties())
                {
                    AddColumns(tableName, propinfo.Name, propinfo.PropertyType);
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
        private void AddColumns(string tableName, string propertyName, Type PropertyType, ILogger logger = null)
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
        private void CreateTable(string tableName, ILogger logger = null)
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
