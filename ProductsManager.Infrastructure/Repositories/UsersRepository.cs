using ProductsManager.Infrastructure.DataBase;
using ProductsManager.Infrastructure.DataBase.Entities;
using ProductsManager.Infrastructure.DataBase.Enums;
using ProductsManager.Infrastructure.Repositories.Interfaces;

namespace ProductsManager.Infrastructure.Repositories
{
    public class UsersRepository : RepositoryBase<BotUser>, IUsersRepository
    {
        public UsersRepository(ProductsManagerDb productsManager) : base(productsManager)
        {

        }

        public BotUser? GetByNetId(BotType type, string netId)
        {
            return Context.BotUsers.FirstOrDefault(u => u.BotType == type && u.NetId == netId);
        }

        public async Task<BotUser?> UpdateUserPlaceAsync(BotUser user, UserPlace place)
        {
            var dbUser = Context.BotUsers.First(u => u.Id == user.Id);

            dbUser.Place = place;

            await Context.SaveChangesAsync();

            return dbUser;
        }
    }
}
