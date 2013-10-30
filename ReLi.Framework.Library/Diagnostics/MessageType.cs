namespace ReLi.Framework.Library.Diagnostics
{
    #region Using Declarations

    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion
        
	public class MessageType : ObjectBase 
    {
        private string _strName;
        private string _strFullName;
        private bool _blnEnabled;

        public MessageType(Type objType)
            : this(objType, true)
        {}
        
        public MessageType(Type objType, bool blnEnabled)
            : base()
        {
            bool blnIsMessageBase = objType.IsSubclassOf(typeof(MessageBase));
            if (blnIsMessageBase == false)
            {
                throw new ArgumentException("Type", "A type of MessageBase is required.");
            }

            FullName = objType.FullName;
            Name = objType.Name;
            Enabled = blnEnabled;
        }

        public MessageType(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public MessageType(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public string Name
        {
            get
            {
                return _strName;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Name", "A valid non-null string is required.");
                }

                _strName = value;
            }
        }

        public string FullName
        {
            get
            {
                return _strFullName;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("FullName", "A valid non-null string is required.");
                }

                _strFullName = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _blnEnabled;
            }
            set
            {
                if (_blnEnabled != value)
                {
                    bool blnOldValue = _blnEnabled;

                    _blnEnabled = value;
                    OnPropertyChanged("Enabled", blnOldValue, value);                    
                }                
            }
        }

        #region Event Declarations

        public event MessageTypePropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string strName, object objOldValue, object objNewValue)
        {
            if (this.PropertyChanged != null)
            {
                MessageTypePropertyChangedEventArgs objArguments = new MessageTypePropertyChangedEventArgs(this, strName, objOldValue, objNewValue);
                this.PropertyChanged(this, objArguments);
            }
        }

        #endregion

        #region SerializableObject Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            objSerializedObject.Values.Add("Name", Name);
            objSerializedObject.Values.Add("FullName", FullName);
            objSerializedObject.Values.Add("Enabled", Enabled);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Name = objSerializedObject.Values.GetValue<string>("Name", null);
            FullName = objSerializedObject.Values.GetValue<string>("FullName", null);
            Enabled = objSerializedObject.Values.GetValue<bool>("Enabled", true);
        }

        #endregion    

        #region TransportableObject Members

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            objBinaryWriter.Write(Name);
            objBinaryWriter.Write(FullName);
            objBinaryWriter.Write(Enabled);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            Name = objBinaryReader.ReadString();
            FullName = objBinaryReader.ReadString();
            Enabled = objBinaryReader.ReadBoolean();
        }

        #endregion

    }
}
