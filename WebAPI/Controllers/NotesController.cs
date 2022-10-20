using Logic;

using Microsoft.AspNetCore.Mvc;

using Model;

namespace WebAPI.Controllers
{
	[ApiController]
	public class NotesController: ControllerBase
	{
		private readonly INoteService _noteService;

		public NotesController(INoteService noteService)
		{
			_noteService = noteService;
		}

		/// <summary>
		/// Возвращает информацию о всех заметках
		/// </summary>
		/// <returns>Список всех заметок</returns>
		/// <respons code="200">Заметки успешно возвращены</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Какие-либо заметки не найдены</respons>
		[Route("/note")]
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Note>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetAllNotes()
		{
			var list = _noteService.GetAllNotesList();
			if (list.Count == 0)
				return new NotFoundResult();

			return new OkObjectResult(list);
		}

		/// <summary>
		/// Обновляет существующую заметку
		/// </summary>
		/// <returns>Изменённая заметка</returns>
		/// <param name="note">Изменяемая заметка</param>
		/// <param name="Id">Идентификатор заметки</param>
		/// <respons code="200">Существующая заметка изменена</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Заметка не найдена</respons>
		[Route("/note/{Id}")]
		[HttpPatch]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Note))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult EditNote([FromBody] Note note, [FromRoute] Guid Id)
		{
			Note n;
			try
			{
				n = _noteService.Edit(new Note(Id, note.Body, note.IsTemporal));
				return new OkObjectResult(n);
			}
			catch
			{
				return new NotFoundResult();
			}
		}

		/// <summary>
		/// Создаёт новую заметку
		/// </summary>
		/// <param name="note">Новая заметка</param>
		/// <respons code="201">Заметка успешно создана</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="409">Заметка с таким идентификатором уже существует</respons>
		[Route("/note")]
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Note))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult CreateNote([FromBody] Note note)
		{
			Note n;
			try
			{
				n = _noteService.Create(new Note(Guid.NewGuid(), note.Body, note.IsTemporal));
			}
			catch
			{
				return new ConflictResult();
			}
			return new CreatedResult("Notes isolated storage", n);
		}

		/// <summary>
		/// Возвращает заметку по её идентификатору
		/// </summary>
		/// <param name="Id">Идентификатор искомой заметки</param>
		/// <respons code="200">Заметка найдена</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Заметка не найдена</respons>
		[Route("/note/{Id}")]
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Note))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetNote([FromRoute] Guid Id)
		{
			Note? note;
			try
			{
				note = _noteService.GetNote(Id);
			}
			catch
			{
				return new NotFoundResult();
			}
			return new OkObjectResult(note);
		}

		/// <summary>
		/// Удаляет заметку по её идентификатору
		/// </summary>
		/// <param name="Id">Идентификатор искомой заметки</param>
		/// <respons code="200">Заметка успешно удалена</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="404">Заметка не найдена</respons>
		[Route("/note/{Id}")]
		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteNote([FromRoute] Guid Id)
		{
			try
			{
				_noteService.Delete(Id);
			}
			catch
			{
				return new NotFoundResult();
			}
			return new OkResult();
		}
	}
}
