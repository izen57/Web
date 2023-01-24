using Model;

using System.Drawing;

namespace Logic
{
	public interface IStopwatchService
	{
		void EditName(string name);
		void Set();
		void Reset();
		long Stop();
		long AddStopwatchFlag();
		Stopwatch Get();
		public void EditColor(Color stopwatchColor);
	}
}
