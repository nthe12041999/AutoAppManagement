using AutoAppManagement.Models.ViewModel.Account;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoAppManagement.WebApp.Models
{
    [NotMapped]
    public class AccountUpdateFile : AccountUpdate
    {
        public List<IFormFile> SelectedImage { get; set; } = new List<IFormFile>();
    }
}
