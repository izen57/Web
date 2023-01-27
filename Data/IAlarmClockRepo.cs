using Model;

namespace Repositories
{
	public interface IAlarmClockRepo
	{
		AlarmClock Create(AlarmClock alarmClock);
		AlarmClock Edit(AlarmClock alarmClock);
		void Delete(Guid guid);
		AlarmClock? GetAlarmClock(Guid guid, Guid ownerId);
		List<AlarmClock> GetAlarmClocks(Guid ownerId);
		List<AlarmClock> GetAlarmClocksByQuery(QueryStringParameters param, Guid ownerId);
	}
}