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
using System.Reflection;
using System.Web.Mvc;
using GitAspx.Lib;

namespace GitAspx.Controllers
{
    public class WebBrowsingSettingsController : Controller
    {
        public ActionResult Index(string key, string value)
        {
            WebBrowsingSettings loSettings = Session["WebBrowsingSettings"] as WebBrowsingSettings;
            if (loSettings == null)
                Session["WebBrowsingSettings"] = loSettings = new WebBrowsingSettings();
            PropertyInfo loProp = typeof(WebBrowsingSettings).GetProperty(key);
            if (loProp != null)
            {
                object loValue = Convert.ChangeType(value, loProp.PropertyType);
                loProp.SetValue(loSettings, loValue, null);
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
    }
}