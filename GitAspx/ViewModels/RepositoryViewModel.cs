namespace GitAspx.ViewModels {
	using System;
	using GitAspx.Lib;

	public class RepositoryViewModel {
		private Repository repository;
		private CommitInfo latestCommit;

		public RepositoryViewModel(Repository repository) {
			this.repository = repository;
			this.latestCommit = repository.GetLatestCommit();
		}

		public string Name {
            get { return repository.Name.Substring(0, repository.Name.Length - 4); }
		}

		private string CommitDate {
			get { return latestCommit != null ? latestCommit.Date.ToPrettyDateString() : string.Empty; }
		}

		private string Message {
			get { return latestCommit != null ? latestCommit.Message.Shorten(60) : string.Empty; }
		}

		public string LatestCommitInfo {
			get { return Message + " - " + CommitDate; }
		}
	}
}