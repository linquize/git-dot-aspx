using GitSharp;

namespace GitAspx.ViewModels
{
    public class DownloadViewModel : WebBrowsingBaseViewModel
    {
        public Tree Directory { get; set; }
        public Leaf File { get; set; }
    }
}