using Exceptions.AlarmClockExceptions;

using Logic;

using Microsoft.AspNetCore.Mvc;

using Model;

using WebAPI.DataTransferObject;

namespace WebAPI.Controllers
{
	[ApiController]
	[Produces("application/json")]
	[Route("api/v1/alarmclocks")]
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
		[HttpGet("")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AlarmClockDTO>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult GetAlarmClocks([FromQuery] QueryStringParameters param)
		{
			var list = _alarmClockService.GetAlarmClocks(param);

			List<AlarmClockDTO> listDTO = new();
			if (list.Count > 0)
				foreach (AlarmClock alarmClock in list)
					listDTO.Add(AlarmClockDTO.ToDTO(alarmClock));

			return new OkObjectResult(listDTO);
		}

		/// <summary>
		/// Создаёт новый будильник
		/// </summary>
		/// <param name="alarmClockDTO">Создаваемый будильник</param>
		/// <respons code="201">Будильник успешно создан</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		/// <respons code="500">Будильник на такие дату и время уже существует</respons>
		[HttpPost("")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AlarmClockDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult CreateAlarmClock([FromBody] AlarmClockDTO alarmClockDTO)
		{
			try
			{
				AlarmClock alarmClock = AlarmClockDTO.FromDTO(alarmClockDTO);
				alarmClockDTO = AlarmClockDTO.ToDTO(_alarmClockService.Create(alarmClock));
			}
			catch (AlarmClockCreateException e) when (e.InnerException.Message.Contains("Read-only file system"))
			{
				return StatusCode(403);
			}
			catch (AlarmClockCreateException e) when (e.InnerException.Message.Contains("already exists"))
			{
				return StatusCode(409);
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
		/// <respons code="403">У Вас нет прав доступа</respons>
		/// <respons code="404">Будильник не найден</respons>
		[HttpPut("{alarmClockTime}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AlarmClockDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult EditAlarmClock([FromBody] AlarmClockDTO alarmClockDTO, [FromRoute] DateTime alarmClockTime)
		{
			try
			{
				AlarmClock alarmClock = AlarmClockDTO.FromDTO(alarmClockDTO);
				alarmClockDTO = AlarmClockDTO.ToDTO(_alarmClockService.Edit(alarmClock, alarmClockTime));
			}
			catch (AlarmClockEditException e) when (e.InnerException.Message.Contains("Read-only file system"))
			{
				return StatusCode(403);
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
		[HttpGet("{alarmClockTime}")]
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
		/// <respons code="403">У Вас нет прав доступа</respons>
		/// <respons code="404">Будильник не найден</respons>
		[HttpDelete("{alarmClockTime}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteAlarmClock([FromRoute] DateTime alarmClockTime)
		{
			try
			{
				_alarmClockService.Delete(alarmClockTime);
			}
			catch (AlarmClockDeleteException e) when (e.InnerException.Message.Contains("Read-only file system"))
			{
				return StatusCode(403);
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
