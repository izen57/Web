using Logic;

using Microsoft.AspNetCore.Mvc;
using Model;

using System.Drawing;

namespace WebAPI.Controllers
{
	[ApiController]
	public class StopwatchController
	{
		private readonly IStopwatchService _stopwatchService;

		public StopwatchController(IStopwatchService stopwatchService)
		{
			_stopwatchService = stopwatchService;
		}

		/// <summary>
		/// Возращает секундомер
		/// </summary>
		/// <returns>Секундомер</returns>
		/// <respons code="200">Секундомер найден</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[Route("/stopwatch")]
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Stopwatch))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetStopwatch()
		{
			return new OkObjectResult(_stopwatchService.Get());
		}

		/// <summary>
		/// Запускает секундомер
		/// </summary>
		/// <returns>Секундомер</returns>
		/// <respons code="200">Секундомер запущен</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[Route("/stopwatch/set")]
		[HttpPatch]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Stopwatch))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult SetStopwatch()
		{
			_stopwatchService.Set();

			return new OkObjectResult(_stopwatchService.Get());
		}

		/// <summary>
		/// Сбрасывает секундомер
		/// </summary>
		/// <returns>Секундомер</returns>
		/// <respons code="200">Секундомер сброшен</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[Route("/stopwatch/reset")]
		[HttpPatch]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Stopwatch))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult ResetStopwatch()
		{
			_stopwatchService.Reset();

			return new OkObjectResult(_stopwatchService.Get());
		}

		/// <summary>
		/// Останавливает секундомер
		/// </summary>
		/// <returns>Секундомер</returns>
		/// <respons code="200">Секундомер остановлен</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[Route("/stopwatch/stop")]
		[HttpPatch]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Stopwatch))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult StopStopwatch()
		{
			_stopwatchService.Stop();

			return new OkObjectResult(_stopwatchService.Get());
		}

		/// <summary>
		/// Устанавливает временной флаг
		/// </summary>
		/// <returns>Секундомер</returns>
		/// <respons code="200">Флаг добавлен</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[Route("/stopwatch/addTimeFlag")]
		[HttpPatch]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Stopwatch))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult AddTimeFlags()
		{
			_stopwatchService.AddStopwatchFlag();

			return new OkObjectResult(_stopwatchService.Get());
		}

		/// <summary>
		/// Изменяет цвет секундомера
		/// </summary>
		/// <returns>Секундомер</returns>
		/// <param name="stopwatchColor">Новый цвет секундомера</param>
		/// <respons code="200">Цвет изменён</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[Route("/stopwatch/color")]
		[HttpPatch]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Stopwatch))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult ChangeColor([FromBody] string stopwatchColor)
		{
			_stopwatchService.EditColor(Color.FromName(stopwatchColor));

			return new OkObjectResult(_stopwatchService.Get());
		}
	}
}
