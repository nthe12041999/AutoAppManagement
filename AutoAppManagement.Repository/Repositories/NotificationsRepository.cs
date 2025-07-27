using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Repository.Repositories.Base;

namespace AutoAppManagement.Repository.Repositories
{
    public interface INotificationsRepository : IBaseRepository<Notification>
    {
    }
    public class NotificationsRepository : BaseRepository<Notification>, INotificationsRepository
    {
        public NotificationsRepository(AutoAppManagementContext context) : base(context)
        {

        }
    }
}
