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

namespace GitAspx.Controllers {
	using System;
	using System.IO;
	using System.Web.Mvc;
	using System.Web.SessionState;
	using GitAspx.Lib;
	using ICSharpCode.SharpZipLib.GZip;

	// Handles project/git-upload-pack and project/git-receive-pack
	[SessionState(SessionStateBehavior.Disabled)]	
	public class RpcController : GitHttpBaseController {
		readonly RepositoryService repositories;

		public RpcController(RepositoryService repositories) {
			this.repositories = repositories;
		}

		[HttpPost]
		public ActionResult UploadPack(string cat, string subcat, string project) {
			return ExecuteRpc(cat, subcat, project, "upload-pack", repository => {
				repository.Upload(GetInputStream(), Response.OutputStream);
			});
		}

		[HttpPost]
        public ActionResult ReceivePack(string cat, string subcat, string project)
        {
			return ExecuteRpc(cat, subcat, project, "receive-pack", repository => {
				repository.Receive(GetInputStream(), Response.OutputStream);
			});
		}

		private Stream GetInputStream() {
			if(Request.Headers["Content-Encoding"] == "gzip") {
				return new GZipInputStream(Request.InputStream);
			}
			return Request.InputStream;
		}

        ActionResult ExecuteRpc(string cat, string subcat, string project, string rpc, Action<GitRepository> action)
        {
			if (!HasAccess(rpc, true)) {
				return new ForbiddenResult();
			}

			Response.ContentType = string.Format("application/x-git-{0}-result", rpc);
			Response.WriteNoCache();

			project = project.EndsWith(".git") ? project.Substring(0, project.Length - 4) : project;
			var repository = repositories.GetRepository(cat, subcat, project);

			if (repository == null) {
				return new NotFoundResult();
			}

			action(repository);

			return new EmptyResult();
		}
	}
}