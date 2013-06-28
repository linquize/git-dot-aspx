#region License

// Copyright 2010 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at http://github.com/JeremySkinner/git-dot-aspx

#endregion

namespace GitAspx.Controllers
{
    using System.IO;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.SessionState;
    using GitAspx.Lib;

    // Handles /project/info/refs
    [SessionState(SessionStateBehavior.Disabled)]
    public class InfoRefsController : GitHttpBaseController
    {
        readonly RepositoryService repositories;

        public InfoRefsController(RepositoryService repositories)
        {
            this.repositories = repositories;
        }

        public ActionResult Execute(string cat, string subcat, string project, string service)
        {
            service = GetServiceType(service);
            bool isUsingSmartProtocol = service != null;

            // Service has been specified - we're working with the smart protocol
            if (isUsingSmartProtocol)
            {
                return SmartInfoRefs(service, cat, subcat, project);
            }

            // working with the dumb protocol.
            return DumbInfoRefs(cat, subcat, project);
        }

        ActionResult SmartInfoRefs(string service, string cat, string subcat, string project)
        {
            Response.ContentType = string.Format("application/x-git-{0}-advertisement", service);
            Response.WriteNoCache();

            // Explicitly set the charset to empty string 
            // We do this as certain git clients (jgit) require it to be empty.
            // If we don't set it, then it defaults to utf-8, which breaks jgit's logic for detecting smart http
            Response.Charset = "";

            project = project.EndsWith(".git") ? project.Substring(0, project.Length - 4) : project;
            var repository = repositories.GetRepository(cat, subcat, project);

            if (repository == null)
            {
                return new NotFoundResult();
            }

            Response.PktWrite("# service=git-{0}\n", service);
            Response.PktFlush();

            if (service == "upload-pack")
            {
                repository.AdvertiseUploadPack(Response.OutputStream);
            }

            else if (service == "receive-pack")
            {
                repository.AdvertiseReceivePack(Response.OutputStream);
            }

            return new EmptyResult();
        }

        ActionResult DumbInfoRefs(string cat, string subcat, string project)
        {
            Response.WriteNoCache();

            Response.ContentType = "text/plain; charset=utf-8";
            var repository = repositories.GetRepository(cat, subcat, project);

            if (repository == null)
            {
                return new NotFoundResult();
            }

            repository.UpdateServerInfo();
            Response.WriteFile(Path.Combine(repository.GitDirectory(), "info/refs"));
            return new EmptyResult();
        }
    }
}