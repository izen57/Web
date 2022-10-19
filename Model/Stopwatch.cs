using System.Drawing;

namespace Model {
	public class Stopwatch {
		public string Name { get; set; }
		public Color StopwatchColor { get; set; }
		public System.Diagnostics.Stopwatch Timing { get; set; }
		public SortedSet<DateTime> TimeFlags { get; set; }
		public bool IsWorking { get; set; }
		
		public Stopwatch()
		{
		}

		public Stopwatch(string name, Color stopwatchColor, System.Diagnostics.Stopwatch timing, SortedSet<DateTime> timeFlags, bool isWorking) {
			Name = name;
			StopwatchColor = stopwatchColor;
			Timing = timing;
			TimeFlags = timeFlags;
			IsWorking = isWorking;
		}
	}
}