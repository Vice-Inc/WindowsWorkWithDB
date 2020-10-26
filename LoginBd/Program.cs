using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace LoginBd
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //SqlDependency.Start("server=DESKTOP-RFI8Q0J;Trusted_Connection=Yes;DataBase=Game;");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
            //Application.Run(new AdminForm());

            //SqlDependency.Stop("server=DESKTOP-RFI8Q0J;Trusted_Connection=Yes;DataBase=Game;");
        }

        
    }
}
