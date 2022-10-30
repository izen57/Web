using Model;

namespace Repositories {
	public interface IAlarmClockRepo {
		AlarmClock Create(AlarmClock alarmClock);
		AlarmClock Edit(AlarmClock alarmClock, DateTime oldTime);
		void Delete(DateTime alarmTime);
		AlarmClock? GetAlarmClock(DateTime alarmTime);
		List<AlarmClock> GetAllAlarmClocks();
		List<AlarmClock> GetAlarmClocksByQuery(QueryStringParameters param);
	}
}