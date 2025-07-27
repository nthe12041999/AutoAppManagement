using System.ComponentModel.DataAnnotations.Schema;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.ViewModel.Account
{
    public class AccountUpdate
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public GenderType Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Introduce { get; set; }
        public string ImgAvatar { get; set; }
        public string MaxAccountFb { get; set; }
        public ImgInfor ImgContent { get; set; }
    }
}
