namespace GitAspx.Controllers
{
    using System.IO;
    using System.Web.Mvc;
    using System.Web.SessionState;
    using GitAspx.Lib;

    [SessionState(SessionStateBehavior.Disabled)]
    public class DumbController : GitHttpBaseController
    {
        readonly RepositoryService repositories;

        public DumbController(RepositoryService repositories)
        {
            this.repositories = repositories;
        }

        public ActionResult GetHead(string cat, string subcat, string project)
        {
            return WriteFile(cat, subcat, project, "HEAD", "text/plain");
        }

        public ActionResult GetAlternates(string cat, string subcat, string project)
        {
            return WriteFile(cat, subcat, project, "objects/info/alternates", "text/plain");
        }

        public ActionResult GetHttpAlternates(string cat, string subcat, string project)
        {
            return WriteFile(cat, subcat, project, "objects/info/http-alternates", "text/plain");
        }

        public ActionResult GetOtherInfo(string cat, string subcat, string project, string something)
        {
            return WriteFile(cat, subcat, project, "objects/info/" + something, "text/plain");
        }

        public ActionResult GetInfoPacks(string cat, string subcat, string project)
        {
            return WriteFile(cat, subcat, project, "info/packs", "text/plain; charset=utf-8");
        }

        public ActionResult GetLooseObject(string cat, string subcat, string project, string segment1, string segment2)
        {
            return WriteFile(cat, subcat, project, "objects/" + segment1 + "/" + segment2, "application/x-git-loose-object");
        }

        public ActionResult GetPackFile(string cat, string subcat, string project, string filename)
        {
            return WriteFile(cat, subcat, project, "objects/pack/pack-" + filename + ".pack", "application/x-git-packed-objects");
        }

        public ActionResult GetIdxFile(string cat, string subcat, string project, string filename)
        {
            return WriteFile(cat, subcat, project, "objects/pack/pack-" + filename + ".idx", "application/x-git-packed-objects-toc");
        }

        private ActionResult WriteFile(string cat, string subcat, string project, string path, string contentType)
        {
            Response.WriteNoCache();
            Response.ContentType = contentType;

            var repo = repositories.GetRepository(cat, subcat, project);
            string path2 = Path.Combine(repo.GitDirectory(), path.Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(path2))
                return new NotFoundResult();

            Response.WriteFile(path2);
            return new EmptyResult();
        }
    }
}