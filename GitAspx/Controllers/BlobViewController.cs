using GitAspx.Lib;
using GitAspx.ViewModels;
using GitSharp;

namespace GitAspx.Controllers
{
    public class BlobViewController : WebBrowsingBaseController<BlobViewModel>
    {
        public BlobViewController(RepositoryService repositories) : base(repositories) { }

        public override void Browse()
        {
            Leaf loLeaf = null;
            if (Model.PathSegments.Length > 0)
                loLeaf = Model.RootTree[string.Join("/", Model.PathSegments)] as Leaf;
            Model.File = loLeaf;
        }
    }
}
