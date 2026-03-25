using System;
using System.Windows.Forms;

namespace kiosk
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ── Initialise SQLite database ────────────────────────────────
            // Creates HiveCafe.db next to the .exe if it doesn't exist yet.
            // Requires NuGet: Install-Package System.Data.SQLite
            DatabaseManager.Initialise();

            // ── Launch splash / start screen ──────────────────────────────
            Application.Run(new Form1());
        }
    }
}
