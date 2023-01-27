using Model;

namespace Logic
{
	public interface IUserService
	{
		User Create(User user);
		User Edit(User user);
		void Delete(Guid guid);
		List<User> GetUsers();
		User? GetUser(Guid guid);
		List<User> GetUsersByQuery(QueryStringParameters param);
	}
}
