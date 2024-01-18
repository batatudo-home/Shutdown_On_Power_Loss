using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MAINSPACE
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
        	StaticFunctions.Create_Folder(StaticFunctions.RootFolder()+"logs");
        	StaticFunctions.Create_Folder(StaticFunctions.RootFolder()+"data");
        	StaticFunctions.Delete_Old_Logs(StaticFunctions.RootFolder()+"logs",31,"*.*");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            StaticFunctions.Writelog("--------------------------------------------------------------------------");
            StaticFunctions.Writelog("Application Started");
            Application.Run(new TaskTrayApplicationContext());
            StaticFunctions.Writelog("Application Exit");
        }
    }
    
} //nameclass
