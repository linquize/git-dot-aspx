namespace GitAspx.ViewModels
{
    using GitAspx.Lib;

    public class RepositoryViewModel
    {
        private GitRepository repository;
        private CommitInfo latestCommit;

        public RepositoryViewModel(GitRepository repository)
        {
            this.repository = repository;
            this.latestCommit = repository.GetLatestCommit();
        }

        public string Name
        {
            get { return repository.Name; }
        }

        public string FullName
        {
            get { return repository.FullName; }
        }

        private string CommitDate
        {
            get { return latestCommit != null ? latestCommit.Date.ToPrettyDateString() : string.Empty; }
        }

        private string Message
        {
            get { return latestCommit != null ? latestCommit.Message.Shorten(60) : string.Empty; }
        }

        public string LatestCommitInfo
        {
            get { return Message + " - " + CommitDate; }
        }
    }
}