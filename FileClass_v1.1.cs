//******************************GLOBAL CLASS**********************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;

    public class FileClass
    {
    	
    	private string LOG_EXTRA_NAME="";
    	public bool DEBUG=false;
        public struct FileInfoStruct
        {
            public string FullName;
            public string Name;
            public string Extension;
            public long Size;
            public string DirectoryName;
            public DateTime CreationTime;
            public DateTime LastWriteTime;
        }
        public List<FileInfoStruct> FileInfoArray;
        public struct DirectoryInfoStruct
        {
            public string FullName;
            public string Name;
            public string Parent;
            public string Root;
            public DateTime CreationTime;
            public DateTime LastWriteTime;
        }
        public List<DirectoryInfoStruct> DirectoryInfoArray;

        //**************************************************FUNCTIONS*******************************************************
        public FileClass(string log_extra_name="") //constructor
        {
        	LOG_EXTRA_NAME=log_extra_name.ToUpper().Trim().Replace(",","-").Replace(";","-");
        	StaticFunctions.LOG_EXTRA_NAME=LOG_EXTRA_NAME;
            FileInfoArray=new List<FileInfoStruct>();
            DirectoryInfoArray = new List<DirectoryInfoStruct>();
        }

        public void Clean_File_Array() //constructor
        {
            FileInfoArray.Clear();
            DirectoryInfoArray.Clear();
        }


private bool Writelog(string smessage)
        {
            bool res = false;
            string targetfilename="";
            string currentdatetime = DateTime.Now.ToString("MM-dd-yyyy hh:mm:ss tt");
            string currentdate=DateTime.Now.ToString("MM-dd-yyyy");
		            try
		            {
		            	if (string.IsNullOrEmpty(LOG_EXTRA_NAME)==false) targetfilename=currentdate+"-("+LOG_EXTRA_NAME+").txt";
		            	else targetfilename=currentdate+".txt";
		            	StreamWriter streamWriter = new StreamWriter(StaticFunctions.RootFolder()+@"logs\"+targetfilename, true);
		                streamWriter.WriteLine(currentdatetime+"   "+smessage);
		                streamWriter.Close();
		                
		                if (smessage.ToUpper().Contains("ERROR:")) {
		            		if (string.IsNullOrEmpty(LOG_EXTRA_NAME)==false) targetfilename=currentdate+"-("+LOG_EXTRA_NAME+")(ERROR).txt";
		            		else	targetfilename=currentdate+"(ERROR)"+".txt";
		            		StreamWriter streamWriterExtra = new StreamWriter(StaticFunctions.RootFolder()+@"logs\"+targetfilename, true);
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
        
        
		public bool Get_Most_Updated_File(string sourcepath, string filetype, bool recursive)
        { 	
			bool res=false;
	            Clean_File_Array();
	            List<string> filelist=new List<string>();

				if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
            		for (int y=0;y<filelist.Count;y++) {
	            		res = Scan_Directory_Descending_Final(sourcepath,filelist[y],recursive);
	            	}
	            }
            return res;
        }        
		public bool Get_Most_Updated_File(List<string> sourcepath, string filetype, bool recursive)
        { 	
			bool res=false;
            Clean_File_Array();
            List<string> filelist=new List<string>();
            for (int i=0;i<sourcepath.Count;i++) {
				if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
            		for (int y=0;y<filelist.Count;y++) {
            			res = Scan_Directory_Descending_Final(sourcepath[i],filelist[y],recursive);
                		if (res==false) break;
            		}
            	}
            }
            return res;
        }



        public bool Scan_Directory_All(List<string> sourcepath, string filetype, bool recursive)
        {   bool res=false;
        	List<string> filelist=new List<string>();
            Clean_File_Array();
            for (int i=0;i<sourcepath.Count;i++) {
				if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
            		for (int y=0;y<filelist.Count;y++) {
            			res = Scan_Directory_All_Final(sourcepath[i],filelist[y],recursive);
                		if (res==false) break;
            		}
            	}
            }
            return res;
        }

        public bool Scan_Directory_All(string sourcepath, string filetype, bool recursive)
        {
            bool res = false;
                Clean_File_Array();
                List<string> filelist=new List<string>();
				if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
            		for (int y=0;y<filelist.Count;y++) {
                		res = Scan_Directory_All_Final(sourcepath, filelist[y], recursive);
                	}
                }
            return res;
        }
        
        
        private bool Scan_Directory_Descending_Final(string sourcepath, string filetype, bool recursive)
        {
            bool res = false;
            FileInfoStruct temp;
            DirectoryInfoStruct directorytemp;
            try
            {
                var files = new DirectoryInfo(sourcepath).GetFiles(filetype)
                		.OrderByDescending(f =>f.LastWriteTime)
                        .Select(f => new { f.CreationTime, f.FullName, f.Name, f.Length, f.LastWriteTime, f.DirectoryName, f.Extension });
                foreach (var file in files)
                {
                    if (file.Name.ToUpper() != "THUMBS.DB" &&  file.Name.ToUpper() !="DESKTOP.INI")
                    {
                        temp = default(FileInfoStruct);
                        temp.FullName = file.FullName;
                        temp.Name = file.Name;
                        temp.DirectoryName = StaticFunctions.Add_Last_Slash(file.DirectoryName);
                        temp.Size = file.Length;
                        temp.CreationTime = file.CreationTime;
                        temp.LastWriteTime = file.LastWriteTime;
                        temp.Extension = file.Extension;
                        FileInfoArray.Add(temp);
                    }

                } //for

                //------------------------GET DIRECTORIES-------------------------------
                if (recursive)  {
                    DirectoryInfo di = new DirectoryInfo(sourcepath);
                    DirectoryInfo[] diArr = di.GetDirectories();
                    foreach (DirectoryInfo dri in diArr)
                    {
                        if (Scan_Directory_Descending_Final(dri.FullName, filetype, recursive))
                        {
                            directorytemp.Name = dri.Name;
                            directorytemp.CreationTime = dri.CreationTime;
                            directorytemp.LastWriteTime = dri.LastWriteTime;
                            directorytemp.FullName = dri.FullName;
                            directorytemp.Parent = StaticFunctions.Add_Last_Slash(dri.Parent.FullName);
                            directorytemp.Root = StaticFunctions.Add_Last_Slash(dri.Root.FullName);
                            DirectoryInfoArray.Add(directorytemp);
                        }
                    }
                } //recursive
                //------------------------------------------------------------------------
                res = true;

            } //try
            catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: " + "[" + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "]=" + arg.Message);
            }
            return res;
        }
        
        private bool Scan_Directory_All_Final(string sourcepath, string filetype, bool recursive)
        {
            bool res = false;
            FileInfoStruct temp;
            DirectoryInfoStruct directorytemp;
            try
            {
                var files = new DirectoryInfo(sourcepath).GetFiles(filetype)
                        .OrderBy(f => f.LastWriteTime)
                        .Select(f => new { f.CreationTime, f.FullName, f.Name, f.Length, f.LastWriteTime, f.DirectoryName, f.Extension });
                foreach (var file in files)
                {
                    if (file.Name.ToUpper() != "THUMBS.DB" &&  file.Name.ToUpper() !="DESKTOP.INI")
                    {
                        temp = default(FileInfoStruct);
                        temp.FullName = file.FullName;
                        temp.Name = file.Name;
                        temp.DirectoryName = StaticFunctions.Add_Last_Slash(file.DirectoryName);
                        temp.Size = file.Length;
                        temp.CreationTime = file.CreationTime;
                        temp.LastWriteTime = file.LastWriteTime;
                        temp.Extension = file.Extension;
						FileInfoArray.Add(temp);
                    }

                } //for

                //------------------------GET DIRECTORIES-------------------------------
                if (recursive)  {
                    DirectoryInfo di = new DirectoryInfo(sourcepath);
                    DirectoryInfo[] diArr = di.GetDirectories();
                    foreach (DirectoryInfo dri in diArr)
                    {
                        if (Scan_Directory_All_Final(dri.FullName, filetype, recursive))
                        {
                            directorytemp.Name = dri.Name;
                            directorytemp.CreationTime = dri.CreationTime;
                            directorytemp.LastWriteTime = dri.LastWriteTime;
                            directorytemp.FullName = dri.FullName;
                            directorytemp.Parent =StaticFunctions.Add_Last_Slash(dri.Parent.FullName);
                            directorytemp.Root = StaticFunctions.Add_Last_Slash(dri.Root.FullName);
                            DirectoryInfoArray.Add(directorytemp);
                        }
                    }
                } //recursive
                //------------------------------------------------------------------------
                res = true;

            } //try
            catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: " + "[" + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "]=" + arg.Message);
            }
            return res;
        }
	
        public bool Scan_Directory_Older_Than(List<string> sourcepath, string filetype, DateTime fromdate, bool recursive)
        {
            bool res = false;
            Clean_File_Array();
            List<string> filelist=new List<string>();
            for (int i = 0; i < sourcepath.Count; i++)
            {
				if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
            		for (int y=0;y<filelist.Count;y++) {
            			res = Scan_Directory_Older_Than_Final(sourcepath[i], filelist[y],fromdate, recursive);
                		if (res == false) break;
            		}
            	}
            }
            return res;
        }

        public bool Scan_Directory_Older_Than(string sourcepath, string filetype, DateTime fromdate, bool recursive)
        {
            bool res = false;
            Clean_File_Array();
            List<string> filelist=new List<string>();
			if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
            	for (int y=0;y<filelist.Count;y++) {
            		res = Scan_Directory_Older_Than_Final(sourcepath, filelist[y], fromdate, recursive);
            	}
            }
            return res;
        }

        private bool Scan_Directory_Older_Than_Final(string sourcepath, string filetype, DateTime fromdate, bool recursive)
        {
            bool res = false;
            FileInfoStruct temp;
            DirectoryInfoStruct directorytemp;
            try
            {
                var files = new DirectoryInfo(sourcepath).GetFiles(filetype)
                        .OrderBy(f => f.LastWriteTime)
                        .Where(f => f.LastWriteTime <= fromdate)
                        .Select(f => new { f.CreationTime, f.FullName, f.Name, f.Length, f.LastWriteTime, f.DirectoryName, f.Extension });
                foreach (var file in files)
                {
                    if (file.Name.ToUpper() != "THUMBS.DB" &&  file.Name.ToUpper() !="DESKTOP.INI")
                    {
                        temp = default(FileInfoStruct);
                        temp.FullName = file.FullName;
                        temp.Name = file.Name;
                        temp.DirectoryName = file.DirectoryName;
                        temp.Size = file.Length;
                        temp.CreationTime = file.CreationTime;
                        temp.LastWriteTime = file.LastWriteTime;
                        temp.Extension = file.Extension;
						FileInfoArray.Add(temp);

                    }
                } //for
                //------------------------GET DIRECTORIES-------------------------------
                if (recursive)
                {
                    DirectoryInfo di = new DirectoryInfo(sourcepath);
                    DirectoryInfo[] diArr = di.GetDirectories();
                    foreach (DirectoryInfo dri in diArr)
                    {
                        if (Scan_Directory_Older_Than_Final(dri.FullName, filetype,fromdate, recursive))
                        {
                            directorytemp.Name = dri.Name;
                            directorytemp.CreationTime = dri.CreationTime;
                            directorytemp.LastWriteTime = dri.LastWriteTime;
                            directorytemp.FullName = dri.FullName;
                            directorytemp.Parent = StaticFunctions.Add_Last_Slash(dri.Parent.FullName);
                            directorytemp.Root = StaticFunctions.Add_Last_Slash(dri.Root.FullName);
                            DirectoryInfoArray.Add(directorytemp);
                        }
                    }
                } //recursive
                //------------------------------------------------------------------------
                res = true;
            } //try
            catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: " + "[" + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "]=" + arg.Message);
            }
            return res;
        }

        public bool Scan_Directory_Newer_Than(List<string> sourcepath, string filetype, DateTime fromdate, bool recursive)
        {
            bool res = false;
            Clean_File_Array();
            List<string> filelist=new List<string>();
            for (int i = 0; i < sourcepath.Count; i++)
            {
				if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
            		for (int y=0;y<filelist.Count;y++) {            
            			res = Scan_Directory_Newer_Than_Final(sourcepath[i], filelist[y], fromdate, recursive);
                		if (res == false) break;
					}
				}
            }
            return res;
        }

        public bool Scan_Directory_Newer_Than(string sourcepath, string filetype, DateTime fromdate, bool recursive)
        {
            bool res = false;
            Clean_File_Array();
            List<string> filelist=new List<string>();

			if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
            	for (int y=0;y<filelist.Count;y++) {
            		res = Scan_Directory_Newer_Than_Final(sourcepath, filelist[y], fromdate, recursive);
            	}
            }
            return res;
        }        

        private bool Scan_Directory_Newer_Than_Final(string sourcepath, string filetype, DateTime fromdate, bool recursive)
        {
            bool res = false;
            FileInfoStruct temp;
            DirectoryInfoStruct directorytemp;
            try
            {
                var files = new DirectoryInfo(sourcepath).GetFiles(filetype)
                        .OrderBy(f => f.LastWriteTime)
                        .Where(f => f.LastWriteTime >= fromdate)
                        .Select(f => new { f.CreationTime, f.FullName, f.Name, f.Length, f.LastWriteTime, f.DirectoryName, f.Extension });
                foreach (var file in files)
                {
                    if (file.Name.ToUpper() != "THUMBS.DB" &&  file.Name.ToUpper() !="DESKTOP.INI")
                    {
                        temp = default(FileInfoStruct);
                        temp.FullName = file.FullName;
                        temp.Name = file.Name;
                        temp.DirectoryName = file.DirectoryName;
                        temp.Size = file.Length;
                        temp.CreationTime = file.CreationTime;
                        temp.LastWriteTime = file.LastWriteTime;
                        temp.Extension = file.Extension;
						FileInfoArray.Add(temp);


                    }
                } //for
                //------------------------GET DIRECTORIES-------------------------------
                if (recursive)
                {
                    DirectoryInfo di = new DirectoryInfo(sourcepath);
                    DirectoryInfo[] diArr = di.GetDirectories();
                    foreach (DirectoryInfo dri in diArr)
                    {
                        if (Scan_Directory_Newer_Than_Final(dri.FullName, filetype,fromdate ,recursive))
                        {
                            directorytemp.Name = dri.Name;
                            directorytemp.CreationTime = dri.CreationTime;
                            directorytemp.LastWriteTime = dri.LastWriteTime;
                            directorytemp.FullName = dri.FullName;
                            directorytemp.Parent = StaticFunctions.Add_Last_Slash(dri.Parent.FullName);
                            directorytemp.Root = StaticFunctions.Add_Last_Slash(dri.Root.FullName);
                            DirectoryInfoArray.Add(directorytemp);
                        }
                    }
                } //recursive
                //------------------------------------------------------------------------

                res = true;
            } //try
            catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: " + "[" + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "]=" + arg.Message);
            }
            return res;
        }

        public bool Scan_Directory_Between(List<string> sourcepath, string filetype, DateTime fromdate, DateTime todate,bool recursive)
        {
            bool res = false;
            Clean_File_Array();
            List<string> filelist=new List<string>();
            for (int i = 0; i < sourcepath.Count; i++)
            {
				if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
            		for (int y=0;y<filelist.Count;y++) {
            			res = Scan_Directory_Between_Final(sourcepath[i], filelist[y], fromdate, todate,recursive);
                		if (res == false) break;
            		}
            	}
            }
            return res;
        }

        public bool Scan_Directory_Between(string sourcepath, string filetype, DateTime fromdate, DateTime todate, bool recursive,bool use_file_date_only=false)
        {
            bool res = false;
            Clean_File_Array();
            List<string> filelist=new List<string>();

			if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
            	for (int y=0;y<filelist.Count;y++) {
            		if (use_file_date_only) res = Scan_Directory_Between_Final_By_Date(sourcepath, filelist[y], fromdate, todate, recursive);
            		else res = Scan_Directory_Between_Final(sourcepath, filelist[y], fromdate, todate, recursive);
            	}
            }
            return res;
        }

        private bool Scan_Directory_Between_Final(string sourcepath, string filetype, DateTime fromdate, DateTime todate,bool recursive)
        {
            bool res = false;
            FileInfoStruct temp;
            DirectoryInfoStruct directorytemp;
            try
            {
                var files = new DirectoryInfo(sourcepath).GetFiles(filetype)
                        .OrderBy(f => f.LastWriteTime)
                        .Where(f => f.LastWriteTime >= fromdate && f.LastWriteTime <= todate)
                        .Select(f => new { f.CreationTime, f.FullName, f.Name, f.Length, f.LastWriteTime, f.DirectoryName, f.Extension });
                foreach (var file in files)
                {
                    if (file.Name.ToUpper() != "THUMBS.DB" &&  file.Name.ToUpper() !="DESKTOP.INI")
                    {
                        temp = default(FileInfoStruct);
                        temp.FullName = file.FullName;
                        temp.Name = file.Name;
                        temp.DirectoryName = file.DirectoryName;
                        temp.Size = file.Length;
                        temp.CreationTime = file.CreationTime;
                        temp.LastWriteTime = file.LastWriteTime;
                        temp.Extension = file.Extension;
						FileInfoArray.Add(temp);
                    }
                } //for
                //------------------------GET DIRECTORIES-------------------------------
                if (recursive)
                {
                    DirectoryInfo di = new DirectoryInfo(sourcepath);
                    DirectoryInfo[] diArr = di.GetDirectories();
                    foreach (DirectoryInfo dri in diArr)
                    {
                        if (Scan_Directory_Between_Final(dri.FullName, filetype, fromdate, todate,recursive))
                        {
                            directorytemp.Name = dri.Name;
                            directorytemp.CreationTime = dri.CreationTime;
                            directorytemp.LastWriteTime = dri.LastWriteTime;
                            directorytemp.FullName = dri.FullName;
                            directorytemp.Parent = StaticFunctions.Add_Last_Slash(dri.Parent.FullName);
                            directorytemp.Root = StaticFunctions.Add_Last_Slash(dri.Root.FullName);
                            DirectoryInfoArray.Add(directorytemp);
                        }
                    }
                } //recursive
                //------------------------------------------------------------------------
                res = true;
            } //try
            catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: " + "[" + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "]=" + arg.Message);
            }
            return res;
        }
        
        private bool Scan_Directory_Between_Final_By_Date(string sourcepath, string filetype, DateTime fromdate, DateTime todate,bool recursive)
        {
            bool res = false;
            FileInfoStruct temp;
            DirectoryInfoStruct directorytemp;
            try
            {
                var files = new DirectoryInfo(sourcepath).GetFiles(filetype)
                        .OrderBy(f => f.LastWriteTime)
                        .Where(f => f.LastWriteTime.Date >= fromdate && f.LastWriteTime.Date <= todate)
                        .Select(f => new { f.CreationTime, f.FullName, f.Name, f.Length, f.LastWriteTime, f.DirectoryName, f.Extension });
                foreach (var file in files)
                {
                    if (file.Name.ToUpper() != "THUMBS.DB" &&  file.Name.ToUpper() !="DESKTOP.INI")
                    {
                        temp = default(FileInfoStruct);
                        temp.FullName = file.FullName;
                        temp.Name = file.Name;
                        temp.DirectoryName = file.DirectoryName;
                        temp.Size = file.Length;
                        temp.CreationTime = file.CreationTime;
                        temp.LastWriteTime = file.LastWriteTime;
                        temp.Extension = file.Extension;
						FileInfoArray.Add(temp);
                    }
                } //for
                //------------------------GET DIRECTORIES-------------------------------
                if (recursive)
                {
                    DirectoryInfo di = new DirectoryInfo(sourcepath);
                    DirectoryInfo[] diArr = di.GetDirectories();
                    foreach (DirectoryInfo dri in diArr)
                    {
                        if (Scan_Directory_Between_Final(dri.FullName, filetype, fromdate, todate,recursive))
                        {
                            directorytemp.Name = dri.Name;
                            directorytemp.CreationTime = dri.CreationTime;
                            directorytemp.LastWriteTime = dri.LastWriteTime;
                            directorytemp.FullName = dri.FullName;
                            directorytemp.Parent = StaticFunctions.Add_Last_Slash(dri.Parent.FullName);
                            directorytemp.Root = StaticFunctions.Add_Last_Slash(dri.Root.FullName);
                            DirectoryInfoArray.Add(directorytemp);
                        }
                    }
                } //recursive
                //------------------------------------------------------------------------
                res = true;
            } //try
            catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: " + "[" + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "]=" + arg.Message);
            }
            return res;
        }
	public int Get_Total_Files(string sourcepath, string filetype, bool recursive) {
        int res=0;
		List<string> filelist=new List<string>();
		if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
            for (int y=0;y<filelist.Count;y++) {
				res=res+Get_Total_Files_Final(sourcepath,filelist[y],recursive);
			}
		}
		return res;
	}
	public bool Get_Total_Files(string sourcepath, string filetype, bool recursive,ref int total_files_found) {
        bool res=false;
        int errors=0;
		List<string> filelist=new List<string>();
		try {
			total_files_found=0;
			if (StaticFunctions.Parse_String_Array(filetype,ref filelist)) {
	            for (int y=0;y<filelist.Count;y++) {
					if (Directory.Exists(sourcepath)) {
						total_files_found=total_files_found+Get_Total_Files_Final(sourcepath,filelist[y],recursive);
					} else { errors++;StaticFunctions.Writelog("ERROR: Path not found: "+sourcepath); break;}
				}
			} else {errors++; StaticFunctions.Writelog("ERROR: Parsing File Type"+filetype); }
			if (errors==0) res=true;
		} //try
		catch (Exception arg)
            {
                 Writelog("FILECLASS.ERROR: " + "[" + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "]=" + arg.Message);
            }
		return res;
	}
	private int Get_Total_Files_Final(string sourcepath, string filetype, bool recursive)
        {
            int res = 0;
            int totalfound=0;
            FileInfoStruct temp;
            DirectoryInfoStruct directorytemp;
            try
            {
                var files = new DirectoryInfo(sourcepath).GetFiles(filetype)
                        .OrderBy(f => f.LastWriteTime)
                        .Select(f => new { f.CreationTime, f.FullName, f.Name, f.Length, f.LastWriteTime, f.DirectoryName, f.Extension });
                foreach (var file in files)
                {
                    if (file.Name.ToUpper() != "THUMBS.DB" &&  file.Name.ToUpper() !="DESKTOP.INI")
                    {
   						totalfound++;
                    }

                } //for

                //------------------------GET DIRECTORIES-------------------------------
                if (recursive)  {
                    DirectoryInfo di = new DirectoryInfo(sourcepath);
                    DirectoryInfo[] diArr = di.GetDirectories();
                    foreach (DirectoryInfo dri in diArr)
                    {
                        if (Scan_Directory_All_Final(dri.FullName, filetype, recursive))
                        {
                        	directorytemp.Name = dri.Name;
                            directorytemp.CreationTime = dri.CreationTime;
                            directorytemp.LastWriteTime = dri.LastWriteTime;
                            directorytemp.FullName = dri.FullName;
                            directorytemp.Parent = StaticFunctions.Add_Last_Slash(dri.Parent.FullName);
                            directorytemp.Root = StaticFunctions.Add_Last_Slash(dri.Root.FullName);
                            DirectoryInfoArray.Add(directorytemp);
                        }
                    }
                } //recursive
                //------------------------------------------------------------------------
                res = totalfound;

            } //try
            catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: " + "[" + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "]=" + arg.Message);
            }
            return res;
        }      


		public int Find_File_Name_Index(string name)
        {
            int res=-1;
            try
            {
                if (string.IsNullOrEmpty(name)==false) {
            		name = name.Trim().ToUpper();
	                for (int i = 0; i < FileInfoArray.Count; i++)
	                {
	                    if (FileInfoArray[i].Name.ToUpper() == name)
	                    {
	                        res = i;
	                        break;
	                    }
	                }
                }
            }
            catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: " +"["+this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name+"]=" +arg.Message);
            }
            return res;
        }

		public bool Move_All_Files_From_Array(string target_path) {
			bool res=false;
			int found=0;
			int moved=0;
			try {
				for (int i=0;i<FileInfoArray.Count;i++) {
					found++;
					if (StaticFunctions.File_Move_To_Path(FileInfoArray[i].FullName,target_path,true)) moved++;
				}
					if (found==moved) res=true;
			} //try
			catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: "+arg.Message);
            }
            return res;
		}
		
			public bool Move_All_Files_From_Array(string target_path,ref int files_moved) {
			bool res=false;
			int found=0;
			int moved=0;
			try {
				files_moved=0;
				for (int i=0;i<FileInfoArray.Count;i++) {
					found++;
					if (StaticFunctions.File_Move_To_Path(FileInfoArray[i].FullName,target_path,true)) moved++;
				}
				files_moved=moved;
				res=true;
			} //try
			catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: "+arg.Message);
            }
            return res;
		}
		public bool Move_All_Files_From_Array_With_Retry(string target_path,ref int files_moved,int retry=2,int wait_time=1) {
			bool res=false;
			int found=0;
			int moved=0;
			try {
				files_moved=0;
				for (int i=0;i<FileInfoArray.Count;i++) {
					found++;
					if (StaticFunctions.File_Move_To_Path_With_Retry(FileInfoArray[i].FullName,target_path,true,retry,wait_time)) moved++;
				}
				files_moved=moved;
				res=true;
			} //try
			catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: "+arg.Message);
            }
            return res;
		}

		public bool Copy_All_Files_From_Array(string target_path) {
			bool res=false;
			int found=0;
			int copied=0;
			try {
				for (int i=0;i<FileInfoArray.Count;i++) {
					found++;
					if (StaticFunctions.File_Copy_To_Path(FileInfoArray[i].FullName,target_path,true,false)) copied++;
				}
				if (found==copied) res=true;
			} //try
			catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: "+arg.Message);
            }
            return res;
		}

		public bool Copy_All_Files_From_Array(string target_path,ref int files_copied) {
			bool res=false;
			int found=0;
			int copied=0;
			try {
				files_copied=0;
				for (int i=0;i<FileInfoArray.Count;i++) {
					found++;
					if (StaticFunctions.File_Copy_To_Path(FileInfoArray[i].FullName,target_path,true,false)) copied++;
				}
				files_copied=copied;
				res=true;
			} //try
			catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: "+arg.Message);
            }
            return res;
		}		
        public bool Copy_All_Files_From_Array_With_Retry(string target_path,ref int files_copied,int retry=2,int wait_time=1) {
			bool res=false;
			int found=0;
			int copied=0;
			try {
				files_copied=0;
				for (int i=0;i<FileInfoArray.Count;i++) {
					found++;
					if (StaticFunctions.File_Copy_To_Path_With_Retry(FileInfoArray[i].FullName,target_path,true,retry,wait_time)) copied++;
				}
				files_copied=copied;
				res=true;
			} //try
			catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: "+arg.Message);
            }
            return res;
		}
		public bool Sort_File_List_Ascending_Order() {
			bool res=false;
			try {
				var sortedList = FileInfoArray.OrderBy(x => x.LastWriteTime).ToList();
				FileInfoArray.Clear();
				FileInfoArray=sortedList;
				res=true;
			} //try
			catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: "+ arg.Message);
            }
			return res;
		}	
   
		public bool Sort_File_List_Descending_Order() {
			bool res=false;
			try {
				var sortedList = FileInfoArray.OrderByDescending(x => x.LastWriteTime).ToList();
				FileInfoArray.Clear();
				FileInfoArray=sortedList;
				res=true;
			} //try
			catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: "+ arg.Message);
            }
			return res;
		}

     public bool Mix_Files() {
     	bool res=false;
     	string customer="";
     	List<string> customers_list=new List<string>();
     	List<FileInfoStruct> templist=new List<FileInfoStruct>();
     	FileInfoStruct temp=new FileInfoStruct();
     	int pos=-1;
     	try {
     		if (FileInfoArray.Count>0) {
     			//*******************Get Customer Groups************************************
     			for (int i=0;i<FileInfoArray.Count;i++) {
     				pos=FileInfoArray[i].Name.IndexOf("_");
     				if (pos>-1) {
     					customer=FileInfoArray[i].Name.Substring(0,pos);
     					if (StaticFunctions.Find_String_Index_In_List(customer,ref customers_list)==-1) {
     						customers_list.Add(customer);
     					}
     				}
     			} //for
     			//**************************************************************************
     			//*******************Copy File list to templist*****************************
     			if (customers_list.Count>0) {
     				do {
	     				for (int x=0;x<customers_list.Count;x++) {
		     				for (int i=0;i<FileInfoArray.Count;i++) {
		     					pos=FileInfoArray[i].Name.IndexOf("_");
		     					if (pos>-1) {
	     							customer=FileInfoArray[i].Name.Substring(0,pos);
	     							if (customer.ToUpper()==customers_list[x].ToUpper()) {
	     								templist.Add(FileInfoArray[i]);
	     								FileInfoArray.RemoveAt(i);
	     								break;
	     							}
		     					} else {
		     						templist.Add(FileInfoArray[i]);
	     							FileInfoArray.RemoveAt(i);
		     					}
		     				} //for i
	     				} //for x
     				} while (FileInfoArray.Count>0);
     			} //customer_list>0	
   				//**********************COPY ANY REMAINING FILES TO THE NEW LIST*********
   				for (int i=0;i<FileInfoArray.Count;i++) {
   					templist.Add(FileInfoArray[i]);
   				}
   				//***********************COPY NEW LIST TO ORIGINAL LIST******************
   				FileInfoArray.Clear();
   				for (int i=0;i<templist.Count;i++) {
   					FileInfoArray.Add(templist[i]);
   				}
   				//***********************************************************************
     		} 
     		res=true;
     	}
     	catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: "+ arg.Message);
            }
        return res;
     }
      
     public bool Remove_File_Name_from_Array(string fullname)
        {
            bool res=false;
            try
            {
                if (string.IsNullOrEmpty(fullname)==false) {
	                for (int i = 0; i < FileInfoArray.Count; i++)
	                {
	                	if (FileInfoArray[i].FullName.ToUpper() == fullname.ToUpper())
	                    {
	                    	FileInfoArray.RemoveAt(i);
	                    	res=true;
	                        break;
	                    }
	                }
                }
            }
            catch (Exception arg)
            {
                Writelog("FILECLASS.ERROR: " +"["+this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name+"]=" +arg.Message);
            }
            return res;
        }      
    public bool Set_Creation_Modified_DateTime(string filename,DateTime newcreationdatetime,DateTime newmodifieddatetime)
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
            Writelog("FILECLASS.ERROR: "+arg.Message);
        }
        return res;
    }
    
    public bool Get_FileStamp_Date_List(ref List<string> file_date_list,string date_format="MM-dd-yyyy" ) {
		bool res=false;
		string tempdate="";
		int index=-1;
		try{
			file_date_list.Clear();
			for (int i=0;i<FileInfoArray.Count;i++) {
				tempdate=FileInfoArray[i].LastWriteTime.ToString(date_format);
				index=StaticFunctions.Find_String_Index_In_List(tempdate,ref file_date_list);
				if (index==-1) file_date_list.Add(tempdate);
			} //i
			res=true;
		}
		catch (Exception arg)
		    {
		    	Writelog("FILECLASS.ERROR: "+arg.Message);        
		    }
        return res;
	}
    private bool String_Has_Separation_Chars(string source) {
    	bool res=false;
    	if (source.Contains(",") || source.Contains(";")) res=true;
    	return res;
    }
    
    
 
//-------------------------------------------------------------------------------------------------------------------
    } //class

