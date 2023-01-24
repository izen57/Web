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

		public AlarmClock Edit(AlarmClock alarmClock, DateTime oldTime)
		{
			return _repository.Edit(alarmClock, oldTime);
		}

		public void Delete(DateTime alarmTime)
		{
			_repository.Delete(alarmTime);
		}

		public AlarmClock? GetAlarmClock(DateTime dateTime)
		{
			return _repository.GetAlarmClock(dateTime);
		}

		public List<AlarmClock> GetAllAlarmClocks()
		{
			return _repository.GetAllAlarmClocks();
		}

		public List<AlarmClock> GetAlarmClocksByQuery(QueryStringParameters param)
		{
			return _repository.GetAlarmClocksByQuery(param);
		}

		public void InvertWork(AlarmClock alarmClock)
		{
			alarmClock.IsWorking = !alarmClock.IsWorking;
			Edit(alarmClock, alarmClock.AlarmTime);

			Log.Logger.Information($"Будильник остановлен. Время будильника: {alarmClock.AlarmTime}");
		}
	}
}