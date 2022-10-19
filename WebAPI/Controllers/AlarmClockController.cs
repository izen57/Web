using Logic;

using Microsoft.AspNetCore.Mvc;

using Model;

using Serilog;

namespace WebAPI.Controllers
{
	[ApiController]
	public class AlarmClockController: ControllerBase
	{
		private readonly IAlarmClockService _alarmClockService;

		public AlarmClockController(IAlarmClockService alarmClockService)
		{
			_alarmClockService = alarmClockService;
		}

		/// <summary>
		/// Возвращает информацию о всех будильниках
		/// </summary>
		/// <returns>Список всех будильников</returns>
		/// <respons code="200">Будильники успешно возвращены</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Какие-либо будильники не найдены</respons>
		[Route("/alarmclock")]
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AlarmClock>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetAllAlarmClocks()
		{
			var list = _alarmClockService.GetAllAlarmClocks();
			if (list.Count == 0)
				return new NotFoundResult();

			return new OkObjectResult(list);
		}

		/// <summary>
		/// Создаёт новый будильник
		/// </summary>
		/// <param name="alarmClock">Создаваемый будильник</param>
		/// <returns>Созданный будильник</returns>
		/// <respons code="201">Будильник успешно создан</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[Route("/alarmclock")]
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult CreateAlarmClock([FromBody] AlarmClock alarmClock)
		{
			_alarmClockService.Create(alarmClock);
			return new CreatedResult("AlarmClocks isolated storage.", null);
		}

		/// <summary>
		/// Обновляет существующий будильник по дате и времени
		/// </summary>
		/// <param name="alarmClock">Изменяемый будильник</param>
		/// <param name="alarmClockTime">Старое время будильника</param>
		/// <returns>Изменённый будильник</returns>
		/// <respons code="200">Существующий будильник изменён</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Будильник не найден</respons>
		[Route("/alarmclock/{alarmClockTime}")]
		[HttpPut]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult EditAlarmClock([FromBody] AlarmClock alarmClock, [FromRoute] DateTime alarmClockTime)
		{
			try
			{
				_alarmClockService.Edit(alarmClock, alarmClockTime);
			}
			catch
			{
				return new NotFoundResult();
			}
			return new OkResult();
		}

		/// <summary>
		/// Возвращает будильник по заданным дате и времени
		/// </summary>
		/// <param name="alarmClockTime">Время искомого будильника</param>
		/// <returns>Изменённый будильник</returns>
		/// <respons code="200">Будильник найден</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Будильник не найден</respons>
		[Route("/alarmclock/{alarmClockTime}")]
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AlarmClock))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetAlarmClock([FromRoute] DateTime alarmClockTime)
		{
			var alarmClock = _alarmClockService.GetAlarmClock(alarmClockTime);
			if (alarmClock != null)
				return new OkObjectResult(alarmClock);
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
		[Route("/alarmclock/{alarmClockTime}")]
		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteAlarmClock([FromRoute] DateTime alarmClockTime)
		{
			try
			{
				_alarmClockService.Delete(alarmClockTime);
			}
			catch
			{
				return new NotFoundResult();
			}
			return new OkResult();
		}

		/// <summary>
		/// Инвертирует работу будильника
		/// </summary>
		/// <param name="alarmClockTime">Время искомого будильника</param>
		/// <returns>Изменённый будильник</returns>
		/// <respons code="200">Работа будильника успешно инвертирована</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Будильник не найден</respons>
		[Route("/alarmclock/{alarmClockTime}/invertWork")]
		[HttpPatch]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult InvertAlarmClock([FromRoute] DateTime alarmClockTime)
		{
			try
			{
				_alarmClockService.InvertWork(_alarmClockService.GetAlarmClock(alarmClockTime));
			}
			catch
			{
				return new NotFoundResult();
			}
			return new OkObjectResult(true);
		}
	}
}
