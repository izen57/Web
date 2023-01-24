using Model;

using System.Drawing;

namespace WebAPI.DataTransferObject
{
	public class AlarmClockDTO
	{
		public DateTime AlarmTime { get; set; }
		public string Name { get; set; }
		public string AlarmClockColor { get; set; }
		public bool IsWorking { get; set; }

		public AlarmClockDTO() { }

		public AlarmClockDTO(DateTime alarmTime, string name, string alarmClockColor, bool isWorking)
		{
			AlarmTime = alarmTime;
			Name = name;
			AlarmClockColor = alarmClockColor;
			IsWorking = isWorking;
		}

		public static AlarmClockDTO ToDTO(AlarmClock alarmClock)
		{ 
			return new AlarmClockDTO(
				alarmClock.AlarmTime,
				alarmClock.Name,
				alarmClock.AlarmClockColor.Name,
				alarmClock.IsWorking
			);
		}

		public static AlarmClock FromDTO(AlarmClockDTO alarmClockDTO, Guid ownerId)
		{
			return new AlarmClock(
				alarmClockDTO.AlarmTime,
				alarmClockDTO.Name,
				ownerId,
				Color.FromName(alarmClockDTO.AlarmClockColor),
				alarmClockDTO.IsWorking
			);
		}
	}
}
