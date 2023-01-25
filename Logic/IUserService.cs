using Model;

namespace Logic
{
	public interface IUserService
	{
		User Create(User user);
		User Edit(User user);
		void Delete(Guid id);
		List<User> GetUsers();
		User? GetUser(Guid guid);
	}
}
