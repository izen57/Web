namespace Exceptions.AlarmClockExceptions
{
	public class AlarmClockCreateException: Exception
	{
		public AlarmClockCreateException(): base() { }
		public AlarmClockCreateException(string message): base(message) { }
		public AlarmClockCreateException(string message, Exception inner): base(message, inner) { }
	}
}