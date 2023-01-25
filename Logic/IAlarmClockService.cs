using Model;

namespace Logic
{
	public interface IAlarmClockService
	{
		AlarmClock Create(AlarmClock alarmClock);
		AlarmClock Edit(AlarmClock alarmClock);
		void Delete(Guid guid);
		AlarmClock? GetAlarmClock(Guid guid);
		List<AlarmClock> GetAlarmClocks();
		List<AlarmClock> GetAlarmClocksByQuery(QueryStringParameters param);
		void InvertWork(AlarmClock alarmClock);
	}
}
