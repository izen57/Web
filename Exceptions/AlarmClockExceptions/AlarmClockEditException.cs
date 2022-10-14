namespace Exceptions.AlarmClockExceptions
{
	public class AlarmClockEditException: Exception
	{
		public AlarmClockEditException() : base() { }
		public AlarmClockEditException(string message) : base(message) { }
		public AlarmClockEditException(string message, Exception inner) : base(message, inner) { }
	}
}
