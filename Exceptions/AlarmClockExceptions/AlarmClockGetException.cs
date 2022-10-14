namespace Exceptions.AlarmClockExceptions
{
	public class AlarmClockGetException: Exception
	{
		public AlarmClockGetException(): base() { }
		public AlarmClockGetException(string message): base(message) { }
		public AlarmClockGetException(string message, Exception inner): base(message, inner) { }
	}
}
