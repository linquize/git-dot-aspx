using GitAspx.Lib;
using GitAspx.ViewModels;
using GitSharp;

namespace GitAspx.Controllers
{
    public class DownloadViewController : WebBrowsingBaseController<DownloadViewModel>
    {
        public DownloadViewController(RepositoryService repositories) : base(repositories) { }

        public override void Browse()
        {
            if (Model.PathSegments.Length > 0)
            {
                AbstractObject loObject = Model.RootTree[string.Join("/", Model.PathSegments)];
                if (loObject is Leaf)
                    Model.File = loObject as Leaf;
                else if (loObject is Tree)
                    Model.Directory = loObject as Tree;                    
            }
        }
    }
}
