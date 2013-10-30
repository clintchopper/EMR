namespace ReLi.Framework.Library
{
    #region Using Declarations

    using System;
    using System.Reflection;

    #endregion

    public static class EnumExtensions
    {
        #region Static Members

        public static EnumAttribute GetAttribute(this Enum enuType)
        {
            EnumAttribute objAttribute = null;

            MemberInfo[] objMemberInfo = enuType.GetType().GetMember(enuType.ToString());
            if ((objMemberInfo != null) && (objMemberInfo.Length > 0))
            {
                object[] objAttributes = objMemberInfo[0].GetCustomAttributes(typeof(EnumAttribute), true);
                if ((objAttributes != null) && (objAttributes.Length > 0))
                {
                    objAttribute = objAttributes[0] as EnumAttribute;
                }
            }

            return objAttribute;
        }

        public static string GetCode(this Enum enuType)
        {
            string strCode = string.Empty;

            EnumAttribute objAttribute = GetAttribute(enuType);
            if (objAttribute != null)
            {
                strCode = objAttribute.Code;
            }

            return strCode;
        }

        public static string GetDescription(this Enum enuType)
        {
            string strDescription = string.Empty;

            EnumAttribute objAttribute = GetAttribute(enuType);
            if (objAttribute != null)
            {
                strDescription = objAttribute.Description;
            }

            return strDescription;
        }

        public static string GetName(this Enum enuType)
        {
            string strName = string.Empty;

            EnumAttribute objAttribute = GetAttribute(enuType);
            if (objAttribute != null)
            {
                strName = objAttribute.Name;
            }

            return strName;
        }

        public static TTargetEnumType ConvertTo<TTargetEnumType>(this Enum enuType)
            where TTargetEnumType : struct, IConvertible
        {
            if (typeof(TTargetEnumType).IsEnum == false)
            {
                throw new Exception("The generic TTargetEnumType must be an Enum.");
            }

            TTargetEnumType enuConvertedValue = (TTargetEnumType)Enum.Parse(enuType.GetType(), enuType.ToString());
            return enuConvertedValue;
        }

        #endregion
    }
}
