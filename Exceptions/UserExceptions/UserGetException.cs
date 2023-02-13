namespace Exceptions.UserExceptions
{
	public class UserGetException : Exception
	{
		public UserGetException() : base() { }
		public UserGetException(string message) : base(message) { }
		public UserGetException(string message, Exception inner) : base(message, inner) { }
	}
}
