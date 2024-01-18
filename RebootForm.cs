/*
 * Created by SharpDevelop.
 * User: X005012
 * Date: 3/21/2023
 * Time: 8:38 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Shutdown_On_Power_Loss
{
	/// <summary>
	/// Description of RebootForm.
	/// </summary>
	public partial class RebootForm : Form
	{
		int COUNT_DOWN=61;
		public RebootForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void Button1Click(object sender, EventArgs e)
		{
			Canceling_Shutdown_Button();
		}
		void Canceling_Shutdown_Button() {
			StaticFunctions.Writelog("Canceling Shutdown!!");
			GlobalClass.CANCEL_SHUTDOWN_COMPUTER=true;
			this.Dispose();
			this.Close();
		}
		void Shutdown_Computer() {
			PowerShellClass ps=new PowerShellClass();
				StaticFunctions.Writelog("Shutting Down Computer: [localhost]");
				if (GlobalClass.Config.SHUTDOWN) {
					timer1.Enabled=false;
					StaticFunctions.Writelog("Shutting down Server Now...");
					label1.Text="Shutting down server now...";
					if (ps.Shutdown_Computer("localhost")) {
						StaticFunctions.Writelog("Shutting Down Computer OK");
						System.Environment.Exit(1);
					} else StaticFunctions.Writelog("Shutting Down Computer [localhost] Failed");
				} else {
					StaticFunctions.Writelog("Shutting Down Computer [localhost] has been ignored");
					Canceling_Shutdown_Button();
				}
		}
		void RebootFormLoad(object sender, EventArgs e)
		{
			Update_Count_Down();
		}
		void Update_Count_Down() {
			COUNT_DOWN--;
			lbl_shutdown.Text="Computer will Shutdown  in "+COUNT_DOWN.ToString()+" seconds";
			if (COUNT_DOWN<=0) Shutdown_Computer();
		}
		void Timer1Tick(object sender, EventArgs e)
		{
			Update_Count_Down();
		}
		void RebootFormFormClosing(object sender, FormClosingEventArgs e)
		{
			GlobalClass.CANCEL_SHUTDOWN_COMPUTER=true;
		}
	} //class
} //namespace
