namespace Exceptions.UserExceptions
{
	public class UserCreateException: Exception
	{
		public UserCreateException(): base() { }
		public UserCreateException(string message): base(message) { }
		public UserCreateException(string message, Exception inner): base(message, inner) { }
	}
}
