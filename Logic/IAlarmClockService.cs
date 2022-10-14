using Model;

namespace Logic
{
	public interface IAlarmClockService
	{
		void Create(AlarmClock alarmCloc);
		void Edit(AlarmClock alarmCloc, DateTime oldTime);
		void Delete(DateTime alarmTime);
		AlarmClock? GetAlarmClock(DateTime dateTime);
		List<AlarmClock> GetAllAlarmClocks();
		void InvertWork(AlarmClock alarmClock);
	}
}
