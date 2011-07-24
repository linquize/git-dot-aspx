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

namespace GitAspx {
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;
	using GitAspx.Lib;
	using StructureMap;
	using StructureMap.Configuration.DSL;

	public class MvcApplication : HttpApplication {
        static void RegisterRoutes() {
            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            RouteTable.Routes.IgnoreRoute("favicon.ico");

            MapSimpleRoute("DirectoryList", "", "DirectoryList", "Index");
            MapSimpleRoute("DirectoryListCreate", "Create", "DirectoryList", "Create");

            MapSimpleRouteGetOnly("info-refs", "{project}.git/info/refs", "InfoRefs", "Execute");

            MapSimpleRoutePostOnly("upload-pack", "{project}.git/git-upload-pack", "Rpc", "UploadPack");
            MapSimpleRoutePostOnly("receive-pack", "{project}.git/git-receive-pack", "Rpc", "ReceivePack");

            MapSimpleRoute("get-info-packs", "{project}.git/info/packs", "Dumb", "GetInfoPacks");
            MapSimpleRoute("get-text-file", "{project}.git/HEAD", "Dumb", "GetTextFile");
            MapSimpleRoute("get-text-file2", "{project}.git/objects/info/alternates", "Dumb", "GetTextFile");
            MapSimpleRoute("get-text-file3", "{project}.git/objects/info/http-alternates", "Dumb", "GetTextFile");
            MapSimpleRoute("get-text-file4", "{project}.git/objects/info/{something}", "Dumb", "GetTextFile");
            MapSimpleRoute("get-loose-object", "{project}.git/objects/{segment1}/{segment2}", "Dumb", "GetLooseObject");
            MapSimpleRoute("get-pack-file", "{project}.git/objects/pack/pack-{filename}.pack", "Dumb", "GetPackFile");
            MapSimpleRoute("get-idx-file", "{project}.git/objects/pack/pack-{filename}.idx", "Dumb", "GetIdxFile");

            MapSimpleRoute("giturl", "{project}.git");

            MapSimpleRoute("tree-home", "{project}", "TreeView", "Index");
            MapSimpleRoute("tree", "{project}/tree/{tree}/{*path}", "TreeView", "Index");
            MapSimpleRoute("blob", "{project}/blob/{tree}/{*path}", "BlobView", "Index");
            MapSimpleRoute("download", "{project}/download/{tree}/{*path}", "DownloadView", "Index");

            MapSimpleRoute("settings", "settings/{key}/{value}", "WebBrowsingSettings", "Index");
		}

        static void MapSimpleRoute(string asName, string asUrl)
        {
            RouteTable.Routes.MapRoute(asName, asUrl);
        }

        static void MapSimpleRoute(string asName, string asUrl, string asController, string asAction)
        {
            RouteTable.Routes.MapRoute(asName, asUrl, new { controller = asController, action = asAction });
        }

        static void MapSimpleRouteGetOnly(string asName, string asUrl, string asController, string asAction)
        {
            RouteTable.Routes.MapRoute(asName, asUrl, new { controller = asController, action = asAction }, new { method = new HttpMethodConstraint("GET") });
        }

        static void MapSimpleRoutePostOnly(string asName, string asUrl, string asController, string asAction)
        {
            RouteTable.Routes.MapRoute(asName, asUrl, new { controller = asController, action = asAction }, new { method = new HttpMethodConstraint("POST") });
        }

		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();

			RegisterRoutes();

			ObjectFactory.Initialize(cfg => cfg.AddRegistry(new AppRegistry()));
			ControllerBuilder.Current.SetControllerFactory(new StructureMapControllerFactory());
		}

		class AppRegistry : Registry {
			public AppRegistry() {
				For<AppSettings>()
					.Singleton()
					.Use(AppSettings.FromAppConfig);
			}
		}
	}
}