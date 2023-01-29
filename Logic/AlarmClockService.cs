using Model;

using Repositories;

using Serilog;

namespace Logic
{
	public class AlarmClockService: IAlarmClockService
	{
		IAlarmClockRepo _repository;

		public AlarmClockService(IAlarmClockRepo repo)
		{
			_repository = repo ?? throw new ArgumentNullException(nameof(repo));
		}

		public AlarmClock Create(AlarmClock alarmClock)
		{
			return _repository.Create(alarmClock);
		}

		public AlarmClock Edit(AlarmClock alarmClock)
		{
			return _repository.Edit(alarmClock);
		}

		public void Delete(Guid guid, Guid ownerId)
		{
			_repository.Delete(guid, ownerId);
		}

		public AlarmClock? GetAlarmClock(Guid guid)
		{
			return _repository.GetAlarmClock(guid);
		}

		public List<AlarmClock> GetAlarmClocks(Guid ownerId)
		{
			return _repository.GetAlarmClocks(ownerId);
		}

		public List<AlarmClock> GetAlarmClocks(Guid ownerId, QueryStringParameters param)
		{
			return _repository.GetAlarmClocks(ownerId, param);
		}

		public void InvertWork(AlarmClock alarmClock)
		{
			alarmClock.IsWorking = !alarmClock.IsWorking;
			Edit(alarmClock);

			Log.Logger.Information($"Будильник {alarmClock.Id} остановлен.");
		}
	}
}