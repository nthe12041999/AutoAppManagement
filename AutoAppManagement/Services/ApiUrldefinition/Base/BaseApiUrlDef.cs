namespace AutoAppManagement.WebApp.Services.ApiUrldefinition.Base
{
    public class BaseApiUrlDef
    {
        public BaseApiUrlDef(string pathController)
        {
            _pathController = pathController;
        }
        protected string _pathController { get; set; }

        public string GetAll()
        {
            return @$"{_pathController}/GetAll";
        }

        public string GetById(long id)
        {
            return @$"{_pathController}/GetById?id={id}";
        }

        public string GetPaging()
        {
            return @$"{_pathController}/GetPaging";
        }

        public string SubmitData()
        {
            return @$"{_pathController}/SubmitData";
        }

        public string Delete(long id)
        {
            return @$"{_pathController}/Delete/{id}";
        }
    }
}
