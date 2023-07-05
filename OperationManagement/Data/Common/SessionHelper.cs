using Newtonsoft.Json;

namespace OperationManagement.Data.Common
{
    static public class SessionHelper
    {
        public const string ForgetPasswordKey = "ForgetPasswordModel";
        public const string JoinKey = "JoinVM";
        static public bool saveObject(HttpContext context, string key, Object obj)
        {
            try
            {
                context.Session.SetString(key, JsonConvert.SerializeObject(obj));
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }
        static public T? getObject<T>(HttpContext context, string key)
        {
            try
            {
                var stringObj = context.Session.GetString(key);
                var obj = JsonConvert.DeserializeObject<T>(stringObj);
                return obj;
            }
            catch (Exception err)
            {
                return default(T);
            }
        }
       

    }
}
