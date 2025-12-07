using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.LookAndFeel;
namespace ChargingApp
{
    internal static class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {  
        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Show login form first
            

            Assembly asm = typeof(DevExpress.UserSkins.RoundedTheme.RoundedTheme).Assembly;
            DevExpress.XtraEditors.WindowsFormsSettings.RegisterUserSkins(asm);
            UserLookAndFeel.Default.SetSkinStyle("VGreenRounded");
            Application.Run(new ThanhPhan.Form1());














            /*
            using (var loginForm = new ThanhPhan.FrmLogin())
        {
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // If login is successful, show the main form
                Application.Run(new ThanhPhan.Form1());
            }
        }    */

        }
    }
}
