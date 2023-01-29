using Model;

using Serilog;

using System.Drawing;

namespace Logic
{
	public class StopwatchService: IStopwatchService
	{
		static Stopwatch _stopwatch;

		public StopwatchService(Stopwatch stopwatch)
		{
			_stopwatch = stopwatch;
		}

		public void EditName(string name)
		{
			_stopwatch.Name = name;
		}

		public void Set()
		{
			_stopwatch.Timing.Start();
			_stopwatch.IsWorking = true;

			Log.Logger.Information("Секундомер запущен.");
		}

		public long Stop()
		{
			_stopwatch.Timing.Stop();
			_stopwatch.IsWorking = false;

			Log.Logger.Information("Секундомер остановлен.");

			return _stopwatch.Timing.ElapsedMilliseconds;
		}

		public void Reset()
		{
			_stopwatch.Timing.Reset();
			_stopwatch.TimeFlags.Clear();
			_stopwatch.IsWorking = false;

			Log.Logger.Information("Секундомер и его флаги сброшены.");
		}

		public long AddStopwatchFlag()
		{
			_stopwatch.TimeFlags.Add(DateTime.Now);
			return _stopwatch.Timing.ElapsedMilliseconds;
		}

		public Stopwatch Get()
		{
			return _stopwatch;
		}

		public void EditColor(Color stopwatchColor)
		{
			_stopwatch.StopwatchColor = stopwatchColor;

			Log.Logger.Information("Цвет секундомера изменён.");
		}
	}
}