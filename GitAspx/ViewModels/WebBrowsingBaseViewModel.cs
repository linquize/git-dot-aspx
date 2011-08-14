using System;
using System.Collections.Generic;
using GitAspx.Lib;
using GitSharp;

namespace GitAspx.ViewModels
{
    public class WebBrowsingBaseViewModel
    {
        public WebBrowsingBaseViewModel()
        {
            CtorTime = DateTime.Now;
        }

        public DateTime CtorTime { get; private set; }
        public WebBrowsingSettings WebBrowsingSettings { get; set; }

        public Repository Repository { get; set; }
        public string Project { get; set; }
        public IEnumerable<Branch> Branches { get; set; }
        public string TreeName { get; set; }
        public Commit Commit { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public Tree RootTree { get; set; }
        public string Title { get; set; }
        public string[] PathSegments { get; set; }
    }
}