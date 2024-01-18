using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Diagnostics;

namespace MAINSPACE
{
    public partial  class Mainform : Form
    {
    	
    	 //private System.Threading.Timer testTimer;
    	 private bool TimerRunning=false;
    	 Stopwatch RUNNING_TIME = new Stopwatch();
    	 Stopwatch IDLE_TIME = new Stopwatch();
    	 string RUNFORTIME_VALUE="";
    	 string RUNTOHOUR_VALUE="";
    	 int PING_CHECKS=0;
    	 DateTime LAST_SUCCESS_PING=DateTime.Now;
    	 int RANDOM_NUMBER_PING=15;
        public Mainform()
        {
            InitializeComponent();
            pic_stopped.Left=pic_running.Left;
            pic_stopped.Top=pic_running.Top;
            pic_running.Visible=false;
            pic_stopped.Visible=true;
            lbl_shutdown.Text="";
            RUNFORTIME_VALUE=lbl_shutdown.Text;
            if (GlobalClass.ReadConfig()) {
            	lbl_server.Text=GlobalClass.Config.SERVER_NAME;
            	lbl_shutdown.Text=GlobalClass.Config.SHUTDOWN_AFTER;
            	lbl_last_activity.Text="0:00";
            }
        }

       
		void TimerTick()
		{
			int seconds=0;
			List<string> list=new List<string>();
			TimeSpan ts = RUNNING_TIME.Elapsed;
			int tempint=0;
			int randomnumber=0;
			try {
				if (TimerRunning) {
					if (pictureBox_green.Visible) pictureBox_green.Visible=false;
					else pictureBox_green.Visible=true;
					if (RUNNING_TIME.IsRunning && ts.TotalSeconds>=RANDOM_NUMBER_PING) {
						Check_Server();
						PING_CHECKS++;
						//StaticFunctions.Writelog("Ping Checks: "+PING_CHECKS.ToString());
						RUNNING_TIME.Restart();
						Random ran = new Random();
						RANDOM_NUMBER_PING = ran.Next(10, 25);
						//StaticFunctions.Writelog("Next Random Number="+RANDOM_NUMBER_PING.ToString());	
					}
				} else {
					//StaticFunctions.Writelog("Timer Running is disabled");
					pictureBox_green.Visible=false;
				}
			} //try
			catch (Exception arg)
            {
                StaticFunctions.Writelog("TimerTick.ERROR: " + arg.Message);
            }
		}
		public void Check_Server() {
			PingClass ping=new PingClass();
			double totalminutes=0;
			double maxtoshutdown=0;
			bool result=false;
			try {
				if (string.IsNullOrEmpty(GlobalClass.Config.SERVER_NAME)==false) {
					//StaticFunctions.Writelog("Pinging Server: "+GlobalClass.Config.SERVER_NAME);
					result=ping.Ping_Server(GlobalClass.Config.SERVER_NAME);
					if (result) { //Ping was successfull
						//StaticFunctions.Writelog("Pinging is OK for: "+GlobalClass.Config.SERVER_NAME);
						totalminutes=(DateTime.Now-LAST_SUCCESS_PING).TotalSeconds;
						lbl_last_activity.Text=StaticFunctions.Convert_Seconds_To_Time((int)totalminutes);
						LAST_SUCCESS_PING=DateTime.Now;
					} else {
							StaticFunctions.Writelog("Pinging Failed for: "+GlobalClass.Config.SERVER_NAME);
							totalminutes=(DateTime.Now-LAST_SUCCESS_PING).TotalSeconds;
							lbl_last_activity.Text=StaticFunctions.Convert_Seconds_To_Time((int)totalminutes);
							maxtoshutdown=StaticFunctions.Convert_Time_To_Seconds(lbl_shutdown.Text);
							if (maxtoshutdown>0 && totalminutes>=maxtoshutdown) {
								//StaticFunctions.Writelog("Elapsed minutes since last ping="+Math.Round(totalminutes,2).ToString()+" min");
									Show_Reboot_Form();
									if (GlobalClass.CANCEL_SHUTDOWN_COMPUTER)  Enable_Timer(false);
							}
					} //ping failed
					//totalminutes=(DateTime.Now-LAST_SUCCESS_PING).TotalMinutes;
					//lbl_last_activitu.Text=Math.Round((DateTime.Now-LAST_SUCCESS_PING).TotalMinutes,2).ToString();
				} //Server is not empty
			} //try
			catch (Exception arg)
            {
                StaticFunctions.Writelog("Check_Server.ERROR: " + arg.Message);
            }
		}
		

