using LCTM.Utils;
using System.Web.Mvc;

namespace MVC5AutoVersioningSample.Controllers
{
    public class CacheController : Controller
    {
        // GET: Cashe
        public ActionResult SetCache(string key, string value)
        {
            CacheUtil.SetSignaledCash(key, value);
            return Content($"{key} 에 {value} 가 설정 되었습니다.");
        }

        public ActionResult GetCache(string key)
        {
            var value = CacheUtil.GetSignaledCash(key);

            return Content("value : " + value?.ToString());
        }

        public ActionResult FlushCache(string key)
        {
            CacheUtil.FlushSignaledCash(key);
            key = key ?? "모든"
            return Content($"{key} 값이 삭제 되었습니다.");
        }
    }
}
