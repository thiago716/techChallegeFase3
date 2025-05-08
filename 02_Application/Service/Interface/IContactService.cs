using Application.ViewModel;
using Core.Entity;

namespace Application.Service.Interface
{
    public interface IContactService : IService<Contact>
    {
        Task<IList<ContactResult>> GetAllByDddAsync(int dddId);
    }
}
