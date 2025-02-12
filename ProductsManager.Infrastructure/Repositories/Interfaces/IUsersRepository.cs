using ProductsManager.Infrastructure.DataBase.Entities;
using ProductsManager.Infrastructure.DataBase.Enums;

namespace ProductsManager.Infrastructure.Repositories.Interfaces
{
    public interface IUsersRepository : IRepositoryBase<BotUser>
    {
        BotUser? GetByNetId(BotType type, string netId);
        Task<BotUser?> UpdateUserPlaceAsync(BotUser user, UserPlace place);
    }
}
