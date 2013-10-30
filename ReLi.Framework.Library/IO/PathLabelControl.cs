namespace ReLi.Framework.Library.IO
{
    #region Using Declarations

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Data;
    using System.Text;
    using System.Windows.Forms;

    #endregion

    public partial class PathLabelControl : Label
    {
        private string _strLongText;
        private string _strShortText;
        private EllipsisFormat _enuEllipsisFormat;
        private ToolTip _objToolTip = new ToolTip();

        public override string Text
        {
            set
            {
                _strLongText = value;
                _strShortText = Ellipsis.Compact(_strLongText, this, AutoEllipsis);

                _objToolTip.SetToolTip(this, _strLongText);
                base.Text = _strShortText;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Text = FullText;
        }

        [Browsable(false)]
        public virtual string FullText
        {
            get 
            { 
                return _strLongText; 
            }
        }

        [Browsable(false)]
        public virtual string EllipsisText
        {
            get 
            { 
                return _strShortText; 
            }
        }

        [Browsable(false)]
        public virtual bool IsEllipsis
        {
            get 
            { 
                return (_strLongText != _strShortText); 
            }
        }
        
        [Category("Behavior")]
        [Description("Define EllipsisFormat format and alignment when text exceeds the width of the control")]
        public new EllipsisFormat AutoEllipsis
        {
            get 
            { 
                return _enuEllipsisFormat; 
            }
            set
            {
                if (_enuEllipsisFormat != value)
                {
                    _enuEllipsisFormat = value;
                    this.Text = FullText;
                    OnAutoEllipsisChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Property Changed")]
        [Description("Event raised when the value of AutoEllipsis property is changed on Control")]
        public event EventHandler AutoEllipsisChanged;

        protected void OnAutoEllipsisChanged(EventArgs e)
        {
            if (AutoEllipsisChanged != null)
            {
                AutoEllipsisChanged(this, e);
            }
        }
         
    }
}
