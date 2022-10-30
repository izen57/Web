using Model;

using System.Drawing;

namespace WebAPI.DataTransferObject
{
	public class StopwatchDTO
	{
		public string Name { get; set; }
		public string StopwatchColor { get; set; }
		public System.Diagnostics.Stopwatch Timing { get; set; }
		public SortedSet<DateTime> TimeFlags { get; set; }
		public bool IsWorking { get; set; }
		
		public StopwatchDTO() { }

		public StopwatchDTO(string name, string stopwatchColor, System.Diagnostics.Stopwatch timing, SortedSet<DateTime> timeFlags, bool isWorking)
		{
			Name = name;
			StopwatchColor = stopwatchColor;
			Timing = timing;
			TimeFlags = timeFlags;
			IsWorking = isWorking;
		}

		public static StopwatchDTO ToDTO(Stopwatch stopwatch)
		{
			return new StopwatchDTO(
				stopwatch.Name,
				stopwatch.StopwatchColor.Name,
				stopwatch.Timing,
				stopwatch.TimeFlags,
				stopwatch.IsWorking
			);
		}

		public static Stopwatch FromDTO(StopwatchDTO stopwatchDTO)
		{
			return new Stopwatch(
				stopwatchDTO.Name,
				Color.FromName(stopwatchDTO.StopwatchColor),
				stopwatchDTO.Timing,
				stopwatchDTO.TimeFlags,
				stopwatchDTO.IsWorking
			);
		}
	}
}