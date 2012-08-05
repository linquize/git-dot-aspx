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

namespace GitAspx.Lib {
	using System;
	using System.Configuration;
	using System.IO;

	public class AppSettings {
		public DirectoryInfo RepositoriesDirectory { get; set; }
		public bool UploadPack { get; set; }
		public bool ReceivePack { get; set; }
        public int RepositoryLevel { get; set; }
        public string UserHomeDirectory { get; set; }

        public static AppSettings FromAppConfig() {
			var settings = new AppSettings();

			var path = ConfigurationManager.AppSettings["RepositoriesDirectory"];

			if (string.IsNullOrEmpty(path)) {
				throw new InvalidOperationException("The 'Repositories' AppSetting has not been initialised.");
			}

			if (!Directory.Exists(path)) {
				throw new DirectoryNotFoundException(
					string.Format("Could not find the directory '{0}' which is configured as the directory of repositories.", path));
			}

			settings.RepositoriesDirectory = new DirectoryInfo(path);

			var uploadPackRaw = ConfigurationManager.AppSettings["UploadPack"];
			var receivePackRaw = ConfigurationManager.AppSettings["ReceivePack"];

			bool uploadpack, receivePack;
			settings.UploadPack = bool.TryParse(uploadPackRaw, out uploadpack) ? uploadpack : false;
            settings.ReceivePack = bool.TryParse(receivePackRaw, out receivePack) ? receivePack : false;

            string lsRepositoryLevel = ConfigurationManager.AppSettings["RepositoryLevel"];
            int liRepositoryLevel;
            settings.RepositoryLevel = int.TryParse(lsRepositoryLevel, out liRepositoryLevel) ? liRepositoryLevel : 1;

            settings.UserHomeDirectory = ConfigurationManager.AppSettings["UserHomeDirectory"];
			return settings;
		}
	}
}