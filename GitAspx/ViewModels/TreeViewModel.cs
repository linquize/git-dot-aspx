using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitSharp;

namespace GitAspx.ViewModels
{
    public class TreeViewModel : WebBrowsingBaseViewModel
    {
        string msRepositorySize;

        public string RepositorySize
        {
            get
            {
                if (msRepositorySize == null)
                {
                    DirectoryInfo loDir = new DirectoryInfo(this.Repository.Directory);
                    FileInfo[] laFiles = loDir.GetFiles("*", SearchOption.AllDirectories);
                    long llTotal = laFiles.Sum(a => (a.Length + 1) / 4096) * 4096;
                    if (llTotal < 1024)
                        msRepositorySize = llTotal + "bytes";
                    else if (llTotal < 1024 * 1024)
                        msRepositorySize = (llTotal / 1024) + "KB";
                    else if (llTotal < (long)(1024 * 1024 * 1024))
                        msRepositorySize = (llTotal / (1024 * 1024)) + "MB";
                    else
                        msRepositorySize = (llTotal / (1024 * 1024 * 1024)) + "GB";
                }

                return msRepositorySize;
            }
        }

        public IEnumerable<Tree> Directories { get; set; }
        public IEnumerable<Leaf> Files { get; set; }
    }
}