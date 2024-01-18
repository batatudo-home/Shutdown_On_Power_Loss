//******************************GLOBAL CLASS**********************************************
/*
 * Created by SharpDevelop.
 * User: X005012
 * Date: 8/19/2020
 * Time: 9:05 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net.NetworkInformation;   
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using System.Net.NetworkInformation;

	/// <summary>
	/// Description of PingClass.
	/// </summary>
	
	public class PingClass
	{
		private string LOG_EXTRA_NAME="";
		public PingClass(string log_extra_name="")
		{
			LOG_EXTRA_NAME=log_extra_name.ToUpper().Trim().Replace(",","-").Replace(";","-");
		}

	public bool Ping_Server(string servername) {
     	bool res=false;
        int failed = 0;
     	try {
		     	if (String.IsNullOrEmpty(servername)==false) {
		     	    Ping pingSender = new Ping ();
		            PingOptions options = new PingOptions ();
		
		            // Use the default Ttl value which is 128,
		            // but change the fragmentation behavior.
		            options.DontFragment = true;
		
		            // Create a buffer of 32 bytes of data to be transmitted.
		            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
		            byte[] buffer = Encoding.ASCII.GetBytes (data);
		            int timeout = 6000;
		            PingReply reply = pingSender.Send (servername, timeout, buffer, options);
                    if (reply.Status != IPStatus.Success)
                    {
                        //WaitSeconds(1);
                        reply = pingSender.Send(servername, timeout, buffer, options);
                        if (reply.Status != IPStatus.Success)
                        {
                            //WaitSeconds(1);
                            reply = pingSender.Send(servername, timeout, buffer, options);
                            if (reply.Status == IPStatus.Success) res = true;
                        } else res = true;
                    } else res = true;
		            
		     	} //servername
     	} //try
     	catch (Exception arg)
        {
            //StaticFunctions.Writelog("ERROR: "+arg.Message);
        }
     	return res;
     }
			
		public void WaitSeconds(int seconds=0,bool writemessage=false) {
        	//int miliseconds=0;
        	if (writemessage) StaticFunctions.Writelog("Waiting "+seconds.ToString()+" Seconds...");
        		System.Threading.Thread.Sleep(seconds*1000);
        }
	

		public string RootFolder() {
        	string res=System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location);
        	if (res.Last().ToString()!=@"\") res=res+@"\";
        	return res;
        }
	} //class

