namespace ReLi.Framework.Library.Collections
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;

    #endregion
    
    public class ListBase<TObjectType> : ObjectBase, IList<TObjectType>  
    {
        private List<TObjectType> _objItems;

        public ListBase()
            : this(new List<TObjectType>())
        { }

        public ListBase(IEnumerable<TObjectType> objItems)
            : base()
        {
            if (objItems != null)
            {
                base.BeginInit();
                _objItems.AddRange(objItems);
                base.EndInit();
            }
        }

        public ListBase(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public ListBase(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        protected override void Initialize()
        {
            base.Initialize();

            _objItems = new List<TObjectType>();
        }

        public virtual string ToString(string strSeparator)
        {
            StringBuilder objString = new StringBuilder();

            int intIndex = 0;
            int intTotal = Count;

            foreach (TObjectType objItem in this)
            {
                intIndex++;
                objString.Append(objItem.ToString());

                if (intIndex < intTotal)
                {
                    objString.Append(strSeparator);
                }
            }

            return objString.ToString();
        }

        public TObjectType[] ToArray()
        {
            return _objItems.ToArray();
        }

        public void AddRange(IEnumerable<TObjectType> objItems)
        {
            foreach (TObjectType objItem in objItems)
            {
                Add(objItem);
            }
        }

        public void Sort(Comparison<TObjectType> objComparison)
        {
            _objItems.Sort(objComparison);
        }

        public void Sort(IComparer<TObjectType> objComparer)
        {
            _objItems.Sort(objComparer);
        }

        #region IList<TObjectType> Members

        public int IndexOf(TObjectType objItem)
        {
            return _objItems.IndexOf(objItem);
        }

        public virtual void Insert(int intIndex, TObjectType objItem)
        {
            if ((intIndex < 0) || (intIndex > _objItems.Count))
            {
                throw new ArgumentOutOfRangeException("intIndex", "The index is out of range: " + intIndex);
            }

            BeforeItemAddedEventArgs<TObjectType> objArguments = new BeforeItemAddedEventArgs<TObjectType>(objItem);
            OnBeforeItemAddedEvent(this, objArguments);
            if (objArguments.Cancel == false)
            {
                _objItems.Insert(intIndex, objItem);
                OnItemAddedEvent(this, new ItemAddedEventArgs<TObjectType>(objItem));
            }
        }

        public virtual void RemoveAt(int intIndex)
        {
            if ((intIndex < 0) || (intIndex > _objItems.Count - 1))
            {
                throw new ArgumentOutOfRangeException("intIndex", "The index is out of range: " + intIndex);
            }

            TObjectType objItem = this[intIndex];

            BeforeItemRemovedEventArgs<TObjectType> objArguments = new BeforeItemRemovedEventArgs<TObjectType>(objItem);
            OnBeforeItemRemovedEvent(this, objArguments);
            if (objArguments.Cancel == false)
            {
                _objItems.RemoveAt(intIndex);
                OnItemRemovedEvent(this, new ItemRemovedEventArgs<TObjectType>(objItem));
            }
        }

        public virtual TObjectType this[int intIndex]
        {
            get
            {
                if ((intIndex < 0) || (intIndex > _objItems.Count - 1))
                {
                    throw new ArgumentOutOfRangeException("intIndex", "The index is out of range: " + intIndex);
                }

                return _objItems[intIndex];
            }
            set
            {
                if ((intIndex < 0) || (intIndex > _objItems.Count - 1))
                {
                    throw new ArgumentOutOfRangeException("intIndex", "The index is out of range: " + intIndex);
                }

                _objItems[intIndex] = value;
            }
        }

        #endregion

        #region ICollection<TObjectType> Members

        public virtual void Add(TObjectType objItem)
        {
            if (Initializing == true)
            {
                _objItems.Add(objItem);
            }
            else
            {
                BeforeItemAddedEventArgs<TObjectType> objArguments = new BeforeItemAddedEventArgs<TObjectType>(objItem);
                OnBeforeItemAddedEvent(this, objArguments);
                if (objArguments.Cancel == false)
                {
                    _objItems.Add(objItem);
                    OnItemAddedEvent(this, new ItemAddedEventArgs<TObjectType>(objItem));
                }
            }
        }

        public virtual void Clear()
        {            
            foreach (TObjectType objItem in this.ToArray())
            {
                Remove(objItem);
            }
        }

        public bool Contains(TObjectType objItem)
        {
            return _objItems.Contains(objItem);
        }

        public void CopyTo(TObjectType[] arrItems)
        {
            _objItems.CopyTo(arrItems);
        }

        public void CopyTo(TObjectType[] arrItems, int intArrayIndex)
        {
            _objItems.CopyTo(arrItems, intArrayIndex);
        }

        public void CopyTo(int intIndex, TObjectType[] arrItems, int intArrayIndex, int intCount)
        {
            _objItems.CopyTo(intIndex, arrItems, intArrayIndex, intCount);
        }

        public int Count
        {
            get
            {
                return _objItems.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public virtual bool Remove(TObjectType objItem)
        {
            bool blnResult = false;

            if (Initializing == true)
            {
                blnResult = _objItems.Remove(objItem);
            }
            else
            {
                BeforeItemRemovedEventArgs<TObjectType> objArguments = new BeforeItemRemovedEventArgs<TObjectType>(objItem);
                OnBeforeItemRemovedEvent(this, objArguments);
                if (objArguments.Cancel == false)
                {
                    blnResult = _objItems.Remove(objItem);
                    if (blnResult == true)
                    {
                        OnItemRemovedEvent(this, new ItemRemovedEventArgs<TObjectType>(objItem));
                    }
                }
            }

            return blnResult;
        }

        #endregion

        #region IEnumerable<TObjectType> Members

        public IEnumerator<TObjectType> GetEnumerator()
        {
            return _objItems.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _objItems.GetEnumerator();
        }

        #endregion

        #region ICustomSerializer Members

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);

            if (_objItems.Count > 0)
            {
                SerializedObject objChildren = objSerializedObject.Objects.Add("Items");

                SerializedObjectCollection objObjects = objChildren.Objects;
                SerializedValueCollection objValues = objChildren.Values;

                SerializedValueType enuSerializedValueType = SerializedValueTypeHelper.GetTypeFromSystemType(typeof(TObjectType));
                foreach (TObjectType objChildItem in _objItems)
                {
                    if (enuSerializedValueType != SerializedValueType.Unknown)
                    {
                        objValues.Add("Item", objChildItem);
                    }
                    else if (objChildItem is ICustomSerializer)
                    {
                        objObjects.Add("Item", (ICustomSerializer)objChildItem);
                    }
                    else
                    {
                        objObjects.Add("Item", new SerializedWrapperObject(objChildItem));
                    }
                }
            }
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);

            Initialize();

            SerializedObject objSerializedItems = objSerializedObject.Objects["Items"];
            if (objSerializedItems != null)
            {
                foreach (SerializedValue objItem in objSerializedItems.Values.ToArray())
                {
                    TObjectType objValue = (TObjectType)objItem.Value;
                    Add(objValue);
                }

                foreach (SerializedObject objItem in objSerializedItems.Objects.ToArray())
                {
                    TObjectType objValue = default(TObjectType);
                    ICustomSerializer objCustomSerializer = objItem.Deserialize();

                    if (objCustomSerializer is SerializedWrapperObject)
                    {
                        SerializedWrapperObject objSerializedWrapperObject = (SerializedWrapperObject)objCustomSerializer;
                        objValue = (TObjectType)objSerializedWrapperObject.Data;
                    }
                    else
                    {
                        objValue = (TObjectType)((object)objCustomSerializer);
                    }

                    Add(objValue);
                }
            }
        }

        #endregion

        #region TransportableObject Members 

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);

            TransportableDataType enuDataType = TransportableDataTypeHelper.GetTypeFromSystemType(typeof(TObjectType));

            int intCount = this.Count;
            objBinaryWriter.Write(intCount);

            for (int intIndex = 0; intIndex < intCount; intIndex++)
            {
                TObjectType objItem = this[intIndex];
                objBinaryWriter.WriteObject(objItem, enuDataType);
            }
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);

            int intCount = objBinaryReader.ReadInt32();
            for (int intIndex = 0; intIndex < intCount; intIndex++)
            {
                TObjectType objItem = (TObjectType)objBinaryReader.ReadObject();
                Add(objItem);
            }
        }

        #endregion 

        #region Event Declarations

        public event BeforeItemAddedEventHandler<TObjectType> BeforeItemAddedEvent;
        protected void OnBeforeItemAddedEvent(object objSender, BeforeItemAddedEventArgs<TObjectType> objArguments)
        {
            if (Initializing == false)
            {
                BeforeItemAddedEventHandler<TObjectType> objHandler = BeforeItemAddedEvent;
                if (objHandler != null)
                {
                    objHandler(objSender, objArguments);
                }
            }
        }

        public event ItemAddedEventHandler<TObjectType> ItemAddedEvent;
        public event ItemAddedEventHandler<TObjectType> ItemAddedEventAsync;
        protected void OnItemAddedEvent(object objSender, ItemAddedEventArgs<TObjectType> objArguments)
        {
            if (Initializing == false)
            {
                ItemAddedEventHandler<TObjectType> objHandler = ItemAddedEventAsync;
                if (objHandler != null)
                {
                    AsyncHelper.FireAndForget(objHandler, objSender, objArguments);
                }

                objHandler = ItemAddedEvent;
                if (objHandler != null)
                {
                    objHandler(objSender, objArguments);
                }
            }
        }

        public event BeforeItemRemovedEventHandler<TObjectType> BeforeItemRemovedEvent;
        protected void OnBeforeItemRemovedEvent(object objSender, BeforeItemRemovedEventArgs<TObjectType> objArguments)
        {
            if (Initializing == false)
            {
                BeforeItemRemovedEventHandler<TObjectType> objHandler = BeforeItemRemovedEvent;
                if (objHandler != null)
                {
                    objHandler(objSender, objArguments);
                }
            }
        }

        public event ItemRemovedEventHandler<TObjectType> ItemRemovedEvent;
        public event ItemRemovedEventHandler<TObjectType> ItemRemovedEventAsync;
        protected void OnItemRemovedEvent(object objSender, ItemRemovedEventArgs<TObjectType> objArguments)
        {
            if (Initializing == false)
            {
                ItemRemovedEventHandler<TObjectType> objHandler = ItemRemovedEventAsync;
                if (objHandler != null)
                {
                    AsyncHelper.FireAndForget(objHandler, objSender, objArguments);
                }

                objHandler = ItemRemovedEvent;
                if (objHandler != null)
                {
                    objHandler(objSender, objArguments);
                }
            }
        }

        #endregion
    }
}
