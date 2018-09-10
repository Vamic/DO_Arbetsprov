using System.Web;
using System.Web.Mvc;

namespace DO_Arbetsprov
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
