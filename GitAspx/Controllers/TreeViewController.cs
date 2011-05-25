using System;
using GitAspx.Lib;
using GitAspx.ViewModels;
using GitSharp;

namespace GitAspx.Controllers
{
    public class TreeViewController : WebBrowsingBaseController<TreeViewModel>
    {
        public TreeViewController(RepositoryService repositories) : base(repositories) { }

        public override void Browse()
        {
            Tree loTree = Model.RootTree;
            Tree loTree2 = null;
            if (Model.PathSegments.Length > 0)
                loTree2 = Model.RootTree[string.Join("/", Model.PathSegments)] as Tree;
            if (loTree2 != null)
                loTree = loTree2;
            Model.Directories = loTree.Trees;
            Model.Files = loTree.Leaves;
        }
    }
}
