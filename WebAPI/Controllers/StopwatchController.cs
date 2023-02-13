using Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Drawing;

using WebAPI.DataTransferObject;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/v1/stopwatch")]
	[Produces("application/json")]
	public class StopwatchController: ControllerBase
	{
		private readonly IStopwatchService _stopwatchService;

		public StopwatchController(IStopwatchService stopwatchService)
		{
			_stopwatchService = stopwatchService;
		}

		/// <summary>
		/// Возращает секундомер
		/// </summary>
		/// <returns>Текущее состояние секундомера</returns>
		/// <respons code="200">Секундомер найден</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		[Authorize]
		[HttpGet("")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StopwatchDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public ActionResult GetStopwatch()
		{
			Guid userFromResponse = Guid.Parse(HttpContext.Items["User ID"].ToString());

			return new OkObjectResult(StopwatchDTO.ToDTO(_stopwatchService.Get(userFromResponse)));
		}

		/// <summary>
		/// Изменяет параметры секундомера
		/// </summary>
		/// <returns>Изменённый секундомер</returns>
		/// <param name="stopwatchDTO">Изменяемые поля параметра</param>
		/// <respons code="200">Секундомер изменён</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="401">Необходима авторизация</respons>
		/// <respons code="403">У Вас нет прав доступа</respons>
		[Authorize]
		[HttpPatch("")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StopwatchDTO))]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult StopwatchAction([FromBody] StopwatchDTO stopwatchDTO)
		{
			Guid userFromResponse = Guid.Parse(HttpContext.Items["User ID"].ToString());

			try
			{
				if (stopwatchDTO.Name != null)
					_stopwatchService.EditName(userFromResponse, stopwatchDTO.Name);

				if (stopwatchDTO.StopwatchColor != null)
					_stopwatchService.EditColor(userFromResponse, Color.FromName(stopwatchDTO.StopwatchColor));

				if (stopwatchDTO.IsWorking == true)
					_stopwatchService.Set(userFromResponse);
				else if (stopwatchDTO.IsWorking == false)
					_stopwatchService.Stop(userFromResponse);

				if (stopwatchDTO.ResetSignal == true)
					_stopwatchService.Reset(userFromResponse);
				if (stopwatchDTO.TimeFlags != null)
					_stopwatchService.AddStopwatchFlag(userFromResponse);
			}
			catch (Exception e) when (e.Message.Contains("Read-only file system"))
			{
				return StatusCode(403);
			}
			return new OkObjectResult(StopwatchDTO.ToDTO(_stopwatchService.Get(userFromResponse)));
		}
	}
}
