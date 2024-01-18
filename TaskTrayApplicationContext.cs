using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MAINSPACE
{
	
      
    public class TaskTrayApplicationContext : ApplicationContext
    {
    	[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern Int32 SetForegroundWindow(int hWnd);




        NotifyIcon NOTIFY_ICON = new NotifyIcon();
        Mainform MAIN_FORM = new Mainform();
        
        public TaskTrayApplicationContext()
        {
        	MenuItem showMenuItem = new MenuItem("Show", new EventHandler(ShowForm));
        	MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            NOTIFY_ICON.Icon = Shutdown_On_Power_Loss.Properties.Resources.AppIcon;
            NOTIFY_ICON.Click += new EventHandler(ShowMainForm);
            NOTIFY_ICON.ContextMenu = new ContextMenu(new MenuItem[] { showMenuItem,exitMenuItem });
            
            NOTIFY_ICON.Visible = true;
            MAIN_FORM.Enable_Timer(true);
        }

        void ShowMainForm(object sender, EventArgs e)
        {
        	try {
	        	MouseEventArgs me = (MouseEventArgs) e;
	        	if (me.Button == System.Windows.Forms.MouseButtons.Left)	{
	        		ShowForm(null,null);
	        	}
        	}
        	catch (Exception arg)
			{
				StaticFunctions.Writelog("ERROR: "+arg.Message);
			}
        }


        void Exit(object sender, EventArgs e)
        {
            GlobalClass.CLOSING_APPLICATION=true;
            MAIN_FORM.Close();
            MAIN_FORM.Dispose();
            NOTIFY_ICON.Icon.Dispose();
            NOTIFY_ICON.Icon = null;
            Application.DoEvents();
            StaticFunctions.Writelog("TrayIcon Closed");
            Application.Exit();
        }
        void ShowForm(object sender, EventArgs e) {
        	//StaticFunctions.Writelog("Showing Form #1");
        	MAIN_FORM.Show();
        	//StaticFunctions.Writelog("Showing Form #2");
	        MAIN_FORM.BringToFront();
	        //StaticFunctions.Writelog("Showing Form #3");
	        SetForegroundWindow(MAIN_FORM.Handle.ToInt32());
	        //StaticFunctions.Writelog("Showing Form #4");
        }
    }
}
