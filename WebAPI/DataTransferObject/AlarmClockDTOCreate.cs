namespace WebAPI.DataTransferObject
{
	public class AlarmClockDTOCreate
	{
		public DateTime AlarmTime { get; set; }
		public string Name { get; set; }
		public string AlarmClockColor { get; set; }
		public bool IsWorking { get; set; }

		public AlarmClockDTOCreate(DateTime alarmTime, string name, string alarmClockColor, bool isWorking)
		{
			AlarmTime = alarmTime;
			Name = name;
			AlarmClockColor = alarmClockColor;
			IsWorking = isWorking;
		}
	}
}
