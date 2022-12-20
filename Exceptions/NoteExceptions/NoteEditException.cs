namespace Exceptions.NoteExceptions
{
	public class NoteEditException: Exception
	{
		public NoteEditException(): base() { }
		public NoteEditException(string message): base(message) { }
		public NoteEditException(string message, Exception inner): base(message, inner) { }
	}
}
