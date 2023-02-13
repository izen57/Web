using Model;

namespace Repositories
{
	public interface IAlarmClockRepo
	{
		AlarmClock Create(AlarmClock alarmClock);
		AlarmClock Edit(AlarmClock alarmClock);
		void Delete(Guid guid, Guid ownerId);
		AlarmClock? GetAlarmClock(Guid guid);
		List<AlarmClock> GetAlarmClocks(Guid ownerId);
		List<AlarmClock> GetAlarmClocks(Guid ownerId, QueryStringParameters param);
	}
}