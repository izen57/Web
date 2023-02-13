using Model;

using System.Drawing;

namespace Logic
{
	public interface IStopwatchService
	{
		void EditName(Guid ownerId, string name);
		void Set(Guid ownerId);
		void Reset(Guid ownerId);
		long Stop(Guid ownerId);
		long AddStopwatchFlag(Guid ownerId);
		Stopwatch Get(Guid ownerId);
		void EditColor(Guid ownerId, Color stopwatchColor);
	}
}
