using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Models.ViewModel.Account
{
    public class AccountGenericVM
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public int Level { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool IsLocked { get; set; } = false;

        public string Name { get; set; }

        public GenderType Gender { get; set; } = GenderType.Male;

        public DateTime? DateOfBirth { get; set; }

        public int MaxAccountFb { get; set; }
    }
}
