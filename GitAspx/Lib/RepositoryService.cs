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

namespace GitAspx.Lib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using GitSharp;

    public class RepositoryService
    {
        readonly AppSettings appSettings;

        public bool SingleRepositoryOnly
        {
            get { return appSettings.RepositoryLevel == 0; }
        }

        public RepositoryService(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public string CombineRepositoryName(string cat, string subcat, string project)
        {
            if (appSettings.RepositoryLevel == 0)
                return "";
            if (appSettings.RepositoryLevel == 1)
                return project ?? "";
            if (appSettings.RepositoryLevel == 2)
                return cat + "/" + project;
            if (appSettings.RepositoryLevel == 3)
                return cat + "/" + subcat + "/" + project;
            throw new NotSupportedException();
        }

        string CombinePhysicalDir(string cat, string subcat, string project)
        {
            if (appSettings.RepositoryLevel == 0)
                return appSettings.RepositoriesDirectory.FullName;
            if (appSettings.RepositoryLevel == 1)
                return Path.Combine(appSettings.RepositoriesDirectory.FullName, project + ".git");
            if (appSettings.RepositoryLevel == 2)
                return Path.Combine(appSettings.RepositoriesDirectory.FullName, cat, project + ".git");
            if (appSettings.RepositoryLevel == 3)
                return Path.Combine(appSettings.RepositoriesDirectory.FullName, cat, subcat, project + ".git");
            throw new NotSupportedException();
        }

        public string CombineRepositoryCat(string cat, string subcat)
        {
            if (appSettings.RepositoryLevel == 0)
                return "";
            if (appSettings.RepositoryLevel == 1)
                return "";
            if (appSettings.RepositoryLevel == 2)
                return cat ?? "";
            if (appSettings.RepositoryLevel == 3)
                return cat + "/" + subcat;
            throw new NotSupportedException();
        }

        public IEnumerable<GitRepository> GetAllRepositories(string cat, string subcat)
        {
            return GetAllRepositories(CombineRepositoryCat(cat, subcat));
        }

        IEnumerable<GitRepository> GetAllRepositories(string prefix)
        {
            string prefix2 = Path.Combine(prefix.SplitSlashes_OrEmpty().ToArray());
            prefix2 = prefix2.Length > 0 ? prefix2 + Path.DirectorySeparatorChar : prefix2;
            return appSettings.RepositoriesDirectory
                .GetDirectories(prefix2 + "*")
                .Where(x => x.Name.EndsWith(".git", StringComparison.InvariantCultureIgnoreCase))
                .Select(a => GitRepository.Open(a, appSettings.RepositoriesDirectory.FullName))
                .ToList();
        }

        public GitRepository GetRepository(string cat, string subcat, string project)
        {
            var directory = CombinePhysicalDir(cat, subcat, project);

            if (!Directory.Exists(directory))
                return null;

            return new GitRepository(new DirectoryInfo(directory), appSettings.RepositoriesDirectory.FullName);
        }

        public Repository GetBackendRepository(string cat, string subcat, string project)
        {
            return new Repository(CombinePhysicalDir(cat, subcat, project));
        }

        public DirectoryInfo GetRepositoriesDirectory()
        {
            return appSettings.RepositoriesDirectory;
        }

        public void CreateRepository(string cat, string subcat, string project)
        {
            var directory = CombinePhysicalDir(cat, subcat, project);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);

                using (var repository = new GitSharp.Core.Repository(new DirectoryInfo(directory)))
                {
                    repository.Create(true);
                }
            }
        }
    }
}