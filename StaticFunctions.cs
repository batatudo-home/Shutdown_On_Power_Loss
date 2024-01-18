//******************************GLOBAL CLASS**********************************************
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
using System.Globalization;
using System.Security.Cryptography;

    public static class StaticFunctions
    {
    	static public string LOG_EXTRA_NAME="";
        static public string RUNNINGTIME=DateTime.Now.ToString("MMddyyyy-hhmmss");
        static public DateTime CURRENTDATETIME=DateTime.Now;
        public const int PATH_LOCAL=1;
        public const int PATH_MFT=2;
        public static string DOUBLE_QUOTES= (Convert.ToChar(34)).ToString();
		public static string Get_SubFolder_From_Path(string sourcepath) {
				string res=sourcepath;
				int xpos=-1;
				int xpos2=-1;
				if (string.IsNullOrEmpty(sourcepath)==false) {
					if (sourcepath.StartsWith(@"\\")) { //is a network path
						xpos=sourcepath.IndexOf(@"\",2); //find the next slash
						if (xpos>2) {
									res=sourcepath.Substring(xpos);
						}
				
					} else { //is a local drive
							if (sourcepath.Substring(1,1)==":") {
								res=sourcepath.Substring(3);//get the first letter
							}
					}
				}
				return res;
	}
	public static string Get_Path_From_Filename(string sourcefile) {
				string res="";
				string lasttag="";
				int xpos=-1;
				if (string.IsNullOrEmpty(sourcefile)==false) {
					if (sourcefile.StartsWith(@"\\") || ( sourcefile.Substring(1,1)==":" && sourcefile.Substring(2,1)==@"\" ) ) lasttag=@"\";
					if (sourcefile.StartsWith(@"//") ) lasttag=@"/"; //is an MFT folder
					if (lasttag!="") {
							xpos=sourcefile.LastIndexOf(lasttag); //find the last slash
							if (xpos>0) {
								res=sourcefile.Substring(0,xpos);
							} else res=sourcefile;
							res=Add_Last_Slash(res);
					}
				}
				return res;
      }	
	public static string Get_Server_From_Path_old(string sourcepath) {
				string res=sourcepath;
				int xpos=-1;
				if (string.IsNullOrEmpty(sourcepath)==false) {
					if (sourcepath.StartsWith(@"\\")) { //is a network path
						xpos=sourcepath.IndexOf(@"\",2); //find the next slash
						if (xpos<1) {
							res=sourcepath.Substring(2);
						} else res=sourcepath.Substring(2,xpos-2);
				
					} else { //is a local drive
							if (sourcepath.Substring(1,1)==":") {
								res=sourcepath.Substring(0,1); //get the first letter
							}
					}
				}
				return res;
      }
     public static string Get_Server_From_Path(string sourcepath,bool getfullservername=false) {
				string res=sourcepath;
				int xpos=-1;
				try {
					if (sourcepath.StartsWith(@"\\")) { //is a network path
						xpos=sourcepath.IndexOf(@"\",2); //find the next slash
						if (xpos<1) {
							res=sourcepath.Substring(2);
						} else res=sourcepath.Substring(2,xpos-2);
						xpos=res.IndexOf(@".",1); //find the next slash
						if (getfullservername==false && xpos>0) res=res.Substring(0,xpos);
				
					} else { //is a local drive
							if (sourcepath.Substring(1,1)==":") {
								res=sourcepath.Substring(0,1); //get the first letter
							}
					}
				} //try 
            catch (Exception arg)
            {
                Writelog("ERROR: " + arg.Message);
            }

				return res;
      }
	public static string Get_FileName(string sourcefilename)
        {
            string res = "";
            int xpos=0;
            
            try
            {
	            if (string.IsNullOrEmpty(sourcefilename)==false) {
	            	xpos = sourcefilename.LastIndexOf(@"\");
	            	if (xpos>0) {
	            		res=sourcefilename.Substring(xpos+1);
	            	}
	            	if (res=="") { //check if is a MFT file
	            		xpos = sourcefilename.LastIndexOf(@"/");
	            		if (xpos>0) {
	            			res=sourcefilename.Substring(xpos+1).Trim();
	            		}
            		}
	            }
            }
            catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
            }
            return res;
        }

	public static string Get_FileName_Extencion(string sourcefilename)
        {
            string res = "";
            int xpos=0;
            
            try
            {
            	if (string.IsNullOrEmpty(sourcefilename)==false) {
	            	xpos = sourcefilename.LastIndexOf(@".");
	            	if (xpos>0) {
	            		res=sourcefilename.Substring(xpos+1); //this function does not include the "."
	            	}
            	}
            }
            catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
            }
            return res;
        }
		
	 public static bool Get_File_Year_Month_Day(string sourcefile,ref string year,ref string month,ref string day,ref string hour,ref string minute,ref string second) {
        	bool res=false;
        	string fdate="";
        	try {
		        	if (File.Exists(sourcefile)) {
		        		var lastModified = System.IO.File.GetLastWriteTime(sourcefile);
		
						year=Add_Zeros_Left(lastModified.Year.ToString(),4);
						month=Add_Zeros_Left(lastModified.Month.ToString(),2);
						day=Add_Zeros_Left(lastModified.Day.ToString(),2);
						hour=Add_Zeros_Left(lastModified.Hour.ToString(),2);
						minute=Add_Zeros_Left(lastModified.Minute.ToString(),2);
						second=Add_Zeros_Left(lastModified.Second.ToString(),2);
						res=true;
		        	} else Writelog("ERROR: File not found: "+sourcefile);
        	} //try
	        catch (Exception arg)
	        {
	           Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
	        }
        	return res;
        }
public static bool Get_File_Year_Month_Day(DateTime lastModified,ref string year,ref string month,ref string day,ref string hour,ref string minute,ref string second) {
        	bool res=false;
        	string fdate="";
        	try {
						year=Add_Zeros_Left(lastModified.Year.ToString(),4);
						month=Add_Zeros_Left(lastModified.Month.ToString(),2);
						day=Add_Zeros_Left(lastModified.Day.ToString(),2);
						hour=Add_Zeros_Left(lastModified.Hour.ToString(),2);
						minute=Add_Zeros_Left(lastModified.Minute.ToString(),2);
						second=Add_Zeros_Left(lastModified.Second.ToString(),2);
						res=true;
        	} //try
	        catch (Exception arg)
	        {
	           Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
	        }
        	return res;
        }        
        public static bool Get_File_Last_Modified_Datetime(string sourcefile,ref DateTime last_modified_datetime) {
        	bool res=false;
        	try {
		        	if (File.Exists(sourcefile)) {
		        		var lastModified = System.IO.File.GetLastWriteTime(sourcefile);
		        		last_modified_datetime=lastModified;
						res=true;
        		} else Writelog("ERROR: File not found: "+sourcefile);
        	} //try
	        catch (Exception arg)
	        {
	           Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
	        }
        	return res;
        }
        
	public static bool Set_File_Creation_Modified_Datetime(string filename,DateTime newcreationdatetime,DateTime newmodifieddatetime)
    	{
		bool res=false;
        try
        {
            if (File.Exists(filename))
            {
            	   File.SetCreationTime(filename, newcreationdatetime);
                   File.SetLastWriteTime(filename, newmodifieddatetime);
                   res=true;
            }            
        }
        catch (Exception arg)
        {
            Writelog("ERROR: "+arg.Message);
        }
        return res;
    }    		
        
        public static string Add_Zeros_Left(string source,int maxlengh) {
        	string res="";
        	res="000000000000"+source;
        	res=res.Substring(res.Length-maxlengh);
        	return res;
        }	
	public static bool Writelog(string smessage)
        {
            bool res = false;
            string targetfilename="";
            string currentdatetime = DateTime.Now.ToString("MM-dd-yyyy hh:mm:ss tt");
            string currentdate=DateTime.Now.ToString("MM-dd-yyyy");
		            try
		            {
		            	if (string.IsNullOrEmpty(LOG_EXTRA_NAME)==false) targetfilename=currentdate+"-("+LOG_EXTRA_NAME+").txt";
		            	else targetfilename=currentdate+".txt";
		            	StreamWriter streamWriter = new StreamWriter(RootFolder()+@"logs\"+targetfilename, true);
		                streamWriter.WriteLine(currentdatetime+"   "+smessage);
		                streamWriter.Close();
		                
		                if (smessage.ToUpper().Contains("ERROR:")) {
		            		if (string.IsNullOrEmpty(LOG_EXTRA_NAME)==false) targetfilename=currentdate+"-("+LOG_EXTRA_NAME+")(ERROR).txt";
		            		else	targetfilename=currentdate+"(ERROR)"+".txt";
		            		StreamWriter streamWriterExtra = new StreamWriter(RootFolder()+@"logs\"+targetfilename, true);
		                	streamWriterExtra.WriteLine(currentdatetime+"   "+smessage);
		                	streamWriterExtra.Close();
		                }
		                res = true;
		            }
		            catch (Exception)
		            {
		                
		            }
            return res;
        }	
   public static bool Writelog(string threadname,string smessage)
        {
            bool res = false;
            string targetfilename="";
            string currentdatetime = DateTime.Now.ToString("MM-dd-yyyy hh:mm:ss tt");
            string currentdate=DateTime.Now.ToString("MM-dd-yyyy");
		            try
		            {
		            if (threadname=="") threadname="MAIN";
		            	if (string.IsNullOrEmpty(threadname)==false) targetfilename=currentdate+"-("+threadname+").txt";
		            	else targetfilename=currentdate+".txt";
		            	StreamWriter streamWriter = new StreamWriter(RootFolder()+@"logs\"+targetfilename, true);
		                streamWriter.WriteLine(currentdatetime+"   "+smessage);
		                streamWriter.Close();
		                
		                if (smessage.ToUpper().Contains("ERROR:")) {
		            		if (string.IsNullOrEmpty(threadname)==false) targetfilename=currentdate+"-("+threadname+")(ERROR).txt";
		            		else	targetfilename=currentdate+"(ERROR)"+".txt";
		            		StreamWriter streamWriterExtra = new StreamWriter(RootFolder()+@"logs\"+targetfilename, true);
		                	streamWriterExtra.WriteLine(currentdatetime+"   "+smessage);
		                	streamWriterExtra.Close();
		                }
		                res = true;
		            }
		            catch (Exception)
		            {
		                
		            }
            return res;
        }
	   public static int Convert_To_Integer(string number) {
	   	int res=0;
	   	int tempnumber=0;
	   	try {
	   		if (Int32.TryParse(number,out tempnumber)) {
	   			res=tempnumber;
	   		}
	   	}

	   	catch (Exception arg)
            {
                Writelog("ERROR: "+arg.Message);
            }
	   	return res;
	   }
        public static string Get_Program_Version()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return version;
        }
        public static bool Program_Is_64Bit() //1=32bit,2=64bit
        {
        	bool res=false;
	          if (IntPtr.Size == 4)
				{
	          		res=false;
				}
				else if (IntPtr.Size == 8)
				{
					res=true;
				}
				else
				{
				    // The future is now!
				}
	            return res;
        }
    
        
       public static string Get_Current_Path() {
        	string res="";
        	try {
				string path = Directory.GetCurrentDirectory();
				res=path;
				if (!res.EndsWith("\\")) res =res + "\\";
        	}
        	catch (Exception arg)   
        	{
        		Writelog("STATICFUNCTIONS.ERROR: "+arg.Message);
            }
        	
			return res;
       }
       public static string Get_Current_Application_Path() {
        	string res="";
        	try {
        		
        		var directory="";
        		string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
        		if (path!="") {
					//To get the location the assembly normally resides on disk or the install directory
					//string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
					directory = System.IO.Path.GetDirectoryName(path);
					res=directory;
					if (!res.EndsWith("\\")) res =res + "\\";
        		}
        	}
        	catch (Exception arg)   
        	{
        		Writelog("STATICFUNCTIONS.ERROR: "+arg.Message);
            }
        	
			return res;
        }
        
        public static bool Change_Current_Path(string directory="") {
        	bool res=false;
        	try
			  {
        		if (directory!="") {
				  Directory.SetCurrentDirectory(directory);
				  res=true;
        		}
			  }
			  catch (Exception arg)
			  {
				Writelog("STATICFUNCTIONS.ERROR: "+arg.Message);  
			  }
			  return res;
        }
            
        public static string RootFolder() {
        	string res=System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location);
        	if (res.Last().ToString()!=@"\") res=res+@"\";
        	return res;
        }
        
        public static bool String_Found_On_List(string sourcetext, string sourcelist) {
        	bool res=false;
        	string[] newlist;
        	int index=0;
        	if (string.IsNullOrEmpty(sourcelist)==false && string.IsNullOrEmpty(sourcetext)==false) {
	        	newlist = sourcelist.Split(new char[] { ',',';' });
	        	for(index=0;index<=newlist.Count()-1;index++) {
	        		if (sourcetext.ToUpper().Contains(newlist[index].ToUpper())) {
	        				res=true;
	        				break;
	        			}
	        	}
        	}
        	return res;
        }
        
        public static string Find_String_Between(string sourcetext,string starttag,string endtag,int pos=0) {
        	string res="";
        	int xpos1=0;
        	int xpos2=0;
        	if (string.IsNullOrEmpty(sourcetext)==false) {
        		if (pos==0) xpos1=sourcetext.IndexOf(starttag); else xpos1=sourcetext.IndexOf(starttag,pos);
        		if (xpos1>0) {
        			xpos2=sourcetext.IndexOf(endtag,xpos1);
        			if (xpos2>xpos1) {
        				res=sourcetext.Substring(xpos1+starttag.Length,xpos2-(xpos1+starttag.Length));
        			}
        		}
        	}
        	return res;
        }
        

        
	public static bool Read_File(string filename,ref string thetext)
        {
            bool res = false;
            try
            {
            	thetext="";
            	if (string.IsNullOrEmpty(filename)==false) {
            		if (File.Exists(filename)) {
		            	StreamReader reader = File.OpenText(filename);
		                thetext= reader.ReadToEnd();
		                reader.Close();
		                res = true;
            		}
            	}
            }
            catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: Reading File error: "+arg.Message);
            }
                return res;
        }
	static public bool Read_File(string filename,ref List<string> thelist)
		{
			bool res = false;
			string line="";
			try
			{
				thelist.Clear();
				if (string.IsNullOrEmpty(filename)==false) {
					if (File.Exists(filename)) {
						StreamReader reader = File.OpenText(filename);
						while ((line = reader.ReadLine()) != null) {
							if (line!="")	thelist.Add(line);
						}
						reader.Close();
						res = true;
					}
				}
			}
			catch (Exception arg)
			{
				Writelog("STATICFUNCTIONS.ERROR: Reading File error: "+arg.Message);
			}
			return res;
		}         
     static public bool Write_File(string filename,string stext,bool appenddata)
        {
            bool res = false;  
		            try
		            {
		            	if (string.IsNullOrEmpty(filename)==false && string.IsNullOrEmpty(stext)==false) {
		            		if (appenddata==false) File_Delete(filename);
					        StreamWriter streamWriter = new StreamWriter(filename, appenddata);
					        streamWriter.WriteLine(stext);
					        streamWriter.Close();
					        res = true;
		            	}
		            }
		            catch (Exception arg)
		            {
		                Writelog("STATICFUNCTIONS.ERROR: Writting File error: "+arg.Message);
		            }
            return res;
        }        
        static public bool Write_File(string filename,ref List<string> thelist,bool appenddata)
        {
            bool res = false;
		            try
		            {
		            	if (string.IsNullOrEmpty(filename)==false && thelist.Count>0) {
			            	StreamWriter streamWriter = new StreamWriter(filename, appenddata);
			            	for (int i=0;i<thelist.Count;i++) {       
			            		streamWriter.WriteLine(thelist[i]);
			            	}
			            	streamWriter.Close();		            		
			                res = true;
		            	}
		            }
		            catch (Exception arg)
		            {
		                Writelog("STATICFUNCTIONS.ERROR: Writting File error: "+arg.Message);
		            }
            return res;
        }        
        
        public static bool Write_File(string filename,string thetext)
        {
            bool res = false;
		            try
		            {
		            	File.WriteAllText (filename, thetext);
		                res = true;
		            }
		            catch (Exception arg)
		            {
		                StaticFunctions.Writelog("STATICFUNCTIONS.ERROR: Writting File error: "+arg.Message);
		            }
            return res;
        }        
	public static bool Create_Folder(string sourcefolder="") {
        		bool res=false;
        		DirectoryInfo di=null;
        	try {
	        		if (sourcefolder!="") {
		        		di = Directory.CreateDirectory(sourcefolder);
		        		if (di.Exists) res=true;
	        		}
	        	}
            catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
            }
			return res;
		}
        public static bool File_Delete(string filename,bool set_attributes_to_normal=false)
        {
            bool res = false;
            try
            {
                    if (File.Exists(filename))
                    {
                    	if (set_attributes_to_normal) File.SetAttributes(filename, FileAttributes.Normal);
                        File.Delete(filename);
                        res = true;
                    }
                    else res = true;
                

            }
            catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
            }
            return res;
        }
        
        public static string Get_Host_Name() {
        	return System.Net.Dns.GetHostName().ToUpper();
        }
       
        static public string Get_Date_Formatted(string sourcedate,string dateformat="MM/dd/yyyy hh:mm:ss tt") {
        	string res="";
        	if(string.IsNullOrEmpty(sourcedate)==false) {
	        	DateTime temp=DateTime.Now;
	        	if (DateTime.TryParse(sourcedate,out temp)) {
	        		res=temp.ToString(dateformat);
	        	} else res=sourcedate;
        	}
        	return res;
        }
        
        static public string Get_Date_Formatted(DateTime sourcedate,string dateformat="MM/dd/yyyy hh:mm:ss tt") {
        	string res="";
        	DateTime temp=sourcedate;
       		res=temp.ToString(dateformat);
        	return res;
        }
        static public string Get_Date_Formatted() {
        	string res="";
        	res=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
        	return res;
        }
  private static bool Get_Temporary_Folder(ref string  temporaryfolder,string basefolder="") {
	   	bool res=false;
	   	string temppath="";
	   	int x=0;
	   	int ra=0;
	   	try {
	   		temporaryfolder="";
	   		if (string.IsNullOrEmpty(basefolder)) temppath=RootFolder()+@"temp\";
	   		else temppath=RootFolder()+@"temp\"+Add_Last_Slash(basefolder);
			   	if (temppath!="") {
			  		do {
			  			ra++;
				  		temppath=temppath+"tempfolder-"+ra.ToString()+"-"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+@"\";
				  		if (Directory.Exists(temppath)==false) {
				  			Directory.CreateDirectory(temppath);
				  			if (Directory.Exists(temppath)) {
				  				temporaryfolder=temppath;
				  				res=true;
				  			}
				  		} else {
				  			temporaryfolder=temppath;
				  			res=true;
				  		}
			  		} while (res==false && ra<1000);
			   	}
	   	}
	   		   	catch (Exception arg)
		{
			Writelog("StaticFunctions.Get_Temporary_Folder.ERROR: " + arg.Message);
		}
	   	return res;
	   }       
	   static public string Get_Temporary_Folder_old(string basefolder="") {
	   	string temppath="";
	   	string res="";
	   	int x=0;
	   	int ra=0;
	   	try {
	   		if (basefolder=="") temppath=RootFolder()+@"temp";//temppath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			  else temppath=RootFolder()+@"temp\"+basefolder;
			   	if (temppath!="") {
			  		temppath=Add_Last_Slash(temppath);
			  		do {
			  			ra++;
				  		temppath=temppath+"tempfolder-"+ra.ToString()+"-"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+@"\";
				  		if (Directory.Exists(temppath)==false) {
				  			Directory.CreateDirectory(temppath);
				  			if(Directory.Exists(temppath)) res=temppath;
				   		}
			  		} while (res=="" && ra<1000);
			   	}
	   	}
	   		   	catch (Exception arg)
		{
			Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
		}
	   	return res;
	   }
	public static bool Get_Temporary_File(ref string temporaryfile,string basefolder="") {
			string tempfile="";
			string temppath="";
			bool res=false;
			int ra=0;
			try {
				if (string.IsNullOrEmpty(basefolder)) {
					basefolder=RootFolder()+@"temp\";
					temppath=basefolder;
				}
				else temppath=RootFolder()+@"temp\"+basefolder;
				if (temppath!="") {
					temppath=Add_Last_Slash(temppath);
					do {
						ra++;
						tempfile=temppath+@"tempfile-"+ra.ToString()+"-"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+".tmp";
						if (File.Exists(tempfile)==false) {
							temporaryfile=tempfile;
							res=true;
							break;
						}
					} while (res==false && ra<1000);
				}
			}
			catch (Exception arg)
			{
				Writelog("SFTPCLASS.Get_Temporary_File.ERROR: " + arg.Message);
			}
			return res;
		}	   
	public static string Get_Temporary_File_old(string basefolder="") {
			string temppath="";
			string tempfile="";
			string res="";
			int ra=0;
			try {
				if (temppath!="") {
					do {
						ra++;
						tempfile=temppath+@"tempfile-"+ra.ToString()+"-"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+".tmp";
						if (File.Exists(temppath)==false) {
							res=tempfile;
							break;
						}
					} while (File.Exists(tempfile));
				}
			}
			catch (Exception arg)
			{
				Writelog("SFTPCLASS.ERROR: " + arg.Message);
			}
			return res;
		}        
        
        static public bool Clean_Folder(string sourcepath,bool Delete_Parent_Folder=false,bool delete_subfolders=false) {
	   	bool res=false;
	   	try {
	   		if (string.IsNullOrEmpty(sourcepath)==false) {
	   			sourcepath=Add_Last_Slash(sourcepath);
			   	if (Directory.Exists(sourcepath)) {
			   		EmptyFolder(new DirectoryInfo(sourcepath),delete_subfolders);
			   		if (Delete_Parent_Folder) {
			   			Directory.Delete(sourcepath,true);
			   			if (!Directory.Exists(sourcepath)) res=true; else Writelog("ERROR: Cleaning Folder: " + sourcepath);
			   		} else res=true;
	   			} else Writelog("STATICFUNCTOINS.ERROR: Folder not found: "+sourcepath);
	   		}
	   	}
	  catch (Exception arg)
		{
			Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
		}
	   	return res;
	   }
	   
        static  private void EmptyFolder(DirectoryInfo directoryInfo,bool delete_subfolders)
		{
			try {
			    foreach (FileInfo file in directoryInfo.GetFiles())
			    {       
			       file.Delete();
			     }
			
			    foreach (DirectoryInfo subfolder in directoryInfo.GetDirectories())
			    {
			      EmptyFolder(subfolder,delete_subfolders);
			      if (delete_subfolders) { 
			      		subfolder.Delete();
			    	}
			    }
			    
			}
			catch (Exception arg)
			{
				Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
			}
		}
        
        public static string Resolve_Variables(string sourcestring) {
        	string res=sourcestring;
	        	res=Append_Path(res,"{ROOTFOLDER}",RootFolder());
	        	res=res.Replace("{HOSTNAME}",Get_Host_Name());
	        	res=res.Replace("{SERVER_DATETIME}",DateTime.Now.ToString());
	        	res=res.Replace("{PROGRAM_START_DATETIME}",CURRENTDATETIME.ToString());
	        	res=res.Replace("{DATETIME}",DateTime.Now.ToString());
	        	res=res.Replace("{TIMESTAMP}",Get_Puerto_Rico_DateTime().ToString());
	        	res=res.Replace("{DATE}",DateTime.Now.Date.ToString());
	        	res=res.Replace("{TIME}",DateTime.Now.TimeOfDay.ToString());
	        	res=res.Replace("{PR-DATETIME}",Get_Puerto_Rico_DateTime().ToString());
	        	res=res.Replace("{SUBJECT-DATETIME}",DateTime.Now.ToString("MM-dd-yyyy hh.mm tt"));
	        	res=res.Replace("{EXECUTABLE-NAME}",System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName);
	        	res=res.Replace("{PROGRAM-NAME}",System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.Replace("_"," "));
	        	
        	return res;
        }
	public static string Append_Path(string sourcepath,string tag,string newvalue) {
        	string res="";
        	int xpos=0;
        	try {
        		if (string.IsNullOrEmpty(sourcepath)==false) {
		        	if (sourcepath.Contains(tag)) {
        				newvalue=Add_Last_Slash(newvalue);
		        			xpos=sourcepath.IndexOf(tag);
		        			if (xpos>0) {
			        			if (sourcepath[xpos-1]=='\\') {
					        		if (newvalue.StartsWith(@"\") ) {
				        				newvalue=newvalue.Remove(0,1); //remove first slash
						        	}
			        			}
		        			}
		        			if (sourcepath.Length>xpos+tag.Length) { //if there another char after the tag
			        			if (sourcepath[xpos+tag.Length].ToString()==@"\") {
				        			if (newvalue.EndsWith(@"\") ) {
				        				newvalue=newvalue.Substring(0,newvalue.Length-1); //remove last slash
						        	}
			        			}
		        			}
		        			res=sourcepath.Replace(tag,newvalue);
		        	} else res=sourcepath;
        		}
        	} //try
        	
        	catch (Exception arg)
			{
				Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
			}
        	return res;
        }        
           public static double File_Days_Back(string sourcefilename)
        {
            double res = 0;
            DateTime LastWriteTime;
            DateTime CurrentDateTime;
            CurrentDateTime = DateTime.Now;
            try
            {
            	if (string.IsNullOrEmpty(sourcefilename)==false) {
            		if (File.Exists(sourcefilename)) {
                		LastWriteTime = System.IO.File.GetLastWriteTime(sourcefilename);
                		res = (CurrentDateTime - LastWriteTime).TotalDays;
            		}
            	}
            }
            catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
            }
            return res;
        }
        public static bool Delete_Old_Logs(string sourcepath,int daystokeep,string filepattern) {
        bool res=false;
        double days=0;
	   	try {
        	if(string.IsNullOrEmpty(sourcepath)==false) {
        		sourcepath=Add_Last_Slash(sourcepath);
			   	if (Directory.Exists(sourcepath)) {
					   	 List<string> list = Directory.GetFiles(sourcepath, filepattern).ToList<string>();
					   	 foreach (string element in list) {
					   	 	days=File_Days_Back(element);
					   	 	if (days>daystokeep) File_Delete(element);
					   	 }
					   	 res=true;
			   	} else Writelog("ERROR: Path not found: "+sourcepath);
        	}
	   	}
	   		   	catch (Exception arg)
		{
			Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
		}
        	
        	
        	return res;
        }
        public static bool Check_Abort_Application() {
        	bool res=false;
   			if (File.Exists(RootFolder()+"stop.txt")) {
					Writelog("Stop file found!!");
					//System.Windows.Forms.Application.Exit();
					File_Delete(RootFolder()+"running.txt");
					Writelog("Aborting Application");
					System.Environment.Exit(1);
					res=true;
			}
        	return res;
        }
        public static bool Check_Abort_Application_File_Exist() {
        	bool res=false;
   			if (File.Exists(RootFolder()+"stop.txt")) {
					res=true;
			}
        	return res;
        }
       	public static int Find_String_Index_In_List(string text,ref List<string> thelist)
		{
			int res = -1;
			try
			{
				if (string.IsNullOrEmpty(text)==false) {
					for (int i=0;i<thelist.Count;i++) {
						if (thelist[i].ToUpper()==text.ToUpper()) {
							res=i;
							break;
						}
					} //for
				}
			}
			catch (Exception arg)
			{
				Writelog("STATICFUNCTIONS..ERROR: "+arg.Message);
			}
			return res;
		}
		public static int String_List_Found_On_String(string text,ref List<string> thelist)
		{
			int res = -1;
			try
			{
				if (string.IsNullOrEmpty(text)==false) {
					for (int i=0;i<thelist.Count;i++) {
						if (text.ToUpper().Contains(thelist[i].ToUpper())) {
							res=i;
							break;
						}
					} //for
				}
			}
			catch (Exception arg)
			{
				Writelog("STATICFUNCTIONS.ERROR: "+arg.Message);
			}
			return res;
		}
		static public DateTime Get_Puerto_Rico_DateTime() {
           	DateTime res=DateTime.Now;
           	try {
	        	TimeZone localZone = TimeZone.CurrentTimeZone;
	        	DateTime currentDate = DateTime.Now;
	        	DateTime NewDate = DateTime.Now;
	        	int currentYear = currentDate.Year;
	        	TimeZoneInfo est; 
	         	est = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
	        	DateTime currentUTC = localZone.ToUniversalTime( currentDate );
	        	TimeSpan currentOffset =est.GetUtcOffset( currentDate );	
	        	 //DaylightTime daylight = localZone.GetDaylightChanges( currentYear );
	        	 res=currentUTC+currentOffset;
           	} //try
			catch (Exception arg)
			{
				Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
			}
	       	return res;
        }       	
        public static bool Find_String_Index_In_String(string sourcetext, string sourcelist) {
        	bool res=false;
        	string[] newlist;
        	int index=0;
        try {
        	if (string.IsNullOrEmpty(sourcelist)==false) {
	        	newlist = sourcelist.Split(new char[] { ',',';' });
	        	for(index=0;index<=newlist.Count()-1;index++) {
	        		if (sourcetext.ToUpper().Contains(newlist[index].ToUpper())) {
	        				res=true;
	        				break;
	        			}
	        	}
        	}
        } //try       	
        catch (Exception arg)
		{
			Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
		}
        	return res;
        }        	
		static public string Add_Last_Slash(string value) {
			string res=value;
			if (string.IsNullOrEmpty(value)==false) {
				if (value.Substring(1,1)==":" || value.StartsWith("\\")) { //is a local or network drive
						if (value.EndsWith(@"\")==false) value=value.Trim()+@"\";
						res=value;
				} else	{
							if (value.StartsWith(@"/")) { //is an MFT site
								if (value.EndsWith(@"/")==false) value=value.Trim()+@"/";
								res=value;	
							}
						}
				} //is not null
			return res;
		}	        	
       	
		static public int Path_is_Local_or_MFT(string sourcepath) {
			int res=0; //1=Local or network drive, 2=MFT folder
			if (string.IsNullOrEmpty(sourcepath)==false) {
				if (sourcepath.Substring(1,1)==":" || sourcepath.StartsWith("\\")) { //is a local or network drive
					res=PATH_LOCAL;
				}
				if (sourcepath.StartsWith(@"/") && sourcepath.EndsWith(@"/")) { //is an MFT site
					res=PATH_MFT;	
				}
			} //is not null
			return res;
		}            	
                	
	static public void Wait_Seconds(int seconds) {
			int passseconds=0;
			Stopwatch stopWatch = new Stopwatch();
	        stopWatch.Start();
			do {
					Thread.Sleep(200);
			    	TimeSpan ts = stopWatch.Elapsed;
			    	passseconds=Convert.ToInt32(ts.TotalSeconds);
			    Application.DoEvents();
			} while (passseconds<seconds);
			stopWatch.Stop();
	}       	
       	
        static public bool Parse_String_Array(string source,ref List<string> thearray) {
           	try {
	           	thearray.Clear();
	           	if (string.IsNullOrEmpty(source)==false) {
			     	string[] items = source.Split(new char[] { ',','|' });
		        	for(int x=0;x<items.Length;x++) {
			     		thearray.Add(items[x].Trim());
		        	}
	           	}
           	}
			catch (Exception arg)
			{
				Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
			}

        	return true;
        } 		
		static public bool Parse_String_Array(string source,string tag,ref List<string> thearray) {
			bool res=false;
			int pos=0;
			string newsource=source;
			string temp="";
			try {
				thearray.Clear();
				if (string.IsNullOrEmpty(source)==false) {
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
				}
			} //try
			catch (Exception arg)
			{
				Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
			}
			
			return res;
		}
		static public DateTime Get_File_Last_Write_Time(string sourcefilename)
        {
			DateTime res=DateTime.MinValue;
            try
            {
            	if (File.Exists(sourcefilename)) {
                	res = System.IO.File.GetLastWriteTime(sourcefilename);
            	}
            }
            catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
            }
            return res;
        }      	
		static public bool File_Rename(string sourcefilename, string targetfilename,bool set_attributes_to_normal=false)
		{
			bool res = false;
			try
			{
				if (File.Exists(sourcefilename)) {
					FileInfo fileinfo = new FileInfo(sourcefilename);
					if(set_attributes_to_normal) File.SetAttributes(sourcefilename, FileAttributes.Normal);
					System.IO.File.Move(sourcefilename, targetfilename);
					if (File.Exists(targetfilename)) {
						res=true;
					}
				}
			}
			catch (Exception arg)
			{
				Writelog("STATICFUNCTIONS.ERROR: Renaming file:" + sourcefilename+" to: "+targetfilename);
			}
			return res;
		}
		static public bool File_Rename_Safe_Method(string sourcefilename, string targetfilename,bool set_attributes_to_normal=false)
		{
			bool res = false;
			string tempfilename="";
			try
			{
				if (File.Exists(sourcefilename)) {
					if (StaticFunctions.Get_Temporary_File(ref tempfilename)) {
						FileInfo fileinfo = new FileInfo(sourcefilename);
						if(set_attributes_to_normal) File.SetAttributes(sourcefilename, FileAttributes.Normal);
						System.IO.File.Move(sourcefilename, tempfilename);
						if (File.Exists(tempfilename)) {
							System.IO.File.Move(tempfilename, targetfilename);
							if (File.Exists(targetfilename)) {
								res=true;
							} //target file exist
						} //temp file exist
					} //get temporary file
				} //source file exist
			}
			catch (Exception arg)
			{
				StaticFunctions.File_Delete(tempfilename);
				Writelog("STATICFUNCTIONS.ERROR: Renaming file:" + sourcefilename+" to: "+targetfilename);
			}

			return res;
		}
        static public string Get_Week_Day_Name(DateTime thedate) {
        	string res="";
        	try {
        		switch (thedate.DayOfWeek) {
        			case DayOfWeek.Monday:
        					res="Monday";
        				break;
						case DayOfWeek.Tuesday:
        					res="Tuesday";
        				break;        				
        				case DayOfWeek.Wednesday:
        					res="Wednesday";
        				break;        	
        			case DayOfWeek.Thursday:
        					res="Thursday";
        				break;        	
					case DayOfWeek.Friday:
        					res="Friday";
        				break;        
					case DayOfWeek.Saturday:
        					res="Saturday";
        				break;        	
					case DayOfWeek.Sunday:
        					res="Sunday";
        				break;        				        				
        			default:
        			break;
        		}
        	}
        	catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
            }
        	return res;
        }
        
        static public bool Time_Falls_Between(DateTime thedate,string starthour,string endhour) {
        	bool res=false;
        	try {
        			TimeSpan now = thedate.TimeOfDay;
        			TimeSpan start = TimeSpan.Parse(starthour); // 10 PM
					TimeSpan end = TimeSpan.Parse(endhour);
	        				if (start <= end)
								{
								    // start and stop times are in the same day
								    if (now >= start && now <= end)
								    {
								        // current time is between start and stop
								        res=true;
								    }
								}
								else
								{
								    // start and stop times are in different days
								    if (now >= start || now <= end)
								    {
								       // current time is between start and stop
								       res=true;
								    }
								}
        	}
        	catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
            }
        	return res;
        }		
	    static public bool File_Copy(string sourcefilename, string targetfilename,bool overwrite=true,bool set_attributes_to_normal=false)
        {
            bool res = false;    
            try
            {
            	if (string.IsNullOrEmpty(sourcefilename)==false && string.IsNullOrEmpty(targetfilename)==false) {
            		if (File.Exists(sourcefilename)) {
                		FileInfo fileinfo = new FileInfo(sourcefilename);
                		fileinfo.CopyTo(targetfilename,overwrite);
                		if (set_attributes_to_normal)	File.SetAttributes(targetfilename, FileAttributes.Normal);
                		res = true;
            		}
            	}
           	
            }
            catch (Exception arg)
            {
    	         Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
            }
            return res;
        }        
        public static bool File_Copy_With_Retry(string sourcefilename, string targetpath,bool overwrite=true,bool set_attributes_to_normal=false,int retry=1,int wait_time=1)
        {
            bool res = false;
            int x=0;
            try
            {
            	do {
            		x++;
	            	if (File_Copy(sourcefilename,targetpath,overwrite,set_attributes_to_normal) ) {
            			res=true;
	            	} else {
            			Writelog("Coping File Failed, Retry # "+x.ToString());
            			if (x>=retry) break;
	            		Wait_Seconds(wait_time);
	            	}
            	} while (res==false && x<retry);
            } //try
            catch (Exception arg)
            {
            	Writelog("STATICFUNCTIONS.ERROR: Coping file:" + sourcefilename+" to: "+targetpath);
                Writelog("STATICFUNCTIONS.ERROR: "+arg.Message);
            }
            return res;
        }        
        
		static public bool File_Copy_To_Path(string sourcefilename, string targetpath,bool overwrite=true,bool set_attributes_to_normal=false)
        {
            bool res = false;
            string targetfilename="";
            string filename="";
            bool continue_with_copy=false;
            try
            {
            	if (string.IsNullOrEmpty(targetpath)==false) {
		            	if (File.Exists(sourcefilename)) {
		            		filename=Path.GetFileName(sourcefilename);
		            		if (filename!="") {
		            			targetfilename=Add_Last_Slash(targetpath)+filename;
		            			if (overwrite==false) {
		            				if (File.Exists(targetfilename)==true) continue_with_copy=false; else continue_with_copy=true;
		            			} else continue_with_copy=true;
		            			if (continue_with_copy) {
		                			FileInfo fileinfo = new FileInfo(sourcefilename);
			                		fileinfo.CopyTo(targetfilename,overwrite);
			                		if (set_attributes_to_normal)	File.SetAttributes(targetfilename, FileAttributes.Normal);
			                		if (File.Exists(targetfilename)) {
			                			res = true;
			                		}
		            			} else res=true;
		            			
		            		} else Writelog("ERROR: Filename is empty in: "+sourcefilename);
	            		} else Writelog("ERROR: Source file not found: "+sourcefilename);
	            } else Writelog("ERROR: Target Path is empty");
            }
            catch (Exception arg)
            {
            		Writelog("STATICFUNCTIONS.ERROR: Unable to copy file: "+sourcefilename);
            }
            return res;
        }  
        public static bool File_Copy_To_Path_With_Retry(string sourcefilename, string targetpath,bool overwrite=true,int retry=1,int wait_time=1)
        {
            bool res = false;
            int x=0;
            try
            {
            	do {
            		x++;
	            	if (File_Copy_To_Path(sourcefilename,targetpath,overwrite,false) ) {
            			res=true;
	            	} else {
            			Writelog("Coping File Failed, Retry # "+x.ToString());
            			if (x>=retry) break;
	            		Wait_Seconds(wait_time);
	            	}
            	} while (res==false && x<retry);
            } //try
            catch (Exception arg)
            {
            	Writelog("STATICFUNCTIONS.ERROR: Coping file:" + sourcefilename+" to: "+targetpath);
                Writelog("STATICFUNCTIONS.ERROR: "+arg.Message);
            }
            return res;
        }        
	public static bool File_Match_Wildcard(string input,string pattern)
		{
		    if (String.Compare(pattern, input) == 0)
		    {
		        return true;
		    }
		    else if(String.IsNullOrEmpty(input))
		    {
		        if (String.IsNullOrEmpty(pattern.Trim(new Char[1] { '*' })))
		        {
		            return true;
		        }
		        else
		        {
		            return false;
		        }
		    }
		    else if(pattern.Length == 0)
		    {
		        return false;
		    }
		    else if (pattern[0] == '?')
		    {
		        return File_Match_Wildcard(input.Substring(1),pattern.Substring(1));
		    }
		    else if (pattern[pattern.Length - 1] == '?')
		    {
		        return File_Match_Wildcard(input.Substring(0, input.Length - 1),pattern.Substring(0, pattern.Length - 1));
		    }
		    else if (pattern[0] == '*')
		    {
		        if (File_Match_Wildcard(input,pattern.Substring(1)))
		        {
		            return true;
		        }
		        else
		        {
		            return File_Match_Wildcard(input.Substring(1),pattern);
		        }
		    }
		    else if (pattern[pattern.Length - 1] == '*')
		    {
		        if (File_Match_Wildcard(input,pattern.Substring(0, pattern.Length - 1)))
		        {
		            return true;
		        }
		        else
		        {
		            return File_Match_Wildcard(input.Substring(0, input.Length - 1),pattern);
		        }
		    }
		    else if (pattern[0] == input[0])
		    {
		        return File_Match_Wildcard(input.Substring(1),pattern.Substring(1));
		    }
		    return false;
		}	            
		public static bool File_Matches_Pattern(string filename, string sourcelist) {
        	bool res=false;
        	string[] newlist;
        	int index=0;
        	string fileextencion="";
        try {
        	if (sourcelist!="") {
	        	newlist = sourcelist.Split(new char[] { ',',';' });
	        		fileextencion=StaticFunctions.Get_FileName_Extencion(filename);
	        		//if (fileextencion.Contains(".")) fileextencion=fileextencion.Substring(fileextencion.LastIndexOf(".")+1);
	        	for(index=0;index<=newlist.Count()-1;index++) {
	        	//**************CHECK FOR WILDCARD***************
	        		if (newlist[index].Contains("*")) {
	        			if (StaticFunctions.File_Match_Wildcard(filename,newlist[index]) ) {
		        				res=true;
		        				break;
		        		}
	        		} else {
	        			if (fileextencion.ToUpper().Trim()==newlist[index].ToUpper().Trim()) {
		        				res=true;
		        				break;
		        		}
	        		}
	        	}
        	} else res=true;
        } //try       	
        catch (Exception arg)
		{
			Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
		}
        	return res;
        }
        
	public static bool File_Move(string sourcefilename, string targetfilename,bool overwrite=true)
        {
            bool res = false;
            try
            {
                        FileInfo fileinfo = new FileInfo(sourcefilename);
                       // File.SetAttributes(sourcefilename, FileAttributes.Normal);
                        
                        if (File.Exists(targetfilename)==false) {
                        	fileinfo.MoveTo(targetfilename);
                        	if (File.Exists(targetfilename))  res = true;
                        } else if (overwrite == true)
                        	{
                        	if (File_Delete(targetfilename)) {
	                                fileinfo.MoveTo(targetfilename);
	                                if (File.Exists(targetfilename))  res = true;
	                        	}
                        	}	else res=true;                  
            }
            catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: Moving file:" + sourcefilename+" to: "+targetfilename);
            }
            return res;
        }         
        public static bool File_Move_With_Retry(string sourcefilename, string targetpath,bool overwrite=true,int retry=1,int wait_time=1)
        {
            bool res = false;
            int x=0;
            try
            {
            	do {
            		x++;
	            	if (File_Move(sourcefilename,targetpath,overwrite) ) {
            			res=true;
	            	} else {
            			Writelog("Moving File Failed, Retry # "+x.ToString());
            			if (x>=retry) break;
	            		Wait_Seconds(wait_time);
	            	}
            	} while (res==false && x<retry);
            } //try
            catch (Exception arg)
            {
            	Writelog("STATICFUNCTIONS.ERROR: Moving file:" + sourcefilename+" to: "+targetpath);
                Writelog("STATICFUNCTIONS.ERROR: "+arg.Message);
            }
            return res;
        }        
        
        
	public static bool File_Move_To_Path(string sourcefilename, string targetpath,bool overwrite=true)
        {
            bool res = false;
            string targetfilename="";
            string filename="";
            bool continue_with_move=false;
            try
            {
	            	if (targetpath!="") {
	            		filename=Path.GetFileName(sourcefilename);
	            		targetfilename=StaticFunctions.Add_Last_Slash(targetpath)+filename;
	            		if (overwrite==false) {
							if (File.Exists(targetfilename)==true) continue_with_move=false; else continue_with_move=true;
						} else continue_with_move=true;
						
		            	if (continue_with_move) {
	            				File_Delete(targetfilename);
			                	FileInfo fileinfo = new FileInfo(sourcefilename);
		                        fileinfo.MoveTo(targetfilename);
		                        res = true;
	            		} else {
	            			res=File_Delete(sourcefilename);
	            		}
	            	} else Writelog("ERROR: Target Path is empty");
            	
            }
            catch (Exception arg)
            {
            	Writelog("STATICFUNCTIONS.ERROR: Moving file:" + sourcefilename+" to: "+targetfilename);
                Writelog("STATICFUNCTIONS.ERROR: "+arg.Message);
            }
            return res;
        }
        
        public static bool File_Move_To_Path_With_Retry(string sourcefilename, string targetpath,bool overwrite=true,int retry=1,int wait_time=1)
        {
            bool res = false;
            int x=0;
            try
            {
            	do {
            		x++;
	            	if (File_Move_To_Path(sourcefilename,targetpath,overwrite) ) {
            			res=true;
	            	} else {
            			Writelog("Moving File Failed, Retry # "+x.ToString());
            			if (x>=retry) break;
	            		Wait_Seconds(wait_time);
	            	}
            	} while (res==false && x<retry);
            } //try
            catch (Exception arg)
            {
            	Writelog("STATICFUNCTIONS.ERROR: Moving file:" + sourcefilename+" to: "+targetpath);
                Writelog("STATICFUNCTIONS.ERROR: "+arg.Message);
            }
            return res;
        }
       static public string SQL_String(string source) {
        	string res="";
        	char quote=(char)34;
        	res=quote.ToString()+source+quote.ToString();
        	return res;
        }        
        static public void Display_Time_Zones() {
        	var infos = TimeZoneInfo.GetSystemTimeZones();
			foreach (var info in infos)
			{
			    StaticFunctions.Writelog("TimeZone="+info.Id);
			}
        }        
        
	public static bool File_Delete_Mask(string sourcepath,string filepattern="*.*") {
        bool res=false;
        double days=0;
	   	try {
        	if (string.IsNullOrEmpty(sourcepath)==false) {
        		sourcepath=StaticFunctions.Add_Last_Slash(sourcepath);
			   	if (Directory.Exists(sourcepath)) {
					   	 List<string> list = Directory.GetFiles(sourcepath, filepattern).ToList<string>();
					   	 foreach (string element in list) {
					   	 	File_Delete(element);
					   	 }
					   	 res=true;
			   	} else Writelog("STATICFUNCTIONS.ERROR: Path not found: "+sourcepath);
        	}
	   	}
	   catch (Exception arg)
		{
			Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
		}
        	return res;
        }        
        
        
	public static string Get_Elapsed_Time(DateTime startdate,DateTime enddate) {
			string res="";
			TimeSpan ts;
			try {
				ts=enddate-startdate;
				res = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
				
			}
			catch (Exception arg)
			{
				Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
			}
			return res;
		}        
		public static void Display_Message(string message,string caption="",MessageBoxButtons default_buttons=MessageBoxButtons.OK,MessageBoxIcon default_icon=MessageBoxIcon.Information)
        {
			try {
            	MessageBox.Show(message, caption, default_buttons, default_icon);
			}
			catch (Exception arg)
			{
				Writelog("STATICFUNCTIONS.ERROR: " + arg.Message);
			}
        }
        
	public static bool Replace_Values_Between_Tags(string source, string starttag, string endtag, string newvalue,ref string outputresult)
        {
            bool res = false;
            int spos = -1;
            int epos = -1;
            try
            {
                spos = source.ToUpper().IndexOf(starttag.ToUpper());
                if (spos > -1)
                {
                    epos = source.ToUpper().IndexOf(endtag.ToUpper(), spos + starttag.Length);
                    if (epos > 0 && epos > spos)
                    {
                        outputresult = source.Substring(0,spos+starttag.Length);
                        outputresult = outputresult + " "+newvalue.Trim();
                        outputresult = outputresult + source.Substring(epos);
                        res = true;
                    }
                }
            }
            catch (Exception arg)
            {
                StaticFunctions.Writelog("ERROR: " + arg.Message);
            }
            return res;
        }

        public static bool Replace_Values(string tag,string value) {
            bool res = false;
            int pos1 = 0;
            int pos2 = 0;
            pos1 = value.IndexOf(tag);
            if (pos1 > -1)
            {
                pos2 = value.IndexOf("\r");
                if (pos2 > -1)
                {

                }
            }
            return res;
        }
  public static string Format_Date_To_Oracle(string sourcedatetime)
  {
      string res = "";
      try
      {
          DateTime dateout;
          if (DateTime.TryParse(sourcedatetime, out dateout))
          {
              res = dateout.ToString("dd-MMM-yy HH:mm:ss");
          }
      }
      catch (Exception arg)
      {
          StaticFunctions.Writelog("StaticFunctions.ERROR: " + arg.Message);
      }
      return res;
  } 
	public static bool Get_Value_Between_Tags(string source, string starttag, string endtag, ref string value)
        {
            bool res = false;
            int spos = -1;
            int epos = -1;
            try
            {
            	value="";
                spos = source.ToUpper().IndexOf(starttag.ToUpper());
                if (spos > -1)
                {
                	spos=spos+starttag.Length;
                    epos = source.ToUpper().IndexOf(endtag.ToUpper(), spos + starttag.Length);
                    if (epos > 0 && epos > spos)
                    {
                        value = source.Substring(spos,epos-spos);
                        res = true;
                    }
                }
            }
            catch (Exception arg)
            {
                StaticFunctions.Writelog("StaticFunctions.ERROR: " + arg.Message);
            }
            return res;
        }
  
  public static bool Get_Tag_Value(string source, string tag, ref string value)
        {
  			bool res=false;
  			string newvalue="";
  			value="";
  			if (Get_Value_Between_Tags(source,tag+"="+DOUBLE_QUOTES,DOUBLE_QUOTES,ref newvalue)) {
  				value=newvalue;
  				res=true;
  			}
  			return res;
  		}
	public static string Get_MD5_Hash(string inputtext)
        {
			string res="";
			try {
					MD5 md5Hash = MD5.Create();
					if (md5Hash!=null) {
								// Convert the input string to a byte array and compute the hash.
					            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(inputtext));
					            // Create a new Stringbuilder to collect the bytes
					            // and create a string.
					            StringBuilder sBuilder = new StringBuilder();
					            // Loop through each byte of the hashed data
					            // and format each one as a hexadecimal string.
					            for (int i = 0; i < data.Length; i++)
					            {
					                sBuilder.Append(data[i].ToString("x2"));
					            }
					            res=sBuilder.ToString().ToUpper();
					}
			} //try
	     	catch (Exception arg)
	        {
               Writelog("StaticFunctions.ERROR: "+arg.Message);
	        }

		return res;
        }  		
        
	public static string Convert_List_To_String(ref List<string> list) {
		string res="";
		try {
			for (int i=0;i<list.Count;i++) {
				if (res!="") res=res+",";
				if (string.IsNullOrEmpty(list[i])==false) res=res+list[i];
			}
		} //try
		catch (Exception arg)
	        {
               Writelog("StaticFunctions.ERROR: "+arg.Message);
	        }
		return res;
		
	}
	public static string Get_Last_Folder_From_Path(string sourcepath) {
				string res="";
				string lasttag="";
				int xpos=-1;
				List<string> list=new List<string>();
				try {
					if (string.IsNullOrEmpty(sourcepath)==false) {
						if (sourcepath.StartsWith(@"\\") || ( sourcepath.Substring(1,1)==":" && sourcepath.Substring(2,1)==@"\" ) ) lasttag=@"\";
						if (sourcepath.StartsWith(@"//") ) lasttag=@"/"; //is an MFT folder
					if (lasttag!="") {
							sourcepath=Add_Last_Slash(sourcepath);
							if (Parse_String_Array(sourcepath,lasttag,ref list) ) {
								res=list[list.Count-1]; //get the last string from array
							}
						}
					}
				} //try
				catch (Exception arg)
		        {
	               Writelog("StaticFunctions.ERROR: "+arg.Message);
		        }	
				return res;
      }	
public static bool Find_String_in_File(string filename,string thetext)
        {
            bool res = false;
            string line="";
            try
            {
            	if (File.Exists(filename)) {
					StreamReader reader = File.OpenText(filename);
					while ((line = reader.ReadLine()) != null) {
						if (line.Trim().ToUpper()==thetext.Trim().ToUpper()) {
							res=true;
							break;
						}
					} //while
					reader.Close();
            	}
            }
            catch (Exception arg)
            {
                StaticFunctions.Writelog("ERROR: Reading File error: "+arg.Message);
            }
                return res;
        }    
 
 public static bool Find_String_Anywhere_in_File(string filename,string thetext)
        {
            bool res = false;
            string line="";
            int pos=-1;
            try
            {
            	if (File.Exists(filename)) {
					StreamReader reader = File.OpenText(filename);
					while ((line = reader.ReadLine()) != null) {
						if (line.ToUpper().IndexOf(thetext.ToUpper())>-1) {
							res=true;
							break;
						}
					} //while
					reader.Close();
            	}
            }
            catch (Exception arg)
            {
                StaticFunctions.Writelog("ERROR: Reading File error: "+arg.Message);
            }
                return res;
        } 
        
   public static bool Convert_Threshold_To_Seconds(string threshold,ref int totalseconds,ref string thresholdformat)
		{
			bool res = false;
			string format="";
			string newformat="";
			string number="";
			int xlen=0;
			int tempint=0;
			try
			{
				totalseconds=-1;
				thresholdformat="";
				if (threshold!="") {
					xlen=threshold.Length;
					newformat=threshold.Substring(xlen-1,1);
					format=newformat.ToUpper();
					number=threshold.Substring(0,xlen-1);
					if (format!="S" && format!="M" && format!="H" && format!="D") format="S";
					if (format!="" && number!="") {

						if (Int32.TryParse(number,out tempint) ) {
							thresholdformat=newformat;
							switch (format) {
								case "S":
									totalseconds=tempint;
									res=true;
								break;
								case "M":
									totalseconds=tempint*60;
									res=true;
								break;
								case "H":
									totalseconds=tempint*60*60;
									res=true;
								break;
								case "D":
									totalseconds=tempint*24*60*60;
									res=true;
								break;
							} //switch		
						} //if int32
					} //format!=""
				}
			} //try
			catch (Exception arg)
			{
				Writelog("ERROR: "+arg.Message);
			}
			return res;
		}
   public static string Convert_Seconds_To_Threshold(string thresholdformat, int seconds)
		{
			string res = "";
			string format="";
			string number="";
			int xlen=0;
			int tempint=0;
			double value=0.0;
			double newseconds=0.0;
			string newvalue="";
			int pos=0;
			int len=0;
			try
			{
				if (thresholdformat.ToUpper()=="S" || thresholdformat.ToUpper()=="M" || thresholdformat.ToUpper()=="H" || thresholdformat.ToUpper()=="D") {
					newseconds=seconds;
					switch (thresholdformat.ToUpper()) {
						case "S":
							res=seconds.ToString()+thresholdformat.ToLower();
						break;
						case "M":
							value=newseconds/60.0;
							newvalue=value.ToString();
							pos=newvalue.IndexOf(".");
							if (pos>0) {
								//len=newvalue.Substring(pos).Length;
								//if (len>=1) len=0; else len=0;
								newvalue=newvalue.Substring(0,pos);
							}
							res=newvalue+thresholdformat.ToLower();
						break;
						case "H":
							value=(newseconds/60.0)/60.0;
							newvalue=value.ToString();
							pos=newvalue.IndexOf(".");
							if (pos>0) {
								len=newvalue.Substring(pos).Length;
								if (len>=3) len=3; else len=2;
								newvalue=newvalue.Substring(0,pos+len);
							}
							res=newvalue+thresholdformat.ToLower();
						break;
						case "D":
							value=((newseconds/60.0)/60.0)/24.0;
							newvalue=value.ToString();
							pos=newvalue.IndexOf(".");
							if (pos>0) {
								len=newvalue.Substring(pos).Length;
								if (len>=3) len=3; else len=2;
								newvalue=newvalue.Substring(0,pos+len);
							}
							res=newvalue+thresholdformat.ToLower();
						break;
						default:
							res="N/A";
						break;
					}
				}
			} //try
			catch (Exception arg)
			{
				Writelog("ERROR: "+arg.Message);
			}
			return res;
		}
		
static public string Convert_Seconds_To_String(int seconds)
		{
			string res = "";
			try
			{
				TimeSpan t = TimeSpan.FromSeconds( seconds );
				if (t.Days==0)	res = string.Format("{0:D2}:{1:D2}:{2:D2}",t.Hours,t.Minutes,t.Seconds);	
				else res = string.Format("{0:D1}d {1:D2}:{2:D2}:{3:D2}",t.Days,t.Hours,t.Minutes,t.Seconds);
				//else res = string.Format("{0:D2}:{1:D2}:{2:D2}",(t.Days*24)+t.Hours,t.Minutes,t.Seconds);
				
				
			} //try
			catch (Exception arg)
			{
				Writelog("ERROR: "+arg.Message);
			}
			return res;
		}
static public string Convert_Seconds_To_Time(int seconds)
		{
			string res = "";
			try
			{
				TimeSpan t = TimeSpan.FromSeconds( seconds );
				res = string.Format("{0:D1}:{1:D2}",t.Minutes,t.Seconds);
			} //try
			catch (Exception arg)
			{
				Writelog("ERROR: "+arg.Message);
			}
			return res;
		}		
static public int Convert_Time_To_Seconds(string time_formatted)
		{
			int res = 0;
			List<string> values=new List<string>();
			int temp=0;
			int counter=0;
			try
			{	
				if (Parse_String_Array( time_formatted,":",ref values)) {
					for (int i=values.Count-1;i>=0;i--) {
						if (Int32.TryParse(values[i],out temp) ) {
							counter++;
							if (counter==1) res=res+temp; //seconds
							if (counter==2) res=res+(temp*60); //minutes
							if (counter==3) res=res+(temp*60*60); //hours
						} else { StaticFunctions.Writelog("ERROR: Parsing Time:"+time_formatted); break; } //try parse
					} //for
				}
			} //try
			catch (Exception arg)
			{
				Writelog("ERROR: "+arg.Message);
			}
			return res;
		}		
static public string Get_File_Size_Formatted(double byteCount,int presicion=0)
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
	    
	 static public string Get_Week_Number_Of_Month(DateTime date)
	{
		string res="0";
		int ires=0;
		try {
		    DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
		    DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
		    if (firstMonthMonday > date)
		    {
		        firstMonthDay = firstMonthDay.AddMonths(-1);
		        firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
		    }
		    ires=(date - firstMonthMonday).Days / 7 + 1;
		}
		catch (Exception arg)
		{
		    StaticFunctions.Writelog("ERROR: "+arg.Message);
		}
	    return ires.ToString(); 
	}
	
	public static string Get_Week_Number()
        {
            string res = "0";
            try {
				var d = DateTime.Now;
				CultureInfo cul = CultureInfo.CurrentCulture;
				var firstDayWeek = cul.Calendar.GetWeekOfYear(d,CalendarWeekRule.FirstDay,DayOfWeek.Monday);
				int weekNum = cul.Calendar.GetWeekOfYear(d,CalendarWeekRule.FirstDay,DayOfWeek.Monday);
				int year = weekNum == 52 && d.Month == 1 ? d.Year - 1 : d.Year;
				res=weekNum.ToString();
            } //try
            catch (Exception arg)
		            {
		                StaticFunctions.Writelog("ERROR: "+arg.Message);
		            }
		return res;
	}
	
	public static bool Folder_Found_On_Path(string sourcepath, string searchfolder) {
        	bool res=false;
        	string[] folderlist;
        	string[] ignorefolderlist;
        	int x=0;
        	int index=0;
        	try {
        	if (searchfolder!="") {
	        	ignorefolderlist = searchfolder.Split(new char[] { ';' });
	        	folderlist = sourcepath.Split(new char[] { '\\' });
	        	for(index=0;index<ignorefolderlist.Count();index++) {
			        	for (x=0;x<folderlist.Count();x++) {
			        			if (folderlist[x]!="" && folderlist[x].ToUpper()==ignorefolderlist[index].ToUpper()) {
			        				res=true;
			        				break;
			        			}
			        			if (ignorefolderlist[index].StartsWith(@"\")) { //is a directory
				        			if (sourcepath.ToUpper().Contains(ignorefolderlist[index].ToUpper())) {
				        				res=true;
				        				break;
				        			}
			        			}
		        			
			        	} //for x
		        		if (res==true) break;
	        		
	        	} //for index
	        	
        	}
        } //try
       catch (Exception arg)
		{
			Writelog("ERROR: " + arg.Message);
       }
        	return res;
    }
    
    public static bool Get_Folder_Recursive(string sDir,ref List<string> list,string ignorefolder) {
      			bool res =false;
			    try
			    {
			        foreach (string d in Directory.GetDirectories(sDir))
			        {
			        	if (Folder_Found_On_Path(d,ignorefolder)==false) {
					        	list.Add(d);
					        	if (Get_Folder_Recursive(d,ref list,ignorefolder)==false) {
					        		if (list.Count()>0) list.RemoveAt(list.Count() - 1); //remove the last subfolder
					        		res=false;
					        		break;
					        	}
			        	} else {
			        		//Writelog("","Ignoring Folder: "+d);
			        	}
			        		
		            
			        }
			        res=true;
			    }
			    catch (Exception arg)
			    {
					Writelog("ERROR: " + arg.Message);
			    }
			return res;
      }
      
      public static bool File_Move_All_Files_From_Path_To_Path(string sourcepath, string targetpath,string filetype="*.*",bool recursive=false)
        {
            bool res = false;
            string targetfilename="";
            string filename="";
            bool continue_with_move=false;
            FileClass sourcefiles=new FileClass();
            try
            {
            	if (string.IsNullOrEmpty(targetpath)==false) {
            		if (sourcefiles.Scan_Directory_All(sourcepath,filetype,recursive) ) {
            			if (sourcefiles.Move_All_Files_From_Array(targetpath)) {
            				res=true;
            			} else Writelog("ERROR: Moving all files from array to Target Path: "+targetpath);
            		} else Writelog("ERROR: Scanning Source Path: "+sourcepath);
	            } else Writelog("ERROR: Target Path is empty");
            }
            catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: "+arg.Message);
            }
            return res;
        }
        
    public static int Get_Total_Files_On_Path(string sourcepath,string filetype="*.*",bool recursive=false)
        {
            int res = -1;
            FileClass sourcefiles=new FileClass();
            try
            {
            	if (string.IsNullOrEmpty(sourcepath)==false) {
            		res=sourcefiles.Get_Total_Files(sourcepath,filetype,recursive);
	            } else Writelog("ERROR: Source Path is empty");
            }
            catch (Exception arg)
            {
                Writelog("STATICFUNCTIONS.ERROR: "+arg.Message);
            }
            return res;
        }

//-------------------------------------------------------------------------------------------------------
    } //StaticFunctions

