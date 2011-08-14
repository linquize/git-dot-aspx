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
using GitAspx.Lib;
using GitAspx.ViewModels;
using GitSharp;

namespace GitAspx.Controllers
{
    public class WebBrowsingBaseController<TModel> : Controller where TModel: WebBrowsingBaseViewModel, new()
    {
        readonly RepositoryService repositories;
        readonly TModel model;

        public TModel Model { get { return model; } }

        public WebBrowsingBaseController(RepositoryService repositories)
        {
            this.repositories = repositories;
            model = new TModel();
        }

        public ActionResult Index(string cat, string subcat, string project, string tree, string path)
        {
            try
            {
                model.WebBrowsingSettings = this.GetWebBrowsingSettings();
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = model.WebBrowsingSettings.CultureObject;

                model.PathSegments = path.SplitSlashes_OrEmpty().ToArray();
                model.Project = repositories.CombineRepositoryName(cat, subcat, project);
                model.Repository = repositories.GetBackendRepository(cat, subcat, project);

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
                }
                else
                {
                    loCommit = loBranch.CurrentCommit;
                }

                model.Branches = model.Repository.Branches
                    .Where(a => a.Value.CurrentCommit.Hash == loCommit.Hash || a.Value.CurrentCommit.Ancestors.Any(b => b.Hash == loCommit.Hash))
                    .Select(a => a.Value);
                model.Tags = model.Repository.Tags.Where(a => a.Value.Target.Hash == loCommit.Hash).Select(a => a.Value);
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
