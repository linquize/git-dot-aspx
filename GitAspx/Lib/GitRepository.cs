namespace GitAspx.Lib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using GitSharp.Core;
    using GitSharp.Core.Transport;

    public class GitRepository
    {
        private DirectoryInfo directory;
        string rootDirectory;

        public static GitRepository Open(DirectoryInfo directory, string rootDirectory)
        {
            if (GitSharp.Repository.IsValid(directory.FullName))
            {
                return new GitRepository(directory, rootDirectory);
            }

            return null;
        }

        public GitRepository(DirectoryInfo directory, string rootDirectory)
        {
            this.directory = directory;
            this.rootDirectory = rootDirectory;
        }

        public void AdvertiseUploadPack(Stream output)
        {
            using (var repository = GetRepository())
            {
                var pack = new UploadPack(repository);
                pack.sendAdvertisedRefs(new RefAdvertiser.PacketLineOutRefAdvertiser(new PacketLineOut(output)));
            }
        }

        public void AdvertiseReceivePack(Stream output)
        {
            using (var repository = GetRepository())
            {
                var pack = new ReceivePack(repository);
                pack.SendAdvertisedRefs(new RefAdvertiser.PacketLineOutRefAdvertiser(new PacketLineOut(output)));
            }
        }

        public void Receive(Stream inputStream, Stream outputStream)
        {
            using (var repository = GetRepository())
            {
                var pack = new ReceivePack(repository);
                pack.setBiDirectionalPipe(false);
                pack.receive(inputStream, outputStream, outputStream);
            }
        }

        public void Upload(Stream inputStream, Stream outputStream)
        {
            using (var repository = GetRepository())
            {
                using (var pack = new UploadPack(repository))
                {
                    pack.setBiDirectionalPipe(false);
                    pack.Upload(inputStream, outputStream, outputStream);
                }
            }
        }

        public CommitInfo GetLatestCommit()
        {
            using (var repository = new GitSharp.Repository(PhysicalPathDotGit))
            {
                var commit = repository.Head.CurrentCommit;

                if (commit == null)
                {
                    return null;
                }

                return new CommitInfo
                {
                    Message = commit.Message,
                    Date = commit.CommitDate.LocalDateTime
                };
            }
        }

        private GitSharp.Core.Repository GetRepository()
        {
            return GitSharp.Core.Repository.Open(directory);
        }

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

        public void UpdateServerInfo()
        {
            using (var rep = GetRepository())
            {
                if (rep.ObjectDatabase is ObjectDirectory)
                {
                    RefWriter rw = new SimpleRefWriter(rep, rep.getAllRefs().Values);
                    rw.writePackedRefs();
                    rw.writeInfoRefs();

                    var packs = GetPackRefs(rep);
                    WriteInfoPacks(packs, rep);
                }
            }
        }

        private void WriteInfoPacks(IEnumerable<string> packs, GitSharp.Core.Repository repository)
        {

            var w = new StringBuilder();

            foreach (string pack in packs)
            {
                w.Append("P ");
                w.Append(pack);
                w.Append('\n');
            }

            var infoPacksPath = Path.Combine(repository.ObjectsDirectory.FullName, "info/packs");
            var encoded = Encoding.ASCII.GetBytes(w.ToString());


            using (Stream fs = File.Create(infoPacksPath))
            {
                fs.Write(encoded, 0, encoded.Length);
            }
        }

        private IEnumerable<string> GetPackRefs(GitSharp.Core.Repository repository)
        {
            var packDir = repository.ObjectsDirectory.GetDirectories().SingleOrDefault(x => x.Name == "pack");

            if (packDir == null)
            {
                return Enumerable.Empty<string>();
            }

            return packDir.GetFiles("*.pack").Select(x => x.Name).ToList();
        }
    }

    public class CommitInfo
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}