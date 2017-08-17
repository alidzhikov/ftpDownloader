using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;

class Program
{
    static void Main(string[] args)
    {
        string host = "ftp://jedpanel.com/wp-content";
        string folderPath = @"d:\ftpDownloads\ftpGet-" + DateTime.Now.ToString("h-dd-m-f");
        DownloadFiles(host, folderPath);
    }

    public static void CreateDirectoryWithPermissions(string folderPath)
    {
        Directory.CreateDirectory(folderPath);
        DirectoryInfo dInfo = new DirectoryInfo(folderPath);
        DirectorySecurity dSequrity = dInfo.GetAccessControl();
        string Acc = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        DirectorySecurity sec = Directory.GetAccessControl(folderPath);
        sec.AddAccessRule(new FileSystemAccessRule(Acc, FileSystemRights.Modify | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
        Directory.SetAccessControl(folderPath, sec);
    }

    public static void DownloadFiles(string url, string folderPath, string folderInner = null)
    {
        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(url);
        ftpRequest.Credentials = new NetworkCredential("xinlong@jedpanel.com", "ZS)m38T!RVu");
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
        FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
        StreamReader streamReader = new StreamReader(response.GetResponseStream());
        List<string> directories = new List<string>();

        string line = streamReader.ReadLine();
        while (!string.IsNullOrEmpty(line))
        {
            directories.Add(line);
            line = streamReader.ReadLine();
        }
        streamReader.Close();

        CreateDirectoryWithPermissions(folderPath);

        using (WebClient ftpClient = new WebClient())
        {
            ftpClient.Credentials = new System.Net.NetworkCredential("xinlong@jedpanel.com", "ZS)m38T!RVu");

            for (int i = 0; i <= directories.Count - 1; i++)
            {
                if (directories[i].Contains("."))
                {
                    if (directories[i].Contains(".php") || directories[i].Contains(".js"))
                    {
                        string filePath = url + '/' + directories[i].ToString();
                        string trnsfPath = folderPath + '/' + directories[i].ToString();
                        ftpClient.DownloadFile(filePath, trnsfPath);
                        continue;
                    }
                    else if (directories[i] == "." || directories[i] == "..")
                    {
                        continue;
                    }
                }else
                {
                    string urlP = url + '/' + directories[i].ToString();
                    var asdf = System.IO.Path.GetExtension(urlP);
                    
                    if (System.IO.Path.GetExtension(urlP) == string.Empty)
                    {
                        if (directories.Count < 2)
                        {
                            continue;
                        }
                        string localPath = folderPath + '\\' + directories[i];
                        CreateDirectoryWithPermissions(localPath);
                        DownloadFiles(url + '/' + directories[i].ToString(), localPath);
                    }                 
                }
            }
        }
    }
}

