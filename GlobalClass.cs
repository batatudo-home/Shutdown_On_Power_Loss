using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Net.Mail;
using System.Windows.Forms;


    public static class GlobalClass
    {
    	
    	//public static System.Threading.Timer testTimer;
    	//public static TimerCallback timerDelegate; //= new TimerCallback(TimerTick);
    	
    	public static bool CLOSING_APPLICATION=false;
    	public static bool CANCEL_SHUTDOWN_COMPUTER=false;

    	
        public struct ConfigStruct
        {
        	public string SERVER_NAME;
        	public string SHUTDOWN_AFTER;
        	public bool SHUTDOWN;
        }

        static public ConfigStruct Config;
        static public string RUNNINGTIME=DateTime.Now.ToString("MMddyyyy-hhmmss");
        static public string PING_FILENAME=StaticFunctions.RootFolder()+@"data\pingstatus.ini";
        public static void InitializeVariables() {
        	Config.SHUTDOWN=true;
        	
        }
        

public static bool ReadConfig()
        {
            bool res = false;
            string line="";
            int start = -1;
            string tag = "";
            string value = "";
            string configfile=StaticFunctions.RootFolder()+"config.ini";
            int errors=0;
            int tempvalue=0;
            bool commentblock=false;
            int commentblockstart=0;
            int commentblockend=0;
            try
            {
            	InitializeVariables();
                if (File.Exists(configfile)) {
	                StreamReader reader = File.OpenText(configfile);
		                while ((line = reader.ReadLine()) != null)
	                {
		                	
		                	commentblockstart = line.IndexOf("<!--"); //comments found
		                	if (commentblockstart>-1) {
		                		line="";
		                		commentblock=true;
		                	}
		                	commentblockend = line.IndexOf("-->"); //comments found
		                	if (commentblockend>-1) {
		                		line="";
		                		commentblock=false;
		                	}
		                start = line.IndexOf("~");
	                    if (start > -1 && line.ToUpper().Contains("PASSWORD")==false) line = line.Substring(0, start);
	                    line = line.Trim();
	                    if (line != "" && commentblock==false)
	                    {
	                            start = line.IndexOf("=");
	                            if (start>-1)
	                            {
	                                tag= line.Substring(0, start+1);
	                                value= line.Substring(start+1);
	                                value=value=StaticFunctions.Resolve_Variables(value);
			                                switch (tag.ToUpper())
			                                {
			                                		
			                                	case "SERVER_NAME=":
			                                		if (value!="") {
			                                			Config.SERVER_NAME = value;
			                                		} else { errors++; StaticFunctions.Writelog("ERROR: CHECK_SERVER is empty");}
			                                		break;
			                                    case "SHUTDOWN_AFTER=":
			                                    			Config.SHUTDOWN_AFTER = value;
			                                    break;
			                                    case "SHUTDOWN=":
				                                   	if (value!="") {
			                                    		if (value.ToUpper()=="TRUE") {
			                                    			Config.SHUTDOWN=true;
			                                    		} else Config.SHUTDOWN = false;
			                                    	}
			                                    		 
			                                    break;
			                                    default:                                      
			                                    break;
			                                } //end switch
	                            }
	                        
	                    }
	                }
	                reader.Close();
	                if (errors==0) res = true;
                } else StaticFunctions.Writelog("ERROR: Config file not found: "+configfile);
            } //try
            catch (Exception arg)
            {
                StaticFunctions.Writelog("GLOBALCLASS.ERROR: "+arg.Message);
            }
                return res;
        }




        
		public static int Find_String_Index_In_ComboBox(string text,ref System.Windows.Forms.ComboBox cmb)
		{
			int res = -1;
			try
			{
				if (text!="") {
					for (int i=0;i<cmb.Items.Count;i++) {
						if (cmb.Items[i].ToString().ToUpper()==text.ToUpper()) {
							res=i;
							break;
						}
					} //for
				}
			}
			catch (Exception arg)
			{
				StaticFunctions.Writelog("ERROR: "+arg.Message);
			}
			return res;
		}
		
	//*****************************************************************************************************************		
    } //GlobalClass

