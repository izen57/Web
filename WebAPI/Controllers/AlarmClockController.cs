using Exceptions.AlarmClockExceptions;

using Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Model;

using System.Drawing;

using WebAPI.DataTransferObject;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/v1/alarmclocks")]
	[Produces("application/json")]
	public class AlarmClockController: ControllerBase
	{
		readonly IAlarmClockService _alarmClockService;

		public AlarmClockController(IAlarmClockService alarmClockService)
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
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		[Authorize]
		[HttpGet("")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AlarmClockDTO>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public ActionResult GetAlarmClocks([FromQuery] QueryStringParameters param)
		{
			Guid userFromResponse = Guid.Parse(HttpContext.Items["User ID"].ToString());
			var list = _alarmClockService.GetAlarmClocks(userFromResponse, param);

			List<AlarmClockDTO> listDTO = new();
			if (list.Count > 0)
				foreach (AlarmClock alarmClock in list)
					listDTO.Add(AlarmClockDTO.ToDTO(alarmClock));

			return new OkObjectResult(listDTO);
		}

		/// <summary>
		/// Создаёт новый будильник
		/// </summary>
		/// <param name="alarmClockDTOCreate">Создаваемый будильник</param>
		/// <respons code="201">Будильник успешно создан</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		[Authorize]
		[HttpPost("")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AlarmClockDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public ActionResult CreateAlarmClock([FromBody] AlarmClockDTOCreate alarmClockDTOCreate)
		{
			if (alarmClockDTOCreate == null)
				return StatusCode(400);
			Guid userFromResponse = Guid.Parse(HttpContext.Items["User ID"].ToString());
			AlarmClock alarmClock = new(
				Guid.NewGuid(),
				alarmClockDTOCreate.AlarmTime,
				alarmClockDTOCreate.Name,
				userFromResponse,
				Color.FromName(alarmClockDTOCreate.AlarmClockColor),
				alarmClockDTOCreate.IsWorking
			);
			try
			{
				alarmClock = _alarmClockService.Create(alarmClock);
			}
			catch (AlarmClockCreateException e) when (e.InnerException.Message.Contains("Read-only file system"))
			{
				return StatusCode(403);
			}
			catch (AlarmClockCreateException e) when (e.InnerException.Message.Contains("already exists"))
			{
				return StatusCode(409);
			}
			return new CreatedResult("Alarm clocks' isolated storage", AlarmClockDTO.ToDTO(alarmClock));
		}

		/// <summary>
		/// Обновляет существующий будильник по идентификатору
		/// </summary>
		/// <param name="alarmClockDTOCreate">Изменяемый будильник</param>
		/// <param name="Id">Идентификатор будильника</param>
		/// <respons code="200">Существующий будильник изменён</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		/// <respons code="404">Будильник не найден</respons>
		[Authorize]
		[HttpPut("{Id}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AlarmClockDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult EditAlarmClock([FromBody] AlarmClockDTOCreate alarmClockDTOCreate, [FromRoute] Guid Id)
		{
			AlarmClock? alarmClock = _alarmClockService.GetAlarmClock(Id);
			if (alarmClock == null)
				return new NotFoundResult();

			alarmClock.Name = alarmClockDTOCreate.Name;
			alarmClock.AlarmTime = alarmClockDTOCreate.AlarmTime;
			alarmClock.AlarmClockColor = Color.FromName(alarmClockDTOCreate.AlarmClockColor);
			alarmClock.IsWorking = alarmClockDTOCreate.IsWorking;

			try
			{
				alarmClock = _alarmClockService.Edit(alarmClock);
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
			return new OkObjectResult(AlarmClockDTO.ToDTO(alarmClock));
		}

		/// <summary>
		/// Возвращает будильник по заданному идентификатору
		/// </summary>
		/// <param name="Id">Идентификатор искомого будильника</param>
		/// <returns>Изменённый будильник</returns>
		/// <respons code="200">Будильник найден</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		/// <respons code="404">Будильник не найден</respons>
		[Authorize]
		[HttpGet("{Id}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AlarmClockDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetAlarmClock([FromRoute] Guid Id)
		{
			AlarmClock? alarmClock = _alarmClockService.GetAlarmClock(Id);
			if (alarmClock != null)
				return new OkObjectResult(AlarmClockDTO.ToDTO(alarmClock));
			else
				return new NotFoundResult();
		}

		/// <summary>
		/// Удаляет будильник по идентификатору
		/// </summary>
		/// <param name="Id">Идентификатор искомого будильника</param>
		/// <respons code="200">Будильник успешно удалён</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		/// <respons code="404">Будильник не найден</respons>
		[Authorize]
		[HttpDelete("{Id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteAlarmClock([FromRoute] Guid Id)
		{
			Guid userFromResponse = Guid.Parse(HttpContext.Items["User ID"].ToString());
			try
			{
				_alarmClockService.Delete(Id, userFromResponse);
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
