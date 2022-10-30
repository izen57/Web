using Exceptions.AlarmClockExceptions;

using Logic;

using Microsoft.AspNetCore.Mvc;

using Model;

using WebAPI.DataTransferObject;

namespace WebAPI.Controllers
{
	[ApiController]
	[Produces("application/json")]
	public class AlarmClocksController: ControllerBase
	{
		private readonly IAlarmClockService _alarmClockService;

		public AlarmClocksController(IAlarmClockService alarmClockService)
		{
			_alarmClockService = alarmClockService;
		}

		/// <summary>
		/// Возвращает информацию о всех будильниках
		/// </summary>
		/// <returns>Список всех будильников</returns>
		/// <param name="param">Номер и размер страницы</param>
		/// <respons code="200">Будильники успешно возвращены</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[HttpGet("/api/v1/alarmclocks")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AlarmClockDTO>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult GetAlarmClocks([FromQuery] QueryStringParameters param)
		{
			var list = _alarmClockService.GetAlarmClocks(param);

			List<AlarmClockDTO> listDTO = new();
			foreach (AlarmClock alarmClock in list)
				listDTO.Append(AlarmClockDTO.ToDTO(alarmClock));

			return new OkObjectResult(listDTO);
		}

		/// <summary>
		/// Создаёт новый будильник
		/// </summary>
		/// <param name="alarmClockDTO">Создаваемый будильник</param>
		/// <respons code="201">Будильник успешно создан</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="500">Будильник на такие дату и время уже существует</respons>
		[HttpPost("api/v1/alarmclocks")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AlarmClockDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult CreateAlarmClock([FromBody] AlarmClockDTO alarmClockDTO)
		{
			try
			{
				AlarmClock alarmClock = AlarmClockDTO.FromDTO(alarmClockDTO);
				alarmClockDTO = AlarmClockDTO.ToDTO(_alarmClockService.Create(alarmClock));
			}
			catch
			{
				return StatusCode(500);
			}
			return new CreatedResult("Alarm clocks' isolated storage", alarmClockDTO);
		}

		/// <summary>
		/// Обновляет существующий будильник по дате и времени
		/// </summary>
		/// <param name="alarmClockDTO">Изменяемый будильник</param>
		/// <param name="alarmClockTime">Старое время будильника</param>
		/// <respons code="200">Существующий будильник изменён</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Будильник не найден</respons>
		[HttpPut("api/v1/alarmclocks/{alarmClockTime}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AlarmClockDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult EditAlarmClock([FromBody] AlarmClockDTO alarmClockDTO, [FromRoute] DateTime alarmClockTime)
		{
			try
			{
				AlarmClock alarmClock = AlarmClockDTO.FromDTO(alarmClockDTO);
				alarmClockDTO = AlarmClockDTO.ToDTO(_alarmClockService.Edit(alarmClock, alarmClockTime));
			}
			catch (AlarmClockEditException)
			{
				return new NotFoundResult();
			}
			catch
			{
				return new BadRequestResult();
			}
			return new OkObjectResult(alarmClockDTO);
		}

		/// <summary>
		/// Возвращает будильник по заданным дате и времени
		/// </summary>
		/// <param name="alarmClockTime">Время искомого будильника</param>
		/// <returns>Изменённый будильник</returns>
		/// <respons code="200">Будильник найден</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Будильник не найден</respons>
		[HttpGet("api/v1/alarmclocks/{alarmClockTime}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AlarmClockDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetAlarmClock([FromRoute] DateTime alarmClockTime)
		{
			var alarmClock = _alarmClockService.GetAlarmClock(alarmClockTime);
			if (alarmClock != null)
				return new OkObjectResult(AlarmClockDTO.ToDTO(alarmClock));
			else
				return new NotFoundResult();
		}

		/// <summary>
		/// Удаляет будильник по времени и дате
		/// </summary>
		/// <param name="alarmClockTime">Время искомого будильника</param>
		/// <returns>Изменённый будильник</returns>
		/// <respons code="200">Будильник успешно удалён</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Будильник не найден</respons>
		[HttpDelete("api/v1/alarmclocks/{alarmClockTime}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteAlarmClock([FromRoute] DateTime alarmClockTime)
		{
			try
			{
				_alarmClockService.Delete(alarmClockTime);
			}
			catch (AlarmClockDeleteException)
			{
				return new NotFoundResult();
			}
			catch
			{
				return new BadRequestResult();
			}
			return new OkResult();
		}
	}
}
