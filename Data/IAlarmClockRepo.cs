using Model;

namespace Repositories {
	public interface IAlarmClockRepo {
		void Create(AlarmClock alarmClock);
		void Edit(AlarmClock alarmClock, DateTime oldTime);
		void Delete(DateTime alarmTime);
		AlarmClock? GetAlarmClock(DateTime alarmTime);
		List<AlarmClock> GetAllAlarmClocksList();
	}
}