namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Data;
    using System.Text;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class SqlMessage : MessageBase
    {
        private string _strSqlStatement;
        private string _strSqlParameters;
        private string _strConnectionString;

        public SqlMessage(string strConnectionString, IDbCommand objDbCommand)
            : base()
        {
            if (objDbCommand == null)
            {
                throw new ArgumentNullException("objDbCommand", "A valid non-null IDbCommand is required.");
            }

            StringBuilder objSQLParameters = new StringBuilder();
            foreach (IDataParameter objParameter in objDbCommand.Parameters)
            {
                objSQLParameters.AppendLine("Type: " + objParameter.DbType.ToString() + "; Name: " + objParameter.ParameterName + "; Value: " + objParameter.Value.ToString());
            }

            SqlStatement = objDbCommand.CommandText; 
            SqlParameters = objSQLParameters.ToString(); 
            ConnectionString = strConnectionString;
        }

        public SqlMessage(string strConnectionString, string strSqlStatement, string strSqlParameters)
            : base()
        {
            SqlStatement = strSqlStatement;
            SqlParameters = strSqlParameters;
            ConnectionString = strConnectionString;
        }
        
        public SqlMessage(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public SqlMessage(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string SqlStatement
        {
            get
            {
                return _strSqlStatement;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("SqlStatement", "A valid non-null string is required.");
                }

                _strSqlStatement = value;
            }
        }

        public string SqlParameters
        {
            get
            {
                return _strSqlParameters;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("SqlParameters", "A valid non-null string is required.");
                }

                _strSqlParameters = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return _strConnectionString;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ConnectionString", "A valid non-null string is required.");
                }

                _strConnectionString = value;
            }
        }

        public override string Content
        {
            get
            {
                StringBuilder objContent = new StringBuilder();
                objContent.AppendLine("Connection String: " + _strConnectionString);
                objContent.AppendLine("SQL Statement: " + _strSqlStatement);
                if (_strSqlParameters.Length > 0)
                {
                    objContent.AppendLine("SQL Parameters: " + _strSqlParameters);
                }

                return objContent.ToString();
            }
        }

        public override string ToString()
        {
            StringBuilder objString = new StringBuilder();
            objString.Append(base.ToString());
            objString.AppendLine(Content);

            return objString.ToString();
        }

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("SqlStatement", SqlStatement);
            objSerializedObject.Values.Add("SqlParameters", SqlParameters);
            objSerializedObject.Values.Add("ConnectionString", ConnectionString);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            SqlStatement = objSerializedObject.Values.GetValue<string>("SqlStatement", string.Empty);
            SqlParameters = objSerializedObject.Values.GetValue<string>("SqlParameters", string.Empty);
            ConnectionString = objSerializedObject.Values.GetValue<string>("ConnectionString", string.Empty); 
        }

        #endregion

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(SqlStatement);
            objBinaryWriter.Write(SqlParameters);
            objBinaryWriter.WriteEncryptedString(ConnectionString);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            SqlStatement = objBinaryReader.ReadString();
            SqlParameters = objBinaryReader.ReadString();
            ConnectionString = objBinaryReader.ReadEncryptedString();
        }

        #endregion

    }
}
