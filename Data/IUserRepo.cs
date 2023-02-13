using Model;

namespace Repositories
{
	public interface IUserRepo
	{
		User Create(User user);
		User Edit(User user);
		void Delete(Guid guid);
		User? GetUser(Guid guid);
		List<User> GetUsers();
		List<User> GetUsers(QueryStringParameters param);
	}
}
