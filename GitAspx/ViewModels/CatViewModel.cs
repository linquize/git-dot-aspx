using System.Collections.Generic;

namespace GitAspx.ViewModels
{
    public class CatViewModel
    {
        public string RepositoryCategory { get; set; }
        public IEnumerable<CatInfo> Categories { get; set; }

        public class CatInfo
        {
            public string CatName { get; set; }
            public string LatestRepositoryName { get; set; }
            public string LatestCommitInfo { get; set; }
        }
    }
}