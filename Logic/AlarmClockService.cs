using Repositories;
using Model;
using Serilog;

namespace Logic {
	public class AlarmClockService: IAlarmClockService {
		IAlarmClockRepo _repository;

		public AlarmClockService(IAlarmClockRepo repo) {
			_repository = repo ?? throw new ArgumentNullException(nameof(repo));
		}

		public void Create(AlarmClock alarmClock) {
			_repository.Create(alarmClock);
		}

		public void Edit(AlarmClock alarmClock, DateTime oldTime) {
			_repository.Edit(alarmClock, oldTime);
		}

		public void Delete(DateTime alarmTime) {
			_repository.Delete(alarmTime);
		}

		public AlarmClock? GetAlarmClock(DateTime dateTime)
		{
			return _repository.GetAlarmClock(dateTime);
		}

		public List<AlarmClock> GetAllAlarmClocks() {
			return _repository.GetAllAlarmClocksList();
		}

		public void InvertWork(AlarmClock alarmClock) {
			alarmClock.IsWorking = !alarmClock.IsWorking;
			Edit(alarmClock, alarmClock.AlarmTime);

			Log.Logger.Information($"Будильник остановлен. Время будильника: {alarmClock.AlarmTime}");
		}
	}
}