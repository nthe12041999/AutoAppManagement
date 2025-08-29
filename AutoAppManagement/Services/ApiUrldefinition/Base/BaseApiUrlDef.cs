namespace AutoAppManagement.WebApp.Services.ApiUrldefinition.Base
{
    public class BaseApiUrlDef
    {
        protected static string pathController { get; set; }

        public BaseApiUrlDef(string pathController)
        {
            BaseApiUrlDef.pathController = pathController;
        }

        public static string GetAll()
        {
            return @$"{pathController}/GetAll";
        }

        public static string GetById(long id)
        {
            return @$"{pathController}/GetById?id={id}";
        }

        public static string GetPaging()
        {
            return @$"{pathController}/GetPaging";
        }

        public static string SubmitData()
        {
            return @$"{pathController}/SubmitData";
        }
    }
}
