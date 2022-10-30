using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Model {
	public class Stopwatch {
		[Required]
		public string Name { get; set; }
		[Required]
		public Color StopwatchColor { get; set; }
		[Required]
		public System.Diagnostics.Stopwatch Timing { get; set; }
		[Required]
		public SortedSet<DateTime> TimeFlags { get; set; }
		[Required]
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