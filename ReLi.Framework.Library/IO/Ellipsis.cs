namespace ReLi.Framework.Library.IO
{
    #region Using Declarations

    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.IO;
    using System.Text.RegularExpressions;

    #endregion

    [Flags]
    public enum EllipsisFormat
    {
        NotSelected = 0,
        End = 1,
        Start = 2,
        Middle = 3,
        Path = 4,
        Word = 8
    }
    
	public class Ellipsis
    {
        public static readonly string EllipsisChars = "...";

        private static Regex objPreviousWord = new Regex(@"\W*\w*$");
        private static Regex objNextWord = new Regex(@"\w*\W*");

        public static string Compact(string strText, Control objControl, EllipsisFormat enuEllipsisFormat)
        {
            if (string.IsNullOrEmpty(strText))
            {
                return strText;
            }
            if ((EllipsisFormat.Middle & enuEllipsisFormat) == 0)
            {
                return strText;
            }
            if (objControl == null)
            {
                throw new ArgumentNullException("objControl", "A valid non-null Control is expected.");
            }

            using (Graphics objGraphics = objControl.CreateGraphics())
            {
                Size objSize = TextRenderer.MeasureText(objGraphics, strText, objControl.Font);

                if (objSize.Width <= objControl.Width)
                {
                    return strText;
                }

                string strBegin = string.Empty;
                string strMiddle = strText;
                string strEnd = string.Empty;

                bool blnIsPath = (EllipsisFormat.Path & enuEllipsisFormat) != 0;
                if (blnIsPath)
                {
                    strBegin = Path.GetPathRoot(strText);
                    strMiddle = Path.GetDirectoryName(strText).Substring(strBegin.Length);
                    strEnd = Path.GetFileName(strText);
                }

                int intLength = 0;
                int intSegment = strMiddle.Length;
                string strFit = string.Empty;

                while (intSegment > 1)
                {
                    intSegment -= intSegment / 2;

                    int intLeft = intLength + intSegment;
                    int intRight = strMiddle.Length;

                    if (intLeft > intRight)
                    {
                        continue;
                    }
                    
                    if ((EllipsisFormat.Middle & enuEllipsisFormat) == EllipsisFormat.Middle)
                    {
                        intRight -= intLeft / 2;
                        intLeft -= intLeft / 2;
                    }
                    else if ((EllipsisFormat.Start & enuEllipsisFormat) != 0)
                    {
                        intRight -= intLeft;
                        intLeft = 0;
                    }

                    if ((EllipsisFormat.Word & enuEllipsisFormat) != 0)
                    {
                        if ((EllipsisFormat.End & enuEllipsisFormat) != 0)
                        {
                            intLeft -= objPreviousWord.Match(strMiddle, 0, intLeft).Length;
                        }
                        if ((EllipsisFormat.Start & enuEllipsisFormat) != 0)
                        {
                            intRight += objNextWord.Match(strMiddle, intRight).Length;
                        }
                    }
                                        
                    string strTest = strMiddle.Substring(0, intLeft) + EllipsisChars + strMiddle.Substring(intRight);

                    if (blnIsPath)
                    {
                        strTest = Path.Combine(Path.Combine(strBegin, strTest), strEnd);
                    }

                    objSize = TextRenderer.MeasureText(objGraphics, strTest, objControl.Font);
                    if (objSize.Width <= objControl.Width)
                    {
                        intLength += intSegment;
                        strFit = strTest;
                    }
                }

                if (intLength == 0)
                {
                    if (blnIsPath == false)
                    {
                        return EllipsisChars;
                    }

                    if ((strBegin.Length == 0) && (strMiddle.Length == 0))
                    {
                        return strEnd;
                    }
                                        
                    strFit = Path.Combine(Path.Combine(strBegin, EllipsisChars), strEnd);
                    objSize = TextRenderer.MeasureText(objGraphics, strFit, objControl.Font);

                    if (objSize.Width > objControl.Width)
                    {
                        strFit = Path.Combine(EllipsisChars, strEnd);
                    }
                }

                return strFit;
            }
        }
    }
}
