using Model;

using Repositories;

using Serilog;

namespace Logic
{
	public class UserService: IUserService
	{
		IUserRepo _repository;

		public UserService(IUserRepo repo)
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.File("logs/log.txt")
				.CreateLogger();

			_repository = repo ?? throw new ArgumentNullException(nameof(repo));
		}

		public User Create(User user)
		{
			return _repository.Create(user);
		}

		public User Edit(User user)
		{
			return _repository.Edit(user);
		}

		public void Delete(Guid guid)
		{
			_repository.Delete(guid);
		}

		public List<User> GetUsers()
		{
			return _repository.GetUsers();
		}

		public User? GetUser(Guid guid)
		{
			return _repository.GetUser(guid);
		}

		public List<User> GetUsersByQuery(QueryStringParameters param)
		{
			return _repository.GetUsersByQuery(param);
		}
	}
}
