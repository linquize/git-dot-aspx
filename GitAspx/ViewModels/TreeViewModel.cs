using System.Collections.Generic;
using GitSharp;

namespace GitAspx.ViewModels
{
    public class TreeViewModel : WebBrowsingBaseViewModel
    {
        public IEnumerable<Tree> Directories { get; set; }
        public IEnumerable<Leaf> Files { get; set; }
    }
}