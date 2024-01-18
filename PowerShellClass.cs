//******************************GLOBAL CLASS**********************************************
/*
 * Created by SharpDevelop
 * User: X005012
 * Date: 3/6/2020
 * Time: 9:33 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Collections.Generic;
using System.Management.Automation.Runspaces;
using System.Text;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
//using System.Windows.Forms;


/// <summary>
/// Description of PowerShellClass.
/// </summary>
public class PowerShellClass
{
	private string LOG_EXTRA_NAME="";
	const string USERNAME_SYSTEM=@"SYSTEM";
	const string USERNAME_SYSTEM_FULLNAME=@"NT AUTHORITY\SYSTEM";
	
	public enum TASK_STATE : int
	{
		STOP=0,
		START=1,
		ENABLE=2,
		DISABLE=3
	}
	
	public struct TaskStruct {
		public string COMPUTER_NAME;
		public string TASK_PATH;
		public string TASK_NAME;
		public string STATE;
		public string DESCRIPTION;
		public string AUTHOR;
		public string RUNASUSER;
		public string LOGON_TYPE;
		public string PASSWORD_UPDATED;
		public string EMPTY_TASK;
		public string DELETED_TASK;
		//******EXTRA INFO FILLED BY GET_TASK_EXTRA_INFO*******
		public string LAST_RUNTIME;
		public string LAST_TASK_RESULT;
		public string NEXT_RUNTIME;
		public string NUMBER_OF_MISSED_RUNS;
	}
	
	public struct DriveInfoStruct {
		public string DRIVE_LETTER;
		public string PROVIDER;
		public string USED_SPACE;
		public string FREE_SPACE;
	}
	
	public struct LoggedUsersStruct {
		public string ACTIVE;
		public string SESSION_NAME;
		public string USERNAME;
		public string SESSION_ID;
		public string STATE;
		public string TYPE;
		public string DEVICE;
	}	
	public struct ClusterResourceStruct {
		public string NAME;
		public string STATE;
		public string OWNERGROUP;
		public string RESOURCETYPE;
	}
	public struct AccountInfoStruct {
		public string USER_NAME;
		public string FULL_NAME;
		public string COMMENT;
		public string USER_COMMENT;
		public string ACCOUNT_ACTIVE;
		public string ACCOUNT_EXPIRES;
		public string PASSWORD_LAST_SET;
		public string PASSWORD_EXPIRES;
		public string PASSWORD_CHANGEABLE;
		public string PASSWORD_REQUIRED;
		public string USER_MAY_CHANGE_PASSWORD;
	}
	public PowerShellClass(string log_extra_name="") { //CONTRUCTOR
		LOG_EXTRA_NAME=log_extra_name.ToUpper().Trim().Replace(",","-").Replace(";","-");
		Create_Folder(RootFolder()+"temp");
	}
	public bool Run_Command(string scriptText) 
	{
		bool res=false;
		try {
				if (scriptText!="") { 
					res=Run_Command_No_Pipeline(scriptText);
				} //if
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	public bool Run_Command(List<string> scriptText,ref List<string> outputlist) 
	{
		bool res=false;
		string newstring="";
		try {
			for (int i=0;i<scriptText.Count;i++) {
				if (newstring!="") newstring=newstring+"\n";
				newstring=newstring+scriptText[i];
			}
			if (newstring!="") res=Run_Command(newstring,ref outputlist);
			
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	
	public bool Run_Command(string scriptText,ref List<string> outputlist)
	{
		bool res=false;
		string outputfile="";
		string newscripttext="";
		string command="";
		try {
			outputlist.Clear();
			newscripttext=scriptText;
			outputfile=Get_Temporary_File();
			if (outputfile!="") {
				outputfile=outputfile+".tmp";
				PowerShell ps = PowerShell.Create();
				command=newscripttext+@" | Out-File "+SQL_Single_String(outputfile)+" -Width 5000";
				//Write_File(RootFolder()+"powershell-command.txt",command,false);
				ps.AddScript(command);
				var results=ps.Invoke();
				Thread.Sleep(500);
				if (Read_File_List(outputfile,ref outputlist)) {
					if (File_Delete(outputfile)) {
						res=true;
					} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Unable to delete temporay file: "+outputfile);
				}
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Unable to get temporay file");
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}

	private bool Run_Command_No_Pipeline(string scriptText)
	{
		bool res=false;
		string outputfile="";
		try {
			if (scriptText!="") {
				outputfile=Get_Temporary_File();
				if (outputfile!="") {
					outputfile=outputfile+".tmp";
					PowerShell ps = PowerShell.Create();
					//Write_File(RootFolder()+"powershell-command.txt",scriptText,false);
					ps.AddScript(scriptText);
					var results=ps.Invoke();
					Thread.Sleep(500);
						if (File_Delete(outputfile)) {
							res=true;
						} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Unable to delete temporay file: "+outputfile);
				} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Unable to get temporay file");
			}
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	public bool Run_Script(List<string> scriptlist,ref List<string> outputlist)
	{
		bool res=false;
		string scriptfile="";
		string outputfile="";
		//string command="";
		try {
			outputlist.Clear();
			outputfile=Get_Temporary_File();
			if (outputfile!="") {
				scriptfile=outputfile+".ps1";
				outputfile=outputfile+".tmp";
				scriptlist.Insert(0,"$ErrorActionPreference="+SQL_String("SilentlyContinue"));
				scriptlist.Insert(1,"Stop-Transcript | out-null");
				scriptlist.Insert(2,"$ErrorActionPreference = "+SQL_String("Continue"));
				scriptlist.Insert(3,"Start-Transcript -path "+outputfile+" -append");
				scriptlist.Add("Stop-Transcript");
				if (Write_File_List(scriptfile,ref scriptlist,false)) {
					PowerShell ps = PowerShell.Create();
					ps.AddScript(scriptfile);
					var results=ps.Invoke();
					Thread.Sleep(500);
					if (Read_File_List(outputfile,ref outputlist)) {
						if (File_Delete(scriptfile)) {
							if (File_Delete(outputfile)) {
								res=true;
							} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Unable to delete temporay file: "+outputfile);
						} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Unable to delete temporay file: "+scriptfile);
					}
				}
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Unable to get temporay file");
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	
	public bool Run_Script_With_Credentials_old(ref List<string> scriptlist,ref List<string> outputlist,bool usecredentials)
	{
		bool res=false;
		string scriptfile="";
		string outputfile="";
		//string command="";
		try {
			outputlist.Clear();
			outputfile=Get_Temporary_File();
			if (outputfile!="") {
				scriptfile=outputfile+".ps1";
				outputfile=outputfile+".tmp";
				
				scriptlist.Insert(0,"$ErrorActionPreference="+SQL_String("SilentlyContinue"));
				scriptlist.Insert(1,"Stop-Transcript | out-null");
				scriptlist.Insert(2,"$ErrorActionPreference = "+SQL_String("Continue"));
				scriptlist.Insert(3,"Start-Transcript -path "+outputfile+" -append");
				scriptlist.Insert(4,"$cred = new-object -typename System.Management.Automation.PSCredential -argumentlist "+SQL_Single_String(@"pwus\s-adem")+","+ SQL_Single_String("oG_JuxiOT9Gk1EoYQ9QNrCvU782G5J"));
				scriptlist.Insert(5,@"Invoke-Command -ComputerName VMP7981QA011.utcaus.com -ScriptBlock { Get-Process } -credential $cred");
				scriptlist.Add("Stop-Transcript");
				if (Write_File_List(scriptfile,ref scriptlist,false)) {
					PowerShell ps = PowerShell.Create();
					ps.AddScript(scriptfile);
					var results=ps.Invoke();
					Thread.Sleep(500);
					if (Read_File_List(outputfile,ref outputlist)) {
						if (File_Delete(outputfile)) {
							if (File_Delete(outputfile)) {
								res=true;
							} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Unable to delete temporay file: "+outputfile);
						} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Unable to delete temporay file: "+scriptfile);
					} else res=true;
				}
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Unable to get temporay file");
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	private bool Parse_String_Array(string source,string tag,ref List<string> thearray) {
		bool res=false;
		int pos=0;
		string newsource=source;
		string temp="";
		try {
			thearray.Clear();
			pos=newsource.IndexOf(tag);
			if (pos>-1) {
				do {
					temp=newsource.Substring(0,pos);
					thearray.Add(temp.Trim());
					newsource=newsource.Substring(pos+tag.Length);
					pos=newsource.IndexOf(tag);
					if (pos==-1 && newsource!="") thearray.Add(newsource.Trim()); //add the last piece of string to the array
				} while ( pos>-1);
				res=true;
			} else {
				thearray.Add(source); //no tag found, so add the hole string to the array
				res=true;
			}
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		
		return res;
	}
	private string Get_Temporary_File(string basefolder="") {
		string temppath="";
		string tempfile="";
		string res="";
		int ra=0;
		Random random = new Random();
		try {
			if (basefolder=="") temppath = temppath = RootFolder()+@"temp\";  //Get_Windows_Temp_Folder();
			else temppath=basefolder;
			if (temppath!="") {
				do {
					ra=random.Next(0, 1000);
					tempfile=temppath+@"temp-"+ra.ToString()+"-"+DateTime.Now.ToString("yyyyMMddHHmmss");
					if (File.Exists(temppath)==false) {
						res=tempfile;
						break;
					}
				} while (File.Exists(tempfile));
			}
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	
	
	private bool Read_File_List(string filename,ref List<string> thelist)
	{
		bool res = false;
		string line="";
		thelist.Clear();
		try
		{
			if (File.Exists(filename)) {
				StreamReader reader = File.OpenText(filename);
				while ((line = reader.ReadLine()) != null) {
					if (line!="")	thelist.Add(line);
				}
				reader.Close();
				res = true;
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: File not found: "+filename);
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Reading File: "+arg.Message);
		}
		return res;
	}
	
	private bool File_Delete(string filename)
	{
		bool res = false;
		try
		{
			if (File.Exists(filename))
			{
				File.SetAttributes(filename, FileAttributes.Normal);
				File.Delete(filename);
				res = true;
			}
			else res = true;
			

		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Deleting file:"+filename);
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}



		return res;
	}
	private bool Read_File(string filename,ref string thetext)
	{
		bool res = false;
		try
		{
			StreamReader reader = File.OpenText(filename);
			thetext= reader.ReadToEnd();
			reader.Close();
			res = true;
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Reading File: "+arg.Message);
		}
		return res;
	}
	
	private string Get_Windows_Temp_Folder() {
		string res="";
		try {
			res = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			res=Add_Last_Slash(res);
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	private string RootFolder() {
		string res=System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location);
		if (res.Last().ToString()!=@"\") res=res+@"\";
		return res;
	}
	private string Add_Last_Slash(string value) {
		string res=value;
		if (string.IsNullOrEmpty(res)==false && res.Last().ToString()!=@"\") res=res+@"\";
		return res;
	}

	public bool Start_Process(string computer,string processname,string processfilename,string parameters="",int wait_seconds_after_start=5) {
		bool res=false;
		string outputstring="";
		List<string> outputlist=new List<string>();
		List<string> commandlist=new List<string>();
		string pwcommand="";
		string pwparameters="";
		int pos=-1;
		int retry=0;
		string pid="";
		try {
			StaticFunctions.Writelog("Starting Process ["+processname+"]");
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				pwcommand="Start-Process -FilePath "+SQL_String(processfilename);
				if (parameters!="") pwcommand=pwcommand+" -ArgumentList "+parameters;
			}
			else {
				pwcommand="Invoke-WmiMethod –ComputerName "+computer+" -Class win32_process -Name create -ArgumentList "+SQL_String(processfilename);
			}
			if (Run_Command(pwcommand,ref outputlist)) {
				Wait_Seconds(wait_seconds_after_start);
				if(Process_is_Running(computer,processname)) {
					StaticFunctions.Writelog("Starting Process ["+processname+"] OK");
					res=true;
				} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Starting Process ["+processname+"] FAILED");
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+processname);
			
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		
		return res;
	}
	
	public bool Stop_Process(string computer,string processname,int wait_seconds_after_stop=5) {
		bool res=false;
		bool found=false;
		string outputstring="";
		List<string> outputlist=new List<string>();
		string pwcommand="";
		int retry=0;
		string processid="";
		try {
			StaticFunctions.Writelog("Stopping Process ["+processname+"]");
			if (Get_Process_Id(computer,processname,ref processid)) {
				if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
					pwcommand="Stop-Process -Id "+processid; //pwcommand="Stop-Process -Name "+processname;
				}
				else {
					pwcommand="Invoke-Command -ComputerName "+computer+" {Stop-Process -id "+processid+"}";
					
				}
				do {
					retry++;
					if (Run_Command(pwcommand,ref outputlist)) {
						Wait_Seconds(wait_seconds_after_stop);
						found=Process_is_Running(computer,processname);
						if (found==false) {
							StaticFunctions.Writelog("Stopping Process ["+processname+"] OK");
							res=true;
						} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Stopping Process ["+processname+"] FAILED");
					} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+pwcommand); //run command
				} while (res!=true && retry<5);
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Getting Process ID for: "+processname); //run command
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	
	public bool Stop_Process(string computer,int processid,int wait_seconds_after_stop=5) {
		bool res=false;
		bool found=false;
		string outputstring="";
		List<string> outputlist=new List<string>();
		string pwcommand="";
		int retry=0;
		try {
			StaticFunctions.Writelog("Stopping Process ID ["+processid.ToString()+"]");
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				pwcommand="Stop-Process -Id "+processid.ToString()+" -Force"; //pwcommand="Stop-Process -Name "+processname;
			}
			else {
				pwcommand="Invoke-Command -ComputerName "+computer+" {Stop-Process -Id "+processid.ToString()+" -Force "+"}";
			}
			do {
				retry++;
				if (Run_Command(pwcommand,ref outputlist)) {
					Wait_Seconds(wait_seconds_after_stop);
					found=Process_is_Running(computer,processid);
					if (found==false) {
						StaticFunctions.Writelog("Process ID ["+processid.ToString()+"] not running");
						res=true;
					} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Stopping Process ID ["+processid.ToString()+"] FAILED");
				} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+pwcommand); //run command
			} while (res!=true && retry<5);
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	public bool Start_Service(string computer,string servicename,int wait_seconds_after_start=5) {
		bool res=false;
		bool found=false;
		string outputstring="";
		List<string> outputlist=new List<string>();
		string pwcommand="";
		int retry=0;
		try {
			StaticFunctions.Writelog("Start Service ["+servicename+"]");
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				pwcommand="Start-Service -Name "+SQL_String(servicename);
			}
			else {
				pwcommand="Start-Service -inputobject $(get-service -ComputerName "+computer+" -Name "+SQL_String(servicename)+")";
			}
			do {
				retry++;
				if (Run_Command(pwcommand,ref outputlist)) {
					Wait_Seconds(wait_seconds_after_start);
					found=Service_is_Running(computer,servicename);
					if (found) {
						StaticFunctions.Writelog("Starting Service ["+servicename+"] OK");
						res=true;
					} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Starting Service ["+servicename+"] FAILED");
				} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+pwcommand); //run command
			} while (res!=true && retry<5);
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	
	public bool Stop_Service(string computer,string servicename,int wait_seconds_after_stop=5) {
		bool res=false;
		bool found=false;
		string outputstring="";
		List<string> outputlist=new List<string>();
		string pwcommand="";
		int retry=0;
		try {
			StaticFunctions.Writelog("Stopping Service ["+servicename+"]");
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				pwcommand="Stop-Service -Name "+SQL_String(servicename);
			}
			else {
				pwcommand="Stop-Service -inputobject $(get-service -ComputerName "+computer+" -Name "+SQL_String(servicename)+")";
			}
			do {
				retry++;
				if (Run_Command(pwcommand,ref outputlist)) {
					Wait_Seconds(wait_seconds_after_stop);
					found=Service_is_Running(computer,servicename);
					if (found==false) {
						StaticFunctions.Writelog("Stopping Service ["+servicename+"] OK");
						res=true;
					} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Stopping Service ["+servicename+"] FAILED");
				} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+pwcommand); //run command
			} while (res!=true && retry<5);
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	
	public bool Service_is_Running(string computer, string servicename)
	{
		bool res=false;
		try {
			
			StaticFunctions.Writelog("Checking if Service ["+servicename+"] is running");
			PowerShell ps = PowerShell.Create();
			ps.AddCommand("Get-Service");
			ps.AddParameter("Name",servicename);
			if (computer!="" && Get_Host_Name_From_Name(computer).ToUpper()!=Get_Host_Name().ToUpper()) ps.AddParameter("ComputerName",computer);
			IAsyncResult async = ps.BeginInvoke();
			foreach (PSObject result in ps.EndInvoke(async))
			{
				if (result.Members["Name"].Value.ToString().ToUpper()==servicename.ToUpper() ) {
					if (result.Members["Status"].Value.ToString().ToUpper()=="RUNNING") {
						res=true;
						break;
					}
				}
			}
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	public bool Get_Service_Status(string computer, string servicename,ref string status)
	{
		bool res=false;
		status="";
		try {
			
			StaticFunctions.Writelog("Checking Service ["+servicename+"] Status");
			PowerShell ps = PowerShell.Create();
			ps.AddCommand("Get-Service");
			ps.AddParameter("Name",servicename);
			if (computer!="" && Get_Host_Name_From_Name(computer).ToUpper()!=Get_Host_Name().ToUpper()) ps.AddParameter("ComputerName",computer);
			IAsyncResult async = ps.BeginInvoke();
			foreach (PSObject result in ps.EndInvoke(async))
			{
				if (result.Members["Name"].Value.ToString().ToUpper()==servicename.ToUpper() ) {
					status=result.Members["Status"].Value.ToString().ToUpper();
					res=true;
					break;
				}
			}
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}

	public bool Process_is_Running(string computer, string processname)
	{
		bool res=false;
		int tempint=0;
		try {
			
			StaticFunctions.Writelog("Checking if Process ["+processname+"] is running");
			PowerShell ps = PowerShell.Create();
			ps.AddCommand("Get-Process");
			ps.AddParameter("ProcessName",processname);
			if (computer!="" && Get_Host_Name_From_Name(computer).ToUpper()!=Get_Host_Name().ToUpper()) ps.AddParameter("ComputerName",computer);
			IAsyncResult async = ps.BeginInvoke();
			foreach (PSObject result in ps.EndInvoke(async))
			{
				//	StaticFunctions.Writelog(result.Members["ProcessName"].Value.ToString()+",ID="+result.Members["Id"].Value.ToString());
				if (result.Members["ProcessName"].Value.ToString().ToUpper()==processname.ToUpper() ) {
					if (Int32.TryParse(result.Members["Id"].Value.ToString(),out tempint)) {
						res=true;
						break;
					}
				}
			}
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	

	public bool Process_is_Running(string computer, int processid)
	{
		bool res=false;
		int tempint=0;
		try {
			
			StaticFunctions.Writelog("Checking if Process ID["+processid.ToString()+"] is running");
			PowerShell ps = PowerShell.Create();
			ps.AddCommand("Get-Process");
			ps.AddParameter("Id",processid.ToString());
			if (computer!="" && Get_Host_Name_From_Name(computer).ToUpper()!=Get_Host_Name().ToUpper()) ps.AddParameter("ComputerName",computer);
			IAsyncResult async = ps.BeginInvoke();
			foreach (PSObject result in ps.EndInvoke(async))
			{
				if (Int32.TryParse(result.Members["Id"].Value.ToString(),out tempint)) {
					if (tempint==processid) {
						StaticFunctions.Writelog("Process Still Running: Process Name="+result.Members["ProcessName"].Value.ToString()+",ID="+result.Members["Id"].Value.ToString());
						res=true;
						break;
					}
				}
				
			}
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}

	public bool Get_Process_Running_Count(string computer, string processname,ref int counter)
	{
		bool res=false;
		int tempint=0;
		try {
			counter=0;
			StaticFunctions.Writelog("Checking if Process ["+processname+"] is running");
			PowerShell ps = PowerShell.Create();
			ps.AddCommand("Get-Process");
			ps.AddParameter("ProcessName",processname);
			if (computer!="" && Get_Host_Name_From_Name(computer).ToUpper()!=Get_Host_Name().ToUpper()) ps.AddParameter("ComputerName",computer);
			IAsyncResult async = ps.BeginInvoke();
			foreach (PSObject result in ps.EndInvoke(async))
			{
				//	StaticFunctions.Writelog(result.Members["ProcessName"].Value.ToString()+",ID="+result.Members["Id"].Value.ToString());
				if (result.Members["ProcessName"].Value.ToString().ToUpper()==processname.ToUpper() ) {
					if (Int32.TryParse(result.Members["Id"].Value.ToString(),out tempint)) {
						counter++;
					}
				}
			}
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	
	public bool Get_Process_Id(string computer, string processname,ref string process_id)
	{
		bool res=false;
		int tempint=0;
		try {
			
			StaticFunctions.Writelog("Getting Process ID for: ["+processname+"]");
			PowerShell ps = PowerShell.Create();
			ps.AddCommand("Get-Process");
			ps.AddParameter("ProcessName",processname);
			if (computer!="" && Get_Host_Name_From_Name(computer).ToUpper()!=Get_Host_Name().ToUpper()) ps.AddParameter("ComputerName",computer);
			IAsyncResult async = ps.BeginInvoke();
			foreach (PSObject result in ps.EndInvoke(async))
			{
				//	StaticFunctions.Writelog(result.Members["ProcessName"].Value.ToString()+",ID="+result.Members["Id"].Value.ToString());
				if (result.Members["ProcessName"].Value.ToString().ToUpper()==processname.ToUpper() ) {
					if (Int32.TryParse(result.Members["Id"].Value.ToString(),out tempint)) {
						process_id=tempint.ToString();
						res=true;
						break;
					}
				}
			}
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	private void Wait_Seconds(int seconds) {
		int passseconds=0;
		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Start();
		do {
			Thread.Sleep(200);
			TimeSpan ts = stopWatch.Elapsed;
			passseconds=Convert.ToInt32(ts.TotalSeconds);
			System.Windows.Forms.Application.DoEvents();
		} while (passseconds<seconds);
		stopWatch.Stop();
	}
	private string SQL_String(string sourcetext) {
		string res="\""+sourcetext+"\"";
		return res;
	}
	private string SQL_Single_String(string sourcetext) {
		string res="'"+sourcetext+"'";
		return res;
	}
	private bool Find_Service_Value(ref List<string> outputlist,string servicename,ref string output_value) {
		bool res=false;
		int pos1=-1;
		int pos2=-1;
		int nextblankspace1=-1;
		int nextblankspace2=-1;
		string value1="";
		string value2="";
		int detailline=-1;
		try {
			output_value="";
			if (outputlist.Count>2) {
				for (int i=0;i<outputlist.Count;i++) {
					pos1=outputlist[i].ToUpper().IndexOf("STATUS");
					pos2=outputlist[i].ToUpper().IndexOf("NAME");
					if (pos1>-1 && pos2>-1)  {
						detailline=Find_Detail_Line(ref outputlist);
						if (detailline>i) {
							nextblankspace1=outputlist[detailline].Substring(pos1).IndexOf(" "); //find the next blank space
							if (nextblankspace1>0) {
								value1=outputlist[i+2].Substring(pos1,nextblankspace1).Trim();
								nextblankspace2=outputlist[detailline].Substring(pos2).IndexOf(" ");
								if (nextblankspace2>0) {
									value2=outputlist[detailline].Substring(pos2,nextblankspace2).Trim();
									if (servicename.ToUpper()==value2.ToUpper()) {
										output_value=value1;
										res=true;
										break;
									}
								}
							}
							
							
						} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Detail line not found");
						
					} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Header line not found");
					
				} //for each
			}
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	private bool Find_Process_Value(ref List<string> outputlist,string processname,ref string output_value) {
		bool res=false;
		int pos1=-1;
		int nextblankspace1=-1;
		string value1="";
		int detailline=-1;
		try {
			output_value="";
			if (outputlist.Count>2) {
				for (int i=0;i<outputlist.Count;i++) {
					pos1=outputlist[i].ToUpper().IndexOf("PROCESSNAME");
					if (pos1>-1)  {
						detailline=Find_Detail_Line(ref outputlist);
						if (detailline>i) {
							nextblankspace1=outputlist[detailline].Substring(pos1).IndexOf(" "); //find the next blank space
							if (nextblankspace1>0) {
								value1=outputlist[detailline].Substring(pos1,nextblankspace1).Trim();
							}
							if (processname.ToUpper()==value1.ToUpper()) {
								output_value=value1;
								res=true;
							}
						} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Detail line not found");
						
					} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Header line not found");
					
				} //for each
			}
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	private bool Find_Process_Id_Value(ref List<string> outputlist,string processname,ref string output_value) {
		bool res=false;
		int pos1=-1;
		int nextblankspace1=-1;
		string value1="";
		int tempvalue=0;
		int detailline=-1;
		try {
			output_value="";
			if (outputlist.Count>2) {
				for (int i=0;i<outputlist.Count;i++) {
					pos1=outputlist[i].ToUpper().IndexOf("ID");
					if (pos1>-1)  {
						detailline=Find_Detail_Line(ref outputlist);
						if (detailline>i) {
							nextblankspace1=outputlist[detailline].Substring(0,pos1+2).LastIndexOf(" "); //find last blank spcace
							if (nextblankspace1>0) {
								value1=outputlist[detailline].Substring(nextblankspace1,(pos1+2)-nextblankspace1).Trim();
								if (value1!="" && Int32.TryParse(value1,out tempvalue) ) {
									output_value=tempvalue.ToString();
									res=true;
									break;
								}

							}
						} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Detail line not found");
						
					} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Header line not found");
					
				} //for each
			}
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	private int Find_Detail_Line(ref List<string> outputlist) {
		int res=-1;
		try {
			for (int i=0;i<outputlist.Count;i++) {
				if (outputlist[i].Substring(0,6)=="------") {
					if (i+1<=outputlist.Count-1) res=i+1; //the details are in the next line after the ------------
					break;
				}
			}
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	public bool Server_Last_Rebooted_DateTime(string computer, ref DateTime Rebooted_Datetime)
	{
		bool res=false;
		int tempint=0;
		int found=0;
		DateTime tempdate=DateTime.MinValue;
		string message="";
		string message1="has initiated the restart of computer".ToUpper();
		string message2="The previous system shutdown".ToUpper();
		string[] Source_params = { "User32", "EventLog" };
		try {

			Rebooted_Datetime=DateTime.MinValue;
			StaticFunctions.Writelog("Checking Last Time Server was Rebooted");
			PowerShell ps = PowerShell.Create();
			//ps.AddCommand("Get-EventLog -LogName System -Source User32,EventLog | Format-Table -Property TimeGenerated,Message –AutoSize");
			ps.AddCommand("Get-EventLog");
			ps.AddParameter("LogName","System");
			ps.AddParameter("Source",Source_params);
			if (computer!="" && Get_Host_Name_From_Name(computer).ToUpper()!=Get_Host_Name().ToUpper()) ps.AddParameter("ComputerName",computer);
			IAsyncResult async = ps.BeginInvoke();
			foreach (PSObject result in ps.EndInvoke(async))
			{
				found++;
				message=result.Members["Message"].Value.ToString().ToUpper();
				if (message.Contains(message1) || message.Contains(message2) ) {
					if (DateTime.TryParse(result.Members["TimeGenerated"].Value.ToString(),out Rebooted_Datetime)) {
						//StaticFunctions.Writelog(result.Members["TimeGenerated"].Value.ToString()+",Message="+result.Members["Message"].Value.ToString());
						res=true;
						break;
					}
				}
			}
			StaticFunctions.Writelog("Records Found="+found.ToString());
			if (res==false) StaticFunctions.Writelog("Server was not rebooted");
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	private string Get_Host_Name() {
        	 return System.Net.Dns.GetHostName().ToUpper();
        	//return System.Net.Dns.GetHostEntry("").HostName.ToUpper(); //Return the Full Host Name
	}
	private string Get_Host_Name_From_Name(string computer) {
		int pos=-1;
		string res="";
		pos=computer.IndexOf(".");
		if (pos>-1) {
			res=computer.Substring(0,pos);
		} else res=computer;
		return res;
	}
	
	public string Get_Elapsed_Time_Formatted(DateTime start_date,DateTime end_date)
	{
		string res = "";
		try
		{
			TimeSpan ts =(end_date - start_date);
			if (ts.Days>0) {
				res = String.Format("Days({0}), {1:00}:{2:00}:{3:00}", ts.Days,ts.Hours, ts.Minutes, ts.Seconds);
			} else {
				res = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
			}
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	
	

	public bool Change_Task_State(string computer, string taskpath,string taskname, TASK_STATE state,ref TaskStruct output_taskrecord)
	{
		bool res=false;
		string command="";
		string finalstate="";
		List<string> outputlist=new List<string>();
		List<string> detaillist=new List<string>();
		TaskStruct ts=new TaskStruct();
		string commandstate="";
		string whereparams="";
		try {
			
			StaticFunctions.Writelog("Stopping Task ["+taskpath+taskname+"]");
			if (state==TASK_STATE.STOP) commandstate="Stop-ScheduledTask ";
			if (state==TASK_STATE.START) commandstate="Start-ScheduledTask ";
			if (state==TASK_STATE.ENABLE) commandstate="Enable-ScheduledTask ";
			if (state==TASK_STATE.DISABLE) commandstate="Disable-ScheduledTask ";
			if (taskpath!="") whereparams=whereparams+" -TaskPath "+SQL_Single_String(taskpath);
			
			if (taskname!="") whereparams=whereparams+" -TaskName "+SQL_Single_String(taskname);
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				command=commandstate+whereparams;
			} else {
				command="Invoke-Command -ComputerName "+computer+" { "+commandstate+whereparams +" } ";
			}
			command=command+" | Format-Table "+SQL_Single_String("|")+",TaskPath,"+SQL_Single_String("|")+",TaskName,"+SQL_Single_String("|")+",State,"+SQL_Single_String("|")+",Description,"+SQL_Single_String("|")+" –AutoSize ";
			StaticFunctions.Writelog("Command= "+command);
			if (Run_Command(command,ref outputlist)) {
				if (Get_Task(computer,taskpath,taskname,ref ts)) {
					output_taskrecord=ts;
					if (state==TASK_STATE.START && (ts.STATE.ToUpper()=="RUNNING" || ts.STATE.ToUpper()=="READY")) res=true;
					if (state==TASK_STATE.STOP && ts.STATE.ToUpper()!="RUNNING") res=true;
					if (state==TASK_STATE.ENABLE && (ts.STATE.ToUpper()=="RUNNING" || ts.STATE.ToUpper()=="READY")) res=true;
					if (state==TASK_STATE.DISABLE && ts.STATE.ToUpper()=="DISABLED") res=true;
				}
				
				
				//------------------------------------------
				
				
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+command);//run command
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}

	public bool Change_Task_Credentials(string computer, string taskpath,string taskname,string username,string password)
	{
		bool res=false;
		string command="";
		string finalstate="";
		List<string> outputlist=new List<string>();
		List<string> detaillist=new List<string>();
		TaskStruct ts=new TaskStruct();
		string commandstate="";
		TaskStruct temptask=new TaskStruct();
		string whereparams="";
		string line="";
		int start=-1;
		string value="";
		string tag="";
		try {
			if (username==USERNAME_SYSTEM) {
				username=USERNAME_SYSTEM_FULLNAME;
				password=""; //System don't use password
			}
			StaticFunctions.Writelog("Change Task Credentials ["+taskpath+taskname+"]");
			commandstate="Set-ScheduledTask ";
			if (taskpath!="") whereparams=whereparams+" -TaskPath "+SQL_Single_String(taskpath);
			if (taskname!="") whereparams=whereparams+" -TaskName "+SQL_Single_String(taskname);
			if (username!="") whereparams=whereparams+" -User "+SQL_Single_String(username);
			if (password!="") whereparams=whereparams+" -Password "+SQL_Single_String(password);
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				command=commandstate+whereparams;
			} else {
				command="Invoke-Command -ComputerName "+computer+" { "+commandstate+whereparams +" }";
			}
			command=command+" | Format-List ";
			StaticFunctions.Writelog("Command= "+command);
			if (Run_Command(command,ref outputlist)) {
				if (outputlist.Count>0) {
					temptask.COMPUTER_NAME="";
					temptask.TASK_PATH="";
					temptask.TASK_NAME="";
					for (int i=0;i<outputlist.Count;i++) {
						line=outputlist[i];
						start = line.IndexOf(":");
						tag= line.Substring(0, start).Trim();
						value= line.Substring(start+1).Trim();
						if (start>0) {
							switch (tag.ToUpper()) {
								case "TASKPATH":
									temptask.TASK_PATH=value;
									break;
								case "TASKNAME":
									temptask.TASK_NAME=value;
									break;
								case "PSCOMPUTERNAME":
									temptask.COMPUTER_NAME=value;
									break;
									
								default:
									break;
							}
						} //start>0
					} //for i
					if (temptask.TASK_PATH.ToUpper()==taskpath.ToUpper() && temptask.TASK_NAME.ToUpper()==taskname.ToUpper() ) {
						if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
							res=true;
						} else if (temptask.COMPUTER_NAME.ToUpper()==computer.ToUpper()) {
							res=true;
						}
					}
				} //count>0

			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running command: "+command); //run command
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	
	public bool Test_Computer_Connection(string computer)
	{
		bool res=false;
		string command="";
		string finalstate="";
		List<string> outputlist=new List<string>();
		List<string> detaillist=new List<string>();
		int headerrow=-1;
		string commandstate="";
		string whereparams="";
		string line="";
		int start=-1;
		string value="";
		string tag="";
		string tcpvalue="";
		try {
			//test-netconnection -ComputerName "10.165.20.218" -Port 80 -InformationLevel "Detailed"
			StaticFunctions.Writelog("Test Net Connection ["+computer+"]");
			commandstate="Test-NetConnection -ComputerName "+SQL_String(computer)+ " -Port 80 -InformationLevel "+SQL_String("Detailed");
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				command=commandstate+whereparams;
			} else {
				command="Invoke-Command -ComputerName "+computer+" { "+commandstate+whereparams +" }";
			}
			command=command+" | Format-List ";
			StaticFunctions.Writelog("Command= "+command);
			if (Run_Command(command,ref outputlist)) {
				if (outputlist.Count>0) {
					for (int i=0;i<outputlist.Count;i++) {
						line=outputlist[i];
						start = line.IndexOf(":");
						tag= line.Substring(0, start).Trim();
						value= line.Substring(start+1).Trim();
						if (start>0) {
							switch (tag.ToUpper()) {
								case "TcpTestSucceeded":
									tcpvalue=value;
									break;
									
								default:
									break;
							}
						} //start>0
					} //for i
					if (tcpvalue.ToUpper()=="TRUE") {
						res=true;
					}
				} //count>0

			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running command: "+command); //run command
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	
	private string Get_Task_State_Description(string statenumber) {
		string res="";
		switch (statenumber) {
			case "1":
				res="DISABLED";
				break;
			case "4":
				res="RUNNING";
				break;
			case "3":
				res="READY";
				break;
			default:
				res="";
				break;
		}
		return res;
	}
	
	
	public bool Get_Task_Array(string computer,ref List<TaskStruct> outputtask)
	{
		bool res=false;
		string command="";
		int headerrow=-1;
		string finalrunas="";
		string line="";
		int start=-1;
		List<string> outputlist=new List<string>();
		List<string> detaillist=new List<string>();
		string tag="";
		string value="";
		TaskStruct temptask=new TaskStruct();
		try {
			StaticFunctions.Writelog("Getting All Task");
			outputtask.Clear();
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || computer.ToUpper()=="LOCALHOST" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				command="Get-ScheduledTask ";
				command=command+" | Select-Object TaskPath,TaskName,State,Author,{$_.Principal.UserId},{$_.Principal.Logontype},Description | Format-List";
			} else {
				command="Invoke-Command -ComputerName "+computer+" {Get-ScheduledTask ";
				command=command+" | Select-Object TaskPath,TaskName,State,Author,{$_.Principal.UserId},{$_.Principal.Logontype},Description | Format-List";
				command=command+" } ";
			}
			
			StaticFunctions.Writelog("Command= "+command);
			if (Run_Command(command,ref outputlist)) {
				if (outputlist.Count>0) {
					for (int i=0;i<outputlist.Count;i++) {
						line=outputlist[i];
						start = line.IndexOf(":");
						tag= line.Substring(0, start).Trim();
						value= line.Substring(start+1).Trim();
						if (start>0) {
							switch (tag.ToUpper()) {
								case "TASKPATH":
									temptask=default(TaskStruct);
									temptask.COMPUTER_NAME=computer.ToUpper();
									temptask.PASSWORD_UPDATED="No";
									temptask.TASK_PATH=value;
									temptask.EMPTY_TASK="FALSE";
									temptask.DELETED_TASK="FALSE";
									break;
								case "TASKNAME":
									temptask.TASK_NAME=value;
									break;
								case "STATE":
									temptask.STATE=value;
									break;
								case "AUTHOR":
									temptask.AUTHOR=value;
									break;
								case "$_.PRINCIPAL.USERID":
									temptask.RUNASUSER=value;
									break;
								case "$_.PRINCIPAL.LOGONTYPE":
									temptask.LOGON_TYPE=value;
									break;
								case "DESCRIPTION":
									temptask.DESCRIPTION=value;
									outputtask.Add(temptask);
									temptask=default(TaskStruct);
									break;
									
								default:
									break;
							}
						} //start>0
					} //for i
				} else { //count>0
					temptask.COMPUTER_NAME=computer.ToUpper();
					temptask.TASK_PATH="";
					temptask.TASK_NAME="";
					temptask.STATE="";
					temptask.AUTHOR="";
					temptask.RUNASUSER="";
					temptask.LOGON_TYPE="";
					temptask.PASSWORD_UPDATED="";
					temptask.DESCRIPTION="";
					temptask.EMPTY_TASK="TRUE";
					temptask.DELETED_TASK="FALSE";
					outputtask.Add(temptask);
					temptask=default(TaskStruct);
				}
				res=true;
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+command);//run command
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	
	public bool Get_Task(string computer,string taskpath,string taskname,ref TaskStruct outputtask)
	{
		bool res=false;
		string command="";
		string line="";
		int start=-1;
		List<string> outputlist=new List<string>();
		string tag="";
		string value="";
		string whereclause="";
		string whereparams="";
		TaskStruct temptask=new TaskStruct();
		try {
			outputtask=default(TaskStruct);
			//StaticFunctions.Writelog("Getting Task Info");
			if (taskpath!="" || taskname!="") {
				whereclause=whereclause +" | Where-Object { ";
				if (taskpath!="") {
					if (whereparams!="") whereparams=whereparams+" -and ";
					whereparams=whereparams+" $_.TaskPath -eq "+SQL_Single_String(taskpath);
				}
				if (taskname!="") {
					if (whereparams!="") whereparams=whereparams+" -and ";
					whereparams=whereparams+" $_.TaskName -eq "+SQL_Single_String(taskname);
				}
				whereclause=whereclause+whereparams+ " } ";
			} //where clause
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				command="Get-ScheduledTask ";
				command=command+" | Select-Object TaskPath,TaskName,State,{$_.Principal.UserId},{$_.Principal.Logontype},Description ";
				command=command+whereclause+" | Format-List";
			} else {
				command="Invoke-Command -ComputerName "+computer+" {Get-ScheduledTask ";
				command=command+" | Select-Object TaskPath,TaskName,State,{$_.Principal.UserId},{$_.Principal.Logontype},Description ";
				command=command+whereclause+" | Format-List";
				command=command+" } ";
			}
			
			//	StaticFunctions.Writelog("Command= "+command);
			if (Run_Command(command,ref outputlist)) {
				if (outputlist.Count>0) {
					for (int i=0;i<outputlist.Count;i++) {
						line=outputlist[i];
						start = line.IndexOf(":");
						tag= line.Substring(0, start).Trim();
						value= line.Substring(start+1).Trim();
						if (start>0) {
							switch (tag.ToUpper()) {
								case "TASKPATH":
									temptask.COMPUTER_NAME=computer.ToUpper();
									temptask.PASSWORD_UPDATED="No";
									temptask.EMPTY_TASK="FALSE";
									temptask.DELETED_TASK="FALSE";
									temptask.TASK_PATH=value;
									break;
								case "TASKNAME":
									temptask.TASK_NAME=value;
									break;
								case "STATE":
									temptask.STATE=value;
									break;
								case "AUTHOR":
									temptask.AUTHOR=value;
									break;
								case "$_.PRINCIPAL.USERID":
									temptask.RUNASUSER=value;
									break;
								case "$_.PRINCIPAL.LOGONTYPE":
									temptask.LOGON_TYPE=value;
									break;
								case "DESCRIPTION":
									temptask.DESCRIPTION=value;
									break;
									
								default:
									break;
							}
						} //start>0
					} //for i
					if (temptask.TASK_PATH.ToUpper()==taskpath.ToUpper() && temptask.TASK_NAME.ToUpper()==taskname.ToUpper() ) {
						outputtask=temptask;
						temptask=default(TaskStruct);
						res=true;
					}
				} else { //count>0
					temptask.COMPUTER_NAME=computer.ToUpper();
					temptask.TASK_PATH="";
					temptask.TASK_NAME="";
					temptask.STATE="";
					temptask.AUTHOR="";
					temptask.RUNASUSER="";
					temptask.LOGON_TYPE="";
					temptask.DESCRIPTION="";
					temptask.PASSWORD_UPDATED="";
					temptask.EMPTY_TASK="TRUE";
					temptask.DELETED_TASK="FALSE";
					outputtask=temptask;
					temptask=default(TaskStruct);
					res=true;
				}
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+command);//run command
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}


	public bool Write_File_List(string filename,ref List<string> thelist,bool appenddata)
	{
		bool res = false;
		try
		{
			StreamWriter streamWriter = new StreamWriter(filename, appenddata);
			for (int i=0;i<thelist.Count;i++) {
				streamWriter.WriteLine(thelist[i]);
			}
			streamWriter.Close();
			res = true;
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Writting File error: "+arg.Message);
		}
		return res;
	}
	private bool Find_Header_Line(string headernames,ref List<string> outputlist,ref int headerrow) {
		bool res=false;
		List<string> headerlist=new List<string>();
		int found=0;
		try {
			headerrow=-1;
			//FOUND THE TITLES
			if (Parse_String_Array(headernames,",",ref headerlist)) {
				for (int i=0;i<outputlist.Count;i++) {
					found=0;
					for (int x=0;x<headerlist.Count;x++) {
						if (outputlist[i].ToUpper().Contains(headerlist[x].ToUpper()) ) {
							found++;
						}
					} //for x
					if (found==headerlist.Count) {
						headerrow=i;
						res=true;
						break;
					}
				} //for i
			}
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	private bool Find_Error_Message(List<string> outputlist) {
		bool res=false;

		try {
			//FOUND THE TITLES
				for (int i=0;i<outputlist.Count;i++) {
					if (outputlist[i].Contains("RemoteException") || outputlist[i].Contains("NativeCommandError") || outputlist[i].Contains("FullyQualifiedErrorId")) {
							res=true;
							break;
					}
				} //for i
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	private bool Get_Detail_Values(string headertext,string sourcetext,ref List<string> outputlist) {
		bool res=false;
		List<int> columnposition=new List<int>();
		string temptext="";
		int totalcolumns=0;
		int cols=0;
		try {
			outputlist.Clear();
			//Get the headers positions
			for (int i=0;i<headertext.Length;i++) {
				if (headertext.Substring(i,1)=="|") {
					columnposition.Add(i);
				}
			} //for i
			//Get the details
			for (int x=0;x<columnposition.Count;x++) {
				cols++;
				if (cols==2) {
					cols=1;
					if (sourcetext.Length>columnposition[x]) {
						temptext=sourcetext.Substring(columnposition[x-1],columnposition[x]-columnposition[x-1]);
						outputlist.Add(temptext.Trim());
					} else { 
						temptext=sourcetext.Substring(columnposition[x-1]);
						outputlist.Add(temptext.Trim());
						break;
					}
				} //cols==2
			} //for i
			
			//fill empty the remainig columns
			for (int i=0;i<columnposition.Count;i++) {
				if (outputlist.Count<i) {
					outputlist.Add("");
				}
			}
			res=true;
			
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Writting File error: "+arg.Message);
		}
		return res;
	}
	private int Find_Task_Index(string taskpath,string taskname,ref List<TaskStruct> inputlist) {
		int res=-1;
		try {
			for (int i=0;i<inputlist.Count;i++) {
				if (taskpath.ToUpper().Trim()==inputlist[i].TASK_PATH.ToUpper().Trim()) {
					if (taskname.ToUpper().Trim()==inputlist[i].TASK_NAME.ToUpper().Trim()) {
						res=i;
						break;
					}
				}
			}
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Writting File error: "+arg.Message);
		}
		return res;
	}
	public bool Parse_Task_String(string sourcepath,ref string computer,ref string taskpath,ref string taskname) {
		bool res=false;
		int startpos=0;
		int pos1=0;
		int pos2=0;
		string tempcomputer="";
		string temptaskpath="";
		string temptaskname="";
		List<string> list=new List<string>();
		string tempsourcepath=sourcepath;
		try {
			computer="";
			taskpath="";
			taskname="";
			if (tempsourcepath!="") {
				if (tempsourcepath.StartsWith(@"\\")) {
					tempsourcepath=tempsourcepath.Remove(0,2);
				}
				if (Parse_String_Array(tempsourcepath,@"\",ref list)) {
					if (list.Count>0) tempcomputer=list[0]; //servername
					if (list.Count>1) temptaskname=list[list.Count-1]; //task name
					//****************GET TASK PATH******************
					for (int x=1;x<list.Count-1;x++) {
						
						temptaskpath=temptaskpath+@"\"+list[x];
					}
					temptaskpath=Add_Last_Slash(temptaskpath);
					if (temptaskpath=="") temptaskpath=@"\";
				}
				if (tempcomputer!="" && temptaskpath!="" && temptaskname!="") {
					computer=tempcomputer;
					taskpath=temptaskpath;
					taskname=temptaskname;
					res=true;
				}
			} //tempsourcepath!=""
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}

	public bool Get_Task_Extra_Info_old(ref TaskStruct outputtask)
	{
		bool res=false;
		string command="";
		string line="";
		int start=-1;
		List<string> outputlist=new List<string>();
		string tag="";
		string value="";
		string whereclause="";
		string whereparams="";
		string temp_taskpath="";
		string temp_taskname="";
		string temp_lastruntime="";
		string temp_lasttaskresult="";
		string temp_nextruntime="";
		string temp_numberofmissedruns="";
		TaskStruct temptask=new TaskStruct();
		string computer="";
		string taskname="";
		string taskpath="";
		try {
			if (outputtask.EMPTY_TASK=="FALSE") {
				computer=outputtask.COMPUTER_NAME;
				taskpath=outputtask.TASK_PATH;
				taskname=outputtask.TASK_NAME;
				StaticFunctions.Writelog("Getting Task Extra Info");
				if (taskpath!="" || taskname!="") {
					if (taskpath!="") whereparams=whereparams+" -TaskPath "+SQL_Single_String(taskpath);
					if (taskname!="") whereparams=whereparams+" -TaskName "+SQL_Single_String(taskname);
				} //where clause
				if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
					command="Get-ScheduledTaskInfo ";
					command=command+whereclause+" | Format-List";
				} else {
					command="Invoke-Command -ComputerName "+computer+" {Get-ScheduledTaskInfo ";
					command=command+whereclause+" | Format-List";
					command=command+" } ";
				}
				
				StaticFunctions.Writelog("Command= "+command);
				if (Run_Command(command,ref outputlist)) {
					if (outputlist.Count>0) {
						for (int i=0;i<outputlist.Count;i++) {
							line=outputlist[i];
							start = line.IndexOf(":");
							tag= line.Substring(0, start).Trim();
							value= line.Substring(start+1).Trim();
							if (start>0) {
								switch (tag.ToUpper()) {
									case "TASKPATH":
										temp_taskpath=value;
										break;
									case "TASKNAME":
										temp_taskname=value;
										break;
									case "LASTRUNTIME":
										temp_lastruntime=value;
										break;
									case "LASTTASKRESULT":
										temp_lasttaskresult=value;
										break;
									case "NEXTRUNTIME":
										temp_nextruntime=value;
										break;
									case "NUMBEROFMISSEDRUNS":
										temp_numberofmissedruns=value;
										break;
										
									default:
										break;
								}
							} //start>0
						} //for i
						if (outputtask.TASK_PATH.ToUpper()==temp_taskpath.ToUpper() && outputtask.TASK_NAME.ToUpper()==temp_taskname.ToUpper() ) {
							temptask=outputtask;
							temptask.LAST_RUNTIME=temp_lastruntime;
							temptask.LAST_TASK_RESULT=temp_lasttaskresult;
							temptask.NEXT_RUNTIME=temp_nextruntime;
							temptask.NUMBER_OF_MISSED_RUNS=temp_numberofmissedruns;
							outputtask=temptask;
							res=true;
						}
					} //Count>0
				} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+command);//run command
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Task is empty ");//run command //TASK IS NOT EMPTY
		} //TRY
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	public bool Get_Task_Extra_Info(string computer,string taskpath,string taskname,ref TaskStruct outputtask)
	{
		bool res=false;
		string command="";
		string line="";
		int start=-1;
		List<string> outputlist=new List<string>();
		string tag="";
		string value="";
		string whereclause="";
		string whereparams="";
		string temp_taskpath="";
		string temp_taskname="";
		string temp_lastruntime="";
		string temp_lasttaskresult="";
		string temp_nextruntime="";
		string temp_numberofmissedruns="";
		TaskStruct temptask=new TaskStruct();
		try {
			StaticFunctions.Writelog("Getting Task Extra Info");
			if (taskpath!="" || taskname!="") {
				if (taskpath!="") whereparams=whereparams+" -TaskPath "+SQL_Single_String(taskpath);
				if (taskname!="") whereparams=whereparams+" -TaskName "+SQL_Single_String(taskname);
			} //where clause
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				command="Get-ScheduledTaskInfo ";
				command=command+whereclause+whereparams+" | Format-List";
			} else {
				command="Invoke-Command -ComputerName "+computer+" {Get-ScheduledTaskInfo ";
				command=command+whereclause+whereparams+" | Format-List";
				command=command+" } ";
			}
			
			StaticFunctions.Writelog("Command= "+command);
			if (Run_Command(command,ref outputlist)) {
				if (outputlist.Count>0) {
					for (int i=0;i<outputlist.Count;i++) {
						line=outputlist[i];
						start = line.IndexOf(":");
						tag= line.Substring(0, start).Trim();
						value= line.Substring(start+1).Trim();
						if (start>0) {
							switch (tag.ToUpper()) {
								case "TASKPATH":
									temp_taskpath=value;
									break;
								case "TASKNAME":
									temp_taskname=value;
									break;
								case "LASTRUNTIME":
									temp_lastruntime=value;
									break;
								case "LASTTASKRESULT":
									temp_lasttaskresult=value;
									break;
								case "NEXTRUNTIME":
									temp_nextruntime=value;
									break;
								case "NUMBEROFMISSEDRUNS":
									temp_numberofmissedruns=value;
									break;
									
								default:
									break;
							}
						} //start>0
					} //for i
					if (taskpath.ToUpper()==temp_taskpath.ToUpper() && taskname.ToUpper()==temp_taskname.ToUpper() ) {
						temptask=outputtask;
						temptask.LAST_RUNTIME=temp_lastruntime;
						temptask.LAST_TASK_RESULT=temp_lasttaskresult;
						temptask.NEXT_RUNTIME=temp_nextruntime;
						temptask.NUMBER_OF_MISSED_RUNS=temp_numberofmissedruns;
						outputtask=temptask;
						res=true;
					}
				} //Count>0
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+command);//run command

		} //TRY
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	
	public bool Get_Drive_Info(string computer, string driveletter,ref DriveInfoStruct driveinformation)
	{
		bool res=false;
		string command="";
		List<string> outputlist=new List<string>();
		List<string> detaillist=new List<string>();
		string commandstate="";
		string whereparams="";
		double amount=0;
		int headerrow=-1;
		try {
			driveinformation=default(DriveInfoStruct);
			StaticFunctions.Writelog("Checking if drive exists: ["+driveletter+"]");
			commandstate="Get-PSDrive ";
			if (driveletter!="") whereparams=whereparams+ " "+driveletter.Trim().Substring(0,1);
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				command=commandstate+whereparams;
			} else {
				command="Invoke-Command -ComputerName "+computer+" { "+commandstate+whereparams +" } ";
			}
			command=command+" | Format-Table "+SQL_Single_String("|")+",Name,"+SQL_Single_String("|")+",Provider,"+SQL_Single_String("|")+",Root"+","+SQL_Single_String("|")+",Used"+","+SQL_Single_String("|")+",Free"+","+SQL_Single_String("|")                                        +" –AutoSize ";
			StaticFunctions.Writelog("Command= "+command);
			if (Run_Command(command,ref outputlist)) {
				if (Find_Header_Line("Name,Provider,Root,Used,Free",ref outputlist,ref headerrow)) {
					for (int i=headerrow+2;i<outputlist.Count;i++) {
						if (Get_Detail_Values(outputlist[headerrow],outputlist[i],ref detaillist)) {
							if (detaillist.Count==5) {
								if (detaillist[2].ToUpper()==driveletter.ToUpper()) {
									driveinformation.PROVIDER=detaillist[1].ToUpper();
									driveinformation.DRIVE_LETTER=detaillist[2].ToUpper();
									if (double.TryParse(detaillist[3],out amount)) {
										driveinformation.USED_SPACE=Get_File_Size_Formatted(amount,2);
										if (double.TryParse(detaillist[4],out amount)) {
											driveinformation.FREE_SPACE=Get_File_Size_Formatted(amount,2);
											break;
										} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Parsing Used Space: "+detaillist[3]);
									} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Parsing Free Space: "+detaillist[3]);
								}
							} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Parsing details values: "+outputlist[i]);
						}
					}
				}
				res=true;
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+command);//run command
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	
	private string Get_File_Size_Formatted(double byteCount,int presicion=0)
	{
		string size = "0 Bytes";
		if (byteCount >= 1073741824)
			size = String.Format("{0:##.##}", Math.Round(byteCount / 1073741824,presicion)) + "GB";
		else if (byteCount >= 1048576)
			size = String.Format("{0:##.##}", Math.Round(byteCount / 1048576,presicion)) + "MB";
		else if (byteCount >= 1024)
			size = String.Format("{0:##.##}", Math.Round(byteCount / 1024,presicion)) + "KB";
		else if (byteCount > 0 && byteCount < 1024)
			size = byteCount.ToString() + " Bytes";
		
		return size;
	}
	public bool Create_Folder(string sourcefolder="") {
		bool res=false;
		DirectoryInfo di=null;
		try {
			if (sourcefolder!="") {
				di = Directory.CreateDirectory(sourcefolder);
				if (di.Exists) res=true;
			}
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	
	
	public bool Get_Logged_Users_List(string computer, ref List<LoggedUsersStruct> userslist)
	{
		bool res=false;
		string command="";
		List<string> outputlist=new List<string>();
		List<string> detaillist=new List<string>();
		List<string> commandlist=new List<string>();
		LoggedUsersStruct userinfo=new LoggedUsersStruct();
		int headerrow=-1;
		bool failmessagefound=false;
		try {
			userinfo=default(LoggedUsersStruct);
			userslist.Clear();
			if (computer=="") computer="LOCALHOST";
			StaticFunctions.Writelog("Checking Logged Users on: ["+computer+"]");
			//*****************************************************
			commandlist.Add("$serverName = '"+computer+"'");
			commandlist.Add("$sessions = qwinsta /server $serverName| ?{ $_ -notmatch '^ SESSIONNAME' } | %{");
			commandlist.Add("$item = '' | Select Active,SessionName,Username,Id,State,Type,Device");
			commandlist.Add("$item.Active = $_.Substring(0,1) -match '>'");
			commandlist.Add("$item.SessionName = $_.Substring(1,18).Trim()");
			commandlist.Add("$item.Username = $_.Substring(19,20).Trim()");
			commandlist.Add("$item.Id = $_.Substring(39,9).Trim()");
			commandlist.Add("$item.State = $_.Substring(48,8).Trim()");
			commandlist.Add("$item.Type = $_.Substring(56,12).Trim()");
			commandlist.Add("$item.Device = $_.Substring(68).Trim()");
			commandlist.Add("$item");
			commandlist.Add("}");
			commandlist.Add("foreach ($session in $sessions) {");
			commandlist.Add("if ($session.Username -ne '' -or $session.Username.Length -gt 1){");
			commandlist.Add("Write-Output $session | Format-Table '|',Active,'|',SessionName,'|',Username,'|',Id,'|',State,'|',Type,'|',Device,'|' ");
			commandlist.Add("}");
			commandlist.Add("}");

			//*****************************************************
			if (Run_Script(commandlist,ref outputlist)) {
				if (Find_Error_Message(outputlist)==false) {
					if (Find_Header_Line("Active,SessionName,Username,Id,State,Type,Device",ref outputlist,ref headerrow)) {
						for (int i=headerrow+2;i<outputlist.Count;i++) {
									if (outputlist[i].Contains("Windows PowerShell transcript end")==false) {						
										if (outputlist[i].Contains("******")==false && outputlist[i].Contains("------")==false && outputlist[i].Contains("| Active |")==false) {
											if (Get_Detail_Values(outputlist[headerrow],outputlist[i],ref detaillist)) {
													if (detaillist.Count==7) {
														userinfo.ACTIVE=detaillist[0];
														userinfo.SESSION_NAME=detaillist[1];
														userinfo.USERNAME=detaillist[2];
														userinfo.SESSION_ID=detaillist[3];
														userinfo.STATE=detaillist[4];
														userinfo.TYPE=detaillist[5];
														userinfo.DEVICE=detaillist[6];
														userslist.Add(userinfo);
													} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Parsing details values: "+outputlist[i]);
											} //get details values
										} //filter headers
									} else break;
							
						} //for
					} //find header line
					res=true;
				} //error message was found
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command ");//run command
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	
	public bool Logout_User_From_Server(string computer, string username)
	{
		bool res=false;
		string command="";
		bool userfound=false;
		List<string> outputlist=new List<string>();
		List<LoggedUsersStruct> userslist=new List<LoggedUsersStruct>();
		List<LoggedUsersStruct> userslist2=new List<LoggedUsersStruct>();
		try {
			StaticFunctions.Writelog("Logging out User: ["+username+"] from Computer: ["+computer+"]");
			if (Get_Logged_Users_List(computer,ref userslist)) {
				StaticFunctions.Writelog("Logged users found="+userslist.Count.ToString());
				if (userslist.Count>0) {
					for (int i=0;i<userslist.Count;i++) {
						if (userslist[i].USERNAME.ToUpper()==username.ToUpper()) {
							command="logoff /server '"+computer+"' "+userslist[i].SESSION_ID;
							if (Run_Command(command,ref outputlist)) {
								Wait_Seconds(3);
								StaticFunctions.Writelog("Verify if User: ["+username+"] from Computer: ["+computer+"] was logged out");
								if (Get_Logged_Users_List(computer,ref userslist2)) {
									StaticFunctions.Writelog("Verify Logged users found="+userslist2.Count.ToString());
									if (userslist2.Count>0) {
										for (int x=0;x<userslist2.Count;x++) {
											if (userslist2[x].USERNAME.ToUpper()==username.ToUpper()) {
												userfound=true;
											}
										} //for
									}
									if (userfound==false) { res=true; break;}
								} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR:  Get_Logged_Users_List ");//run command
							} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command ");//run command
						} //compare username
					} //for
				} else res=true; //no users logged found
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR:  Get_Logged_Users_List ");//run command
			

			//*****************************************************
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}	

	public bool Logout_All_Users_From_Server(string computer)
	{
		bool res=false;
		string command="";
		List<string> outputlist=new List<string>();
		List<LoggedUsersStruct> userslist=new List<LoggedUsersStruct>();
		List<LoggedUsersStruct> userslist2=new List<LoggedUsersStruct>();
		try {
			StaticFunctions.Writelog("Logging out all users from Computer: ["+computer+"]");
			if (Get_Logged_Users_List(computer,ref userslist)) {
				StaticFunctions.Writelog("Logged users found="+userslist.Count.ToString());
				if (userslist.Count>0) {
					for (int i=0;i<userslist.Count;i++) {
							command="logoff /server '"+computer+"' "+userslist[i].SESSION_ID;
							if (Run_Command(command,ref outputlist)) {
								Wait_Seconds(3);
							} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command ");//run command
						
					} //for
					StaticFunctions.Writelog("Verify if all users were Logged out");
					//********************************************Verify**********************
					if (Get_Logged_Users_List(computer,ref userslist2)) {
									StaticFunctions.Writelog("Verify Logged users found="+userslist2.Count.ToString());
									if (userslist2.Count==0) {
										res=true;
									}
					} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Get_Logged_Users_List ");//run command
				} else res=true; //no users logged found
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Get_Logged_Users_List ");//run command
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}		
	
	public bool Get_Cluster_Active_Node(string computer, string cluster_name,ref string cluster_active_node,ref string cluster_state)
	{
		bool res=false;
		string command="";
		List<string> outputlist=new List<string>();
		List<string> detaillist=new List<string>();
		string commandstate="";
		string whereparams="";
		double amount=0;
		string temp_ownernode="";
		string temp_state="";
		string temp_name="";
		try {
			cluster_active_node="";
			cluster_state="";
			StaticFunctions.Writelog("Getting Cluster Active Node");
			commandstate="Get-ClusterGroup ";
			whereparams="-Name "+SQL_Single_String(cluster_name)+" | Format-List -Property * ";
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				command=commandstate+whereparams;
			} else {
				command="Invoke-Command -ComputerName "+computer+" { "+commandstate+whereparams +" } ";
			}
			StaticFunctions.Writelog("Command= "+command);
			if (Run_Command(command,ref outputlist)) {
					for (int i=0;i<outputlist.Count;i++) {
						if(Parse_String_Array(outputlist[i],":",ref detaillist)) {
							if (detaillist.Count>1) {
								if (detaillist[0].ToUpper()=="OWNERNODE") {
										temp_ownernode=detaillist[1];
								}
								if (detaillist[0].ToUpper()=="NAME") {
										temp_name=detaillist[1];
								}
								if (detaillist[0].ToUpper()=="STATE") {
										temp_state=detaillist[1];
								}
							}
						}
					} //for
				if (temp_ownernode!="" && temp_name.ToUpper()==cluster_name.ToUpper() && temp_state!="") {
					cluster_active_node=temp_ownernode;
					cluster_state=temp_state;
					res=true;
				} 
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+command);//run command
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	
	public bool Get_Cluster_Resources(string computer, ref List<ClusterResourceStruct> resourcelist)
	{
		bool res=false;
		string command="";
		List<string> outputlist=new List<string>();
		List<string> detaillist=new List<string>();
		ClusterResourceStruct resourceinfo=new ClusterResourceStruct();
		int headerrow=-1;
		string whereparams="";
		string commandstate="";
		try {
			resourceinfo=default(ClusterResourceStruct);
			resourcelist.Clear();
			if (computer=="") computer="LOCALHOST";
			StaticFunctions.Writelog("Checking Cluster Resources on: ["+computer+"]");
			//*****************************************************
			commandstate="Get-ClusterResource ";
			if (computer.ToUpper()=="LOCALHOST" || computer=="" || Get_Host_Name_From_Name(computer).ToUpper()==Get_Host_Name().ToUpper()) {
				command=commandstate+whereparams;
			} else {
				command="Invoke-Command -ComputerName "+computer+" { "+commandstate+whereparams +" } ";
			}
			command=command+" | Format-Table "+SQL_Single_String("|")+",Name,"+SQL_Single_String("|")+",State,"+SQL_Single_String("|")+",OwnerGroup"+","+SQL_Single_String("|")+",ResourceType"+SQL_Single_String("|")                            +" –AutoSize ";
			//*****************************************************
			if (Run_Command(command,ref outputlist)) {
				//if (Find_Error_Message(outputlist)==false) {
					if (Find_Header_Line("Name,State,OwnerGroup,ResourceType",ref outputlist,ref headerrow)) {
						for (int i=headerrow+2;i<outputlist.Count;i++) {
											if (Get_Detail_Values(outputlist[headerrow],outputlist[i],ref detaillist)) {
													if (detaillist.Count==4) {
														resourceinfo=default(ClusterResourceStruct);
														resourceinfo.NAME=detaillist[0];
														resourceinfo.STATE=detaillist[1];
														resourceinfo.OWNERGROUP=detaillist[2];
														resourceinfo.RESOURCETYPE=detaillist[3];
														//if (resourceinfo.NAME.ToUpper()=="SERVICE") resourceinfo.STATE="Failed";
														resourcelist.Add(resourceinfo);
													} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Parsing details values: "+outputlist[i]);
											} //get details values
										
									
							
						} //for
						res=true;
					} //find header line
				//} //error message was found
			} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command ");//run command
		}
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: "+arg.Message);
		}
		return res;
	}
	
	
	public bool Get_Account_Info(string account_name,ref AccountInfoStruct account_info) {
		bool res=false;
		List<string> outputlist=new List<string>();
		string pwcommand="";
		int valuespos=-1;
		string tag="";
		string value="";
		try {
				account_info=default(AccountInfoStruct);
				StaticFunctions.Writelog("Getting Account Info ["+account_name+"]");
				pwcommand="net user "+SQL_String(account_name)+" /domain";
				if (Run_Command(pwcommand,ref outputlist)) {
					for (int i=0;i<outputlist.Count;i++) {
						if (outputlist[i].ToUpper().StartsWith("USER NAME") || valuespos>-1) {
								if (valuespos==-1) valuespos=outputlist[i].ToUpper().IndexOf(account_name.ToUpper());
						    	if (valuespos>-1) {
						    		tag=outputlist[i].Substring(0,valuespos).Trim();
						    		value=outputlist[i].Substring(valuespos).Trim();
						    		switch (tag.ToUpper()) {
			                                        case "USER NAME":
						    							account_info.USER_NAME=value;
			                                        break;
			                                        case "FULL NAME":
			                                        	account_info.FULL_NAME=value;
			                                        break;
			                                        case "COMMENT":
			                                				account_info.COMMENT=value;
			                                        break;
			                                        case "USER'S COMMENT":
			                                				account_info.USER_COMMENT=value;
			                                        break;
			                                        
			                                        case "ACCOUNT ACTIVE":
			                                        	account_info.ACCOUNT_ACTIVE=value;
			                                        break;
			                                        case "ACCOUNT EXPIRES":
			                                        	account_info.ACCOUNT_EXPIRES=value;
			                                        break;
			                                        case "PASSWORD LAST SET":
			                                        	account_info.PASSWORD_LAST_SET=value;
			                                        break;
			                                        case "PASSWORD EXPIRES":
			                                        	account_info.PASSWORD_EXPIRES=value;
			                                        break;
			                                        case "PASSWORD CHANGEABLE":
			                                        	account_info.PASSWORD_CHANGEABLE=value;
			                                        break;
			                                        case "PASSWORD REQUIRED":
			                                        	account_info.PASSWORD_REQUIRED=value;
			                                        break;
			                                        case "USER MAY CHANGE PASSWORD":
			                                        	account_info.USER_MAY_CHANGE_PASSWORD=value;
			                                        break;
//------------------------------------------------------------------------------------------
			                                    default:	
                                        
			                                        break;
			                       } //end switch
						    	} //valuespos>-1
						    } //find username
							
					} //for
					StaticFunctions.Writelog("Getting Account Info ["+account_name+"] OK");
					res=true;
				} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+pwcommand); //run command
			
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
		}
		return res;
	}
	private bool Write_File(string filename,string stext,bool appenddata)
		{
			bool res = false;
			try
			{
				if (filename!="") {
					if (appenddata==false) File_Delete(filename);
					StreamWriter streamWriter = new StreamWriter(filename, appenddata);
					streamWriter.WriteLine(stext);
					streamWriter.Close();
					res = true;
				}
			}
			catch (Exception arg)
			{
				StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);	
			}
			return res;
		}	
	public bool Create_Shortcut(string targetfilenamelink,string targetfilename,string arguments) {
			bool res=false;
			List<string> sourcecode=new List<string>();
			string newsourcecode="";
			string outputstring="";
			string workingdirectory="";
			List<string> outputlist=new List<string>();
			try {
				//---------------------------SHORTCUT SOURCE CODE--------------------------
				sourcecode.Add("$TargetFile = "+SQL_String("{TARGETFILE}"));
				sourcecode.Add("$ShortcutFile = "+SQL_String("{SHORTCUTFILE}"));
				sourcecode.Add("$WScriptShell = New-Object -ComObject WScript.Shell");
				sourcecode.Add("$Shortcut = $WScriptShell.CreateShortcut($ShortcutFile)");
				sourcecode.Add("$Shortcut.TargetPath = $TargetFile");
				sourcecode.Add("$Shortcut.Arguments= "+SQL_String("{ARGUMENTS}"));
				sourcecode.Add("$Shortcut.WorkingDirectory= "+SQL_String("{WORKINGDIRECTORY}"));
				sourcecode.Add("$Shortcut.Save()");
				//-------------------------------------------------------------------------
					workingdirectory=Get_Path_From_File(targetfilename);
					for (int i=0;i<sourcecode.Count;i++) {
						sourcecode[i]=sourcecode[i].Replace("{TARGETFILE}",targetfilename);
						sourcecode[i]=sourcecode[i].Replace("{SHORTCUTFILE}",targetfilenamelink);
						sourcecode[i]=sourcecode[i].Replace("{ARGUMENTS}",arguments);
						sourcecode[i]=sourcecode[i].Replace("{WORKINGDIRECTORY}",workingdirectory);
						newsourcecode=newsourcecode+sourcecode[i]+"\r\n";
					}
					if (File_Delete(targetfilenamelink)) {
						if (Run_Command_No_Pipeline(newsourcecode)) {
							if (File.Exists(targetfilenamelink)) {
								res=true;    	
							} else StaticFunctions.Writelog("ERROR: Creating Shortcut file: "+targetfilenamelink);
						} else StaticFunctions.Writelog("ERROR: Executing Command No Pipeline to Create Shortcut file: "+targetfilenamelink);
					} else StaticFunctions.Writelog("ERROR: Deleting Shortcut File: "+targetfilenamelink);
			}
			catch (Exception arg)
			{
				StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
			}
			return res;
		}	
        private string Get_Path_From_File(string sourcefilename)
        {
            string res = "";
            int xpos=0;
            try
            {
            	xpos = sourcefilename.LastIndexOf(@"\");
            	if (xpos>0) {
            		res=sourcefilename.Substring(0,xpos);	
            		if (res!="") {
            			res=Add_Last_Slash(res);
            		}
            	}
            }
            catch (Exception arg)
            {
                StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: " + arg.Message);
            }
            return res;
        }	
        
	public bool Shutdown_Computer(string computer) {
		bool res=false;
		bool found=false;
		string outputstring="";
		List<string> outputlist=new List<string>();
		List<string> computerlist=new List<string>();
		string pwcommand="";
		string newcomputer="";
		try {
				StaticFunctions.Writelog("Shutdown Computer ["+computer+"]");
				if (string.IsNullOrEmpty(computer)==false) {
					if (StaticFunctions.Parse_String_Array(computer,ref computerlist)) {
						for (int i=0;i<computerlist.Count;i++) {
							if (newcomputer!="") newcomputer=newcomputer+",";
							newcomputer=newcomputer+SQL_String(computerlist[i]);
						}
					}
						pwcommand="Stop-Computer -ComputerName "+newcomputer+" -Force";
						StaticFunctions.Writelog("The Command="+pwcommand);
						if (Run_Command(pwcommand,ref outputlist)) {						
							for (int x=0;x<outputlist.Count;x++) {
								StaticFunctions.Writelog("Outputlist["+x.ToString()+"]="+outputlist[x]);
							}
								StaticFunctions.Writelog("Shutdown Computer ["+computer+"] OK");
								res=true;
						} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+pwcommand); //run command
				} else StaticFunctions.Writelog("ERROR:Computer name is empty");//computer!=""
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.Shutdown_Computer.ERROR: " + arg.Message);
		}
		return res;
	}
	public bool Restart_Computer(string computer) {
		bool res=false;
		bool found=false;
		string outputstring="";
		List<string> outputlist=new List<string>();
		List<string> computerlist=new List<string>();
		string pwcommand="";
		string newcomputer="";
		try {
				StaticFunctions.Writelog("Restart Computer ["+computer+"]");
				if (string.IsNullOrEmpty(computer)==false) {
					if (StaticFunctions.Parse_String_Array(computer,ref computerlist)) {
						for (int i=0;i<computerlist.Count;i++) {
							if (newcomputer!="") newcomputer=newcomputer+",";
							newcomputer=newcomputer+SQL_String(computerlist[i]);
						}
					}
						pwcommand="Restart-Computer -ComputerName "+newcomputer+" -Force";
						StaticFunctions.Writelog("The Command="+pwcommand);
						if (Run_Command(pwcommand,ref outputlist)) {						
							for (int x=0;x<outputlist.Count;x++) {
								StaticFunctions.Writelog("Outputlist["+x.ToString()+"]="+outputlist[x]);
							}
								StaticFunctions.Writelog("Restart Computer ["+computer+"] OK");
								res=true;
						} else StaticFunctions.Writelog("POWERSHELLCLASS.ERROR: Running Powershell command: "+pwcommand); //run command
				} else StaticFunctions.Writelog("ERROR:Computer name is empty");//computer!=""
		} //try
		catch (Exception arg)
		{
			StaticFunctions.Writelog("POWERSHELLCLASS.Restart_Computer.ERROR: " + arg.Message);
		}
		return res;
	}
	//-------------------------------------------------------------------------------------------------------------------
} //class
