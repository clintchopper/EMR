namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.Reflection;

    #endregion

    public static class EnumHelper
    {
        #region Static Members

        public static TEnumType GetByCode<TEnumType>(string strCode)
            where TEnumType : struct, IConvertible
        {
            if (typeof(TEnumType).IsEnum == false)
            {
                throw new ArgumentException("The generic TSourceEnumType must be an Enum.", "enuValue");
            }

            TEnumType enuReturnValue = default(TEnumType);

            foreach (TEnumType enuValue in Enum.GetValues(typeof(TEnumType)))
            {
                EnumAttribute objAttribute = ((Enum)(object)enuValue).GetAttribute();
                if ((objAttribute != null) && (objAttribute.Code == strCode))
                {
                    enuReturnValue = enuValue;
                    break;
                }
            }

            return enuReturnValue;
        }

        public static TEnumType GetByName<TEnumType>(string strName)
            where TEnumType : struct, IConvertible
        {
            if (typeof(TEnumType).IsEnum == false)
            {
                throw new ArgumentException("The generic TSourceEnumType must be an Enum.", "enuValue");
            }

            TEnumType enuReturnValue = default(TEnumType);
            try
            {
                enuReturnValue = (TEnumType)Enum.Parse(typeof(TEnumType), strName, true);
            }
            catch 
            {
                enuReturnValue = default(TEnumType);
            }

            return enuReturnValue;
        }

        public static TTargetEnumType Convert<TSourceEnumType, TTargetEnumType>(TSourceEnumType enuValue)
            where TSourceEnumType : struct, IConvertible
            where TTargetEnumType : struct, IConvertible
        {
            if (typeof(TSourceEnumType).IsEnum == false)
            {
                throw new Exception("The generic TSourceEnumType must be an Enum.");
            }
            if (typeof(TTargetEnumType).IsEnum == false)
            {
                throw new Exception("The generic TTargetEnumType must be an Enum.");
            }

            TTargetEnumType enuConvertedValue = (TTargetEnumType)Enum.Parse(typeof(TSourceEnumType), enuValue.ToString());
            return enuConvertedValue;
        }

        #endregion
    }
}



        