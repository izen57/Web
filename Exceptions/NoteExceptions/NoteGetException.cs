namespace Exceptions.NoteExceptions
{
	public class NoteGetException: Exception
	{
		public NoteGetException(): base() { }
		public NoteGetException(string message): base(message) { }
		public NoteGetException(string message, Exception inner): base(message, inner) { }
	}
}