		void Form1Load(object sender, EventArgs e)
		{
			StaticFunctions.Writelog("Mainform Loaded");
			
		}
		public void Enable_Timer(bool enable) {
			if (enable) {
				StaticFunctions.Writelog("Timer Activated");
				lbl_last_activity.Text="0:00";
				LAST_SUCCESS_PING=DateTime.Now;
				pic_running.Visible=true;
				pic_stopped.Visible=false;
				RUNNING_TIME.Start();
				RUNNING_TIME.Restart();
				IDLE_TIME.Start();
				IDLE_TIME.Restart();
				StaticFunctions.Writelog("Timer Activated OK");
				StaticFunctions.Write_File(GlobalClass.PING_FILENAME,DateTime.Now.ToString(),false);
				TimerRunning=true;
			} else {
				StaticFunctions.Writelog("Timer Deactivated");
				pic_running.Visible=false;
				pic_stopped.Visible=true;
				this.Refresh();
				RUNNING_TIME.Stop();
				IDLE_TIME.Stop();
				RUNTOHOUR_VALUE="";
				RUNFORTIME_VALUE="";
				StaticFunctions.Writelog("Timer Deactivated OK");
				TimerRunning=false;
			}
		}
		void Pic_stoppedClick(object sender, EventArgs e)
		{
			Enable_Timer(true);
		}
		void Pic_runningClick(object sender, EventArgs e)
		{
			Enable_Timer(false);
		}
		void Form1FormClosing(object sender, FormClosingEventArgs e)
		{
			if (GlobalClass.CLOSING_APPLICATION==false)	{
				this.Hide();
				e.Cancel=true;
			}
			else {
				StaticFunctions.Writelog("Mainform Closed");
			}
		}

		void Timer1Tick(object sender, EventArgs e)
		{
			TimerTick();
		}

		private bool Save_Settings_old() {
			bool res=false;
			List<string> configlist=new List<string>();
			string configfile=StaticFunctions.RootFolder()+"config.ini";
			try {
				if (StaticFunctions.Read_File(configfile,ref configlist) ) {
					for (int i=0;i<configlist.Count;i++) {
						if (configlist[i].StartsWith("SERVER_NAME=")) configlist[i]="SERVER_NAME="+lbl_server.Text.Trim();
						if (configlist[i].StartsWith("SHUTDOWN_AFTER=")) configlist[i]="SHUTDOWN_AFTER="+lbl3.Text;
					}
					if (StaticFunctions.Write_File(configfile,ref configlist,false) ) {
						StaticFunctions.Writelog("Saving Config.ini OK");
						if (GlobalClass.ReadConfig()) {
							StaticFunctions.Writelog("Reloading Config.ini OK");
							res=true;
						} else StaticFunctions.Writelog("ERROR: Reloading Config.ini");
					} else StaticFunctions.Writelog("ERROR: Writting Config.ini");
				} else StaticFunctions.Writelog("ERROR: Reading Config.ini");
			} //try
			catch (Exception arg)
            {
                StaticFunctions.Writelog("ERROR: " + arg.Message);
            }
            return res;
		}
		void Show_Reboot_Form() {
			bool maintimer=timer1.Enabled;
			timer1.Enabled=false;
			Shutdown_On_Power_Loss.RebootForm rform = new Shutdown_On_Power_Loss.RebootForm();
			rform.ShowDialog(this);
			timer1.Enabled=maintimer;
		}
		
	//*************************************************************************************************
    } //class
} //namespace
