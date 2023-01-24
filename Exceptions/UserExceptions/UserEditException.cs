namespace Exceptions.UserExceptions
{
	public class UserEditException: Exception
	{
		public UserEditException(): base() { }
		public UserEditException(string message): base(message) { }
		public UserEditException(string message, Exception inner): base(message, inner) { }
	}
}
