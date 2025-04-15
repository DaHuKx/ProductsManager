using ProductsManager.Infrastructure.DataBase;
using ProductsManager.Infrastructure.DataBase.Entities;
using ProductsManager.Infrastructure.DataBase.Enums;
using ProductsManager.Infrastructure.Repositories.Interfaces;

namespace ProductsManager.Infrastructure.Repositories
{
    public class UsersRepository : RepositoryBase<BotUser>, IUsersRepository
    {
        public BotUser? GetByNetId(BotType type, string netId)
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                return context.BotUsers.FirstOrDefault(u => u.BotType == type && u.NetId == netId);
            }
        }

        public async Task<BotUser?> UpdateUserPlaceAsync(BotUser user, UserPlace place)
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                var dbUser = context.BotUsers.First(u => u.Id == user.Id);

                dbUser.Place = place;

                await context.SaveChangesAsync();

                return dbUser;
            }
        }
    }
}
