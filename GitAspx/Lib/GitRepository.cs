namespace GitAspx.Lib
{
    using System;
    using System.IO;

    public abstract class GitRepository
    {
        protected DirectoryInfo directory;
        protected string rootDirectory;

        public static GitRepository Open(DirectoryInfo directory, string rootDirectory)
        {
            return new NGitBasedRepository(directory, rootDirectory);
        }

        protected GitRepository(DirectoryInfo directory, string rootDirectory)
        {
            this.directory = directory;
            this.rootDirectory = rootDirectory;
        }

        public abstract void AdvertiseUploadPack(Stream output);

        public abstract void AdvertiseReceivePack(Stream output);

        public abstract void Receive(Stream inputStream, Stream outputStream);

        public abstract void Upload(Stream inputStream, Stream outputStream);

        public abstract CommitInfo GetLatestCommit();

        public string Name
        {
            get
            {
                string lsPath = NameDotGit;
                return lsPath.Substring(0, lsPath.Length - 4);
            }
        }

        public string NameDotGit
        {
            get { return directory.Name; }
        }

        public string PhysicalPathDotGit
        {
            get { return directory.FullName; }
        }


        public string FullName
        {
            get
            {
                string lsPath = FullNameDotGit;
                return lsPath.Substring(0, lsPath.Length - 4);
            }
        }

        public string FullNameDotGit
        {
            get
            {
                string lsPath = directory.FullName.Substring(rootDirectory.Length);
                lsPath = lsPath.Replace(Path.DirectorySeparatorChar, '/');
                return lsPath.StartsWith("/") ? lsPath.Substring(1) : lsPath;
            }
        }

        public string Category
        {
            get
            {
                string lsCat = Path.GetDirectoryName(directory.FullName).Substring(rootDirectory.Length).Replace('\\', '/');
                return lsCat.StartsWith("/") ? lsCat.Substring(1) : lsCat;
            }
        }

        public string GitDirectory()
        {
            if (PhysicalPathDotGit.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
            {
                return PhysicalPathDotGit;
            }

            return Path.Combine(PhysicalPathDotGit, ".git");
        }

        public abstract void UpdateServerInfo();
    }

    public class CommitInfo
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}