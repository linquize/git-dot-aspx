namespace GitAspx.Lib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using NGit.Api;
    using NGit.Revwalk;
    using NGit.Storage.File;
    using NGit.Transport;
    using Sharpen;
    using NGit;
    using NGit.Util;

    public class NGitBasedRepository : GitRepository
    {
        static NGitBasedRepository()
        {
            //Set user home directory if the home directory of the user running this application cannot be retrieved.
            string lsPath = AppSettings.FromAppConfig().UserHomeDirectory;
            if (!string.IsNullOrEmpty(lsPath))
                FS.DETECTED.SetUserHome(new FilePath(lsPath));
        }

        public NGitBasedRepository(DirectoryInfo directory, string rootDirectory)
            : base(directory, rootDirectory)
        {            
        }

        public override void AdvertiseUploadPack(Stream output)
        {
            using (var repository = GetRepository())
            {
                var pack = new UploadPack(repository);
                pack.SendAdvertisedRefs(new RefAdvertiser.PacketLineOutRefAdvertiser(new PacketLineOut(output)));
            }
        }

        public override void AdvertiseReceivePack(Stream output)
        {
            using (var repository = GetRepository())
            {
                var pack = new ReceivePack(repository);
                pack.SendAdvertisedRefs(new RefAdvertiser.PacketLineOutRefAdvertiser(new PacketLineOut(output)));
            }
        }

        public override void Receive(Stream inputStream, Stream outputStream)
        {
            using (var repository = GetRepository())
            {
                var pack = new ReceivePack(repository);
                pack.SetBiDirectionalPipe(false);
                pack.Receive(inputStream, outputStream, outputStream);
            }
        }

        public override void Upload(Stream inputStream, Stream outputStream)
        {
            using (var repository = GetRepository())
            {
                var pack = new UploadPack(repository);
                pack.SetBiDirectionalPipe(false);
                pack.Upload(inputStream, outputStream, outputStream);
            }
        }

        public override CommitInfo GetLatestCommit()
        {
            using (var repository = GetRepository())
            {
                var repo = (FileRepository)repository;
                var branch = repo.GetBranch();
                if (branch == null) return null;
                var objId = repo.Resolve(branch);
                if (objId == null) return null;
                RevWalk walk = new RevWalk(repo);
                RevCommit commit = walk.ParseCommit(objId);
                if (commit == null) return null;

                return new CommitInfo
                {
                    Message = commit.GetFullMessage(),
                    Date = commit.GetCommitterIdent().GetWhen().ToLocalTime()
                };
            }
        }

        private RepoWrapper GetRepository()
        {
            return new RepoWrapper(Git.Open(new FilePath(directory.FullName)).GetRepository() as FileRepository);
        }

        public override void UpdateServerInfo()
        {
            using (var rep = GetRepository())
            {
                var repo = (FileRepository)rep;
                if (repo.ObjectDatabase is ObjectDirectory)
                {
                    NGitRefWriter rw = new NGitBasedSimpleRefWriter(repo, repo.GetAllRefs().Values);
                    rw.WritePackedRefs();
                    rw.WriteInfoRefs();

                    var packs = GetPackRefs(rep);
                    WriteInfoPacks(packs, rep);
                }
            }
        }

        private void WriteInfoPacks(IEnumerable<string> packs, FileRepository repository)
        {

            var w = new StringBuilder();

            foreach (string pack in packs)
            {
                w.Append("P ");
                w.Append(pack);
                w.Append('\n');
            }

            var infoPacksPath = Path.Combine(repository.ObjectsDirectory.GetAbsolutePath(), "info/packs");
            var encoded = Encoding.ASCII.GetBytes(w.ToString());


            using (Stream fs = File.Create(infoPacksPath))
            {
                fs.Write(encoded, 0, encoded.Length);
            }
        }

        private IEnumerable<string> GetPackRefs(FileRepository repository)
        {
            var packDir = repository.ObjectsDirectory.GetAbsolutePath();

            if (packDir == null)
            {
                return Enumerable.Empty<string>();
            }

            return new DirectoryInfo(packDir).GetFiles("*.pack").Select(x => x.Name).ToList();
        }

        struct RepoWrapper : IDisposable
        {
            FileRepository repository;

            public static implicit operator FileRepository(RepoWrapper wrapper)
            {
                return wrapper.repository;
            }

            public RepoWrapper(FileRepository repository)
            {
                this.repository = repository;
            }
            
            public void Dispose()
            {
                if (repository != null)
                    repository.Close();
            }
        }
    }
}