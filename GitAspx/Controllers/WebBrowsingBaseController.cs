#region License

// Copyright 2011 Linquize
//  
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://github.com/Linquize/git-dot-aspx

#endregion

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using GitAspx.ViewModels;
using GitSharp;

namespace GitAspx.Controllers
{
    public class WebBrowsingBaseController<TModel> : Controller where TModel: WebBrowsingBaseViewModel, new()
    {
        readonly GitAspx.Lib.RepositoryService repositories;
        readonly TModel model;

        public TModel Model { get { return model; } }

        public WebBrowsingBaseController(GitAspx.Lib.RepositoryService repositories)
        {
            this.repositories = repositories;
            model = new TModel();
        }

        public ActionResult Index(string project, string tree,
            string folder1, string folder2, string folder3, string folder4, string folder5, string folder6, string folder7, string folder8,
            string folder9, string folder10, string folder11, string folder12, string folder13, string folder14, string folder15, string folder16)
        {
            try
            {
                model.PageSettings = Session["PageSettings"] as Lib.PageSettings ?? new Lib.PageSettings();

                string[] lsaFolderPath = { folder1, folder2, folder3, folder4, folder5, folder6, folder7, folder8, 
                                      folder9, folder10, folder11, folder12, folder13, folder14, folder15, folder16 };

                try
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo((string)Session["culture"]);
                    Properties.Resources.Culture = CultureInfo.GetCultureInfo((string)Session["culture"]);
                }
                catch { }

                model.PathSegments = lsaFolderPath.TakeWhile(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                model.Project = project;
                model.Repository = new Repository(repositories.GetRepository(project + ".git").GitDirectory());

                if (string.IsNullOrWhiteSpace(tree))
                    tree = model.Repository.CurrentBranch.Name;

                Branch loBranch;
                Commit loCommit;
                if (!model.Repository.Branches.TryGetValue(tree, out loBranch))
                {
                    Tag loTag;
                    if (!model.Repository.Tags.TryGetValue(tree, out loTag))
                    {
                        loCommit = model.Repository.Get<Commit>(tree);
                        if (loCommit == null)
                            throw new Exception(string.Format("tree {0} not found", tree));
                    }
                    else
                        loCommit = loTag.Target as Commit;
                    loBranch = model.Repository.Branches.FirstOrDefault(a => a.Value.CurrentCommit.Hash == loCommit.Hash || a.Value.CurrentCommit.Ancestors.Any(b => b.Hash == loCommit.Hash)).Value;
                }
                else
                {
                    loCommit = loBranch.CurrentCommit;
                }
                model.Branch = loBranch;
                model.Tag = model.Repository.Tags.FirstOrDefault(a => a.Value.Target.Hash == loCommit.Hash).Value;
                model.Commit = loCommit;
                model.RootTree = loCommit.Tree;
                model.TreeName = tree;
                model.Title = model.PathSegments.Length == 0 ? string.Format("{0} at {1}", project, tree)
                    : string.Format("{2} at {1} from {0}", project, tree, string.Join("/", model.PathSegments));

                Browse();
                return View(model);
            }
            finally { GC.Collect(); }
        }

        public virtual void Browse()
        {
        }
    }
}
