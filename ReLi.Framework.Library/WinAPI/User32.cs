namespace ReLi.Framework.Library.WinAPI
{
    #region Using Declarations

    using System;
    using System.Text;
    using System.Runtime.InteropServices;

    #endregion

	public class User32
    {
        private const Int32 MF_BYPOSITION = 0x400;
        private const Int32 MF_REMOVE = 0x1000;

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        public static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        public static void RemoveCloseButton(IntPtr objWindowHandle)
        {
            IntPtr objMenuHandle = GetSystemMenu(objWindowHandle, false);
            if (objMenuHandle != IntPtr.Zero)
            {
                int intMenuItemCount = GetMenuItemCount(objMenuHandle);
                if (intMenuItemCount > 0)
                {
                    RemoveMenu(objMenuHandle, (uint)(intMenuItemCount - 1), MF_BYPOSITION | MF_REMOVE);
                    RemoveMenu(objMenuHandle, (uint)(intMenuItemCount - 2), MF_BYPOSITION | MF_REMOVE);
                    DrawMenuBar(objWindowHandle);
                }
            }
        }
    }
}
