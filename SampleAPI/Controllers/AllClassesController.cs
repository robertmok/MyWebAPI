using System.Web.Http;

namespace SampleAPI.Controllers
{
    public class AllClassesController : ApiController
    {
        public class DetailsList
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
