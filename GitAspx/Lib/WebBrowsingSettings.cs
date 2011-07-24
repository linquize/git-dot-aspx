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

using System.Globalization;

namespace GitAspx.Lib
{
    public class WebBrowsingSettings
    {
        CultureInfo moCultureObject;

        public string Culture
        {
            get { return CultureObject.Name; }
            set { CultureObject = CultureInfo.GetCultureInfo(value); }
        }
        public CultureInfo CultureObject
        {
            get { return moCultureObject ?? (moCultureObject = CultureInfo.InvariantCulture); }
            set { moCultureObject = value; }
        }

        public bool ShowTreeNodeDetails { get; set; }
    }
}