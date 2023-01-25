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

		public void Delete(Guid guid)
		{
			_repository.Delete(guid);
		}

		public AlarmClock? GetAlarmClock(Guid guid)
		{
			return _repository.GetAlarmClock(guid);
		}

		public List<AlarmClock> GetAlarmClocks()
		{
			return _repository.GetAlarmClocks();
		}

		public List<AlarmClock> GetAlarmClocksByQuery(QueryStringParameters param)
		{
			return _repository.GetAlarmClocksByQuery(param);
		}

		public void InvertWork(AlarmClock alarmClock)
		{
			alarmClock.IsWorking = !alarmClock.IsWorking;
			Edit(alarmClock);

			Log.Logger.Information($"Будильник {alarmClock.Id} остановлен.");
		}
	}
}