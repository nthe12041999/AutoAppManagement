namespace AutoAppManagement.Models.ViewModel
{
    public interface IRestOutput : IResponseOutput<object>
    {

    }
    public class RestOutput : ResponseOutput<object>, IRestOutput
    {

    }
}
