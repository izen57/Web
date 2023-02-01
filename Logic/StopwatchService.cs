using Model;

using Serilog;

using System.Drawing;

namespace Logic
{
	public class StopwatchService: IStopwatchService
	{
		static Dictionary<Guid, Stopwatch> _stopwatches;

		public StopwatchService(Dictionary<Guid, Stopwatch> stopwatches)
		{
			_stopwatches = stopwatches;
		}

		public void EditName(Guid ownerId, string name)
		{
			_stopwatches[ownerId].Name = name;
		}

		public void Set(Guid ownerId)
		{
			_stopwatches[ownerId].Timing.Start();
			_stopwatches[ownerId].IsWorking = true;

			Log.Logger.Information($"Секундомер пользователя {ownerId} запущен.");
		}

		public long Stop(Guid ownerId)
		{
			_stopwatches[ownerId].Timing.Stop();
			_stopwatches[ownerId].IsWorking = false;

			Log.Logger.Information($"Секундомер пользователя {ownerId} остановлен.");

			return _stopwatches[ownerId].Timing.ElapsedMilliseconds;
		}

		public void Reset(Guid ownerId)
		{
			_stopwatches[ownerId].Timing.Reset();
			_stopwatches[ownerId].TimeFlags.Clear();
			_stopwatches[ownerId].IsWorking = false;

			Log.Logger.Information($"Секундомер пользователя {ownerId} и его флаги сброшены.");
		}

		public long AddStopwatchFlag(Guid ownerId)
		{
			_stopwatches[ownerId].TimeFlags.Add(DateTime.Now);
			return _stopwatches[ownerId].Timing.ElapsedMilliseconds;
		}

		public Stopwatch? Get(Guid ownerId)
		{
			return _stopwatches[ownerId];
		}

		public void EditColor(Guid ownerId, Color stopwatchColor)
		{
			_stopwatches[ownerId].StopwatchColor = stopwatchColor;
			Log.Logger.Information($"Цвет секундомера {ownerId} изменён.");
		}
	}
}