namespace Exceptions.NoteExceptions
{
	public class NoteCreateException: Exception
	{
		public NoteCreateException(): base() { }
		public NoteCreateException(string message): base(message) { }
		public NoteCreateException(string message, Exception inner): base(message, inner) { }
	}
}
