using System;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application. Everything runs inside a
        /// single Shell window; the screens are pages swapped inside it.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Shell());
        }
    }
}
