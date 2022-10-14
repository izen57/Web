namespace Exceptions.NoteExceptions
{
	public class NoteDeleteException: Exception
	{
		public NoteDeleteException(): base() { }
		public NoteDeleteException(string message): base(message) { }
		public NoteDeleteException(string message, Exception inner): base(message, inner) { }
	}
}
