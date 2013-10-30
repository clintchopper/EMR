namespace ReLi.Framework.Library.Wcf
{
    #region Using Declarations 

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Principal;
    using System.Collections.Generic;
    using ReLi.Framework.Library;
    using ReLi.Framework.Library.IO;
    using ReLi.Framework.Library.Serialization;
    using ReLi.Framework.Library.Collections;

    #endregion 

    public class WcfClientChannelConnectionSettingsList : ObjectListBase<WcfClientChannelConnectionSettings>
    {
        public WcfClientChannelConnectionSettingsList()
            : base()
        {}

        public WcfClientChannelConnectionSettingsList(IEnumerable<WcfClientChannelConnectionSettings> objWcfClientChannelConnectionSettings)
             : base(objWcfClientChannelConnectionSettings)
        {}

        public WcfClientChannelConnectionSettingsList(SerializedObject objSerializedObject)
            : base(objSerializedObject)
        {}

        public WcfClientChannelConnectionSettingsList(BinaryReaderExtension objBinaryReader)
            : base(objBinaryReader)
        {}

        public bool AreChannelsAvaliable()
        {
            bool blnAreChannelsAvailable = false;
            
            Sort();
            foreach (WcfClientChannelConnectionSettings objChannelConnection in this)
            {
                blnAreChannelsAvailable = objChannelConnection.IsChannelAvaliable();
                if (blnAreChannelsAvailable == true)
                {
                    break;
                }
            }

            return blnAreChannelsAvailable;
        }

        public void Sort()
        {
            Comparison<WcfClientChannelConnectionSettings> objComparison = new Comparison<WcfClientChannelConnectionSettings>(WcfClientChannelConnectionSettingsComparison);
            base.Sort(objComparison);
        }

        private int WcfClientChannelConnectionSettingsComparison(WcfClientChannelConnectionSettings objWcfClientChannelConnectionSettings1, WcfClientChannelConnectionSettings objWcfClientChannelConnectionSettings2)
        {
            int intResult = 0;

            if ((objWcfClientChannelConnectionSettings1 == null) && (objWcfClientChannelConnectionSettings2 == null))
            {
                intResult = 0;
            }
            else if (objWcfClientChannelConnectionSettings1 == null)
            {
                intResult = -1;
            }
            else if (objWcfClientChannelConnectionSettings2 == null)
            {
                intResult = 1;
            }
            else
            {
                intResult = objWcfClientChannelConnectionSettings1.Priority.CompareTo(objWcfClientChannelConnectionSettings2.Priority);
            }

            return intResult;
        }

        #region ICustomSerializer Members 

        public override void WriteData(SerializedObject objSerializedObject)
        {
            base.WriteData(objSerializedObject);
        }

        public override void ReadData(SerializedObject objSerializedObject)
        {
            base.ReadData(objSerializedObject);
        }

        #endregion 

        #region TransportableObject Members 

        public override void WriteData(BinaryWriterExtension objBinaryWriter)
        {
            base.WriteData(objBinaryWriter);
        }

        public override void ReadData(BinaryReaderExtension objBinaryReader)
        {
            base.ReadData(objBinaryReader);
        }

        #endregion 

    }
}
