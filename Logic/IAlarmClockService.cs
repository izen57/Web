using Model;

namespace Logic
{
	public interface IAlarmClockService
	{
		AlarmClock Create(AlarmClock alarmCloc);
		AlarmClock Edit(AlarmClock alarmCloc, DateTime oldTime);
		void Delete(DateTime alarmTime);
		AlarmClock? GetAlarmClock(DateTime dateTime);
		List<AlarmClock> GetAllAlarmClocks();
		List<AlarmClock> GetAlarmClocksByQuery(QueryStringParameters param);
		void InvertWork(AlarmClock alarmClock);
	}
}
