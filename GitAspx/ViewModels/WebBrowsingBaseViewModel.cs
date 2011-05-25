using GitSharp;

namespace GitAspx.ViewModels
{
    public class WebBrowsingBaseViewModel
    {
        public Repository Repository { get; set; }
        public string Project { get; set; }
        public Branch Branch { get; set; }
        public string TreeName { get; set; }
        public Commit Commit { get; set; }
        public Tag Tag { get; set; }
        public Tree RootTree { get; set; }
        public string Title { get; set; }
        public string[] PathSegments { get; set; }
    }
}