namespace Exceptions.AlarmClockExceptions
{
	public class AlarmClockDeleteException: Exception
	{
		public AlarmClockDeleteException(): base() { }
		public AlarmClockDeleteException(string message): base(message) { }
		public AlarmClockDeleteException(string message, Exception inner): base(message, inner) { }
	}
}
