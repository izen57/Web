using Exceptions.UserExceptions;

using Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Model;

using WebAPI.DataTransferObject;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/v1/users")]
	public class UsersController: ControllerBase
	{
		readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		/// <summary>
		/// Регистрирует нового пользователя
		/// </summary>
		/// <param name="userDTOCreate">Логин и пароль нового пользователя</param>
		/// <returns>Новый пользователь</returns>
		/// <respons code="201">Новый пользователь успешно зарегистрирован</respons>
		/// <respons code="403">Недостаточно прав</respons>
		/// <respons code="500">Ошибка создания пользователя</respons>
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDTO))]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[HttpPost("register")]
		public ActionResult Register([FromBody] UserDTOCreate userDTOCreate)
		{
			User user = new(
				Guid.NewGuid(),
				userDTOCreate.Name,
				userDTOCreate.Password,
				new List<AlarmClock>(),
				new List<Note>()
			);
			try
			{
				_userService.Create(user);
				return new CreatedResult("Users' storage", UserDTO.ToDTO(user));
			}
			catch (UserCreateException e) when (e.InnerException.Message.Contains("Read-only file system"))
			{
				return StatusCode(403);
			}
			catch
			{
				return StatusCode(500);
			}
		}

		/// <summary>
		/// Список всех пользователей
		/// </summary>
		/// <param name="param">Номер и размер страницы</param>
		/// <returns>Новый пользователь</returns>
		/// <respons code="200">Пользователи успешно возвращены</respons>
		/// <respons code="403">Недостаточно прав</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDTO>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[Authorize]
		[HttpGet("")]
		public ActionResult GetUsers([FromQuery] QueryStringParameters param)
		{
			List<UserDTO> userDTOs = new();
			foreach (User user in _userService.GetUsers(param))
				userDTOs.Add(UserDTO.ToDTO(user));

			return new OkObjectResult(userDTOs);
		}

		/// <summary>
		/// Находит пользователя по идентификатору
		/// </summary>
		/// <param name="Id">Идентификатор искомого пользователя</param>
		/// <returns>Новый пользователь</returns>
		/// <respons code="200">Пользователь найден</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="403">Недостаточно прав</respons>
		/// <respons code="404">Пользователь не найден</respons>
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Authorize]
		[HttpGet("{Id}")]
		public ActionResult GetUser([FromQuery] Guid Id)
		{
			UserDTO? userDTO;
			try
			{
				userDTO = UserDTO.ToDTO(_userService.GetUser(Id));
			}
			catch (UserGetException)
			{
				return new NotFoundResult();
			}
			catch
			{
				return StatusCode(500);
			}

			return new OkObjectResult(userDTO);
		}

		/// <summary>
		/// Изменяет существующего пользователя
		/// </summary>
		/// <param name="userDTOCreate">Изменяемый пользователь</param>
		/// <param name="Id">Идентификатор пользователя</param>
		/// <returns>Изменённый пользователь</returns>
		/// <respons code="200">Существующий пользователь успешно изменён</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="403">Недостаточно прав</respons>
		/// <respons code="404">Пользователь не найден</respons>
		[Authorize]
		[HttpPatch("{Id}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult EditUser([FromBody] UserDTOCreate userDTOCreate, [FromRoute] Guid Id)
		{
			User? user = _userService.GetUser(Id);
			if (user == null)
				return new NotFoundResult();
			user.Name = userDTOCreate.Name;
			user.Password = userDTOCreate.Password;

			try
			{
				User editableUser = _userService.Edit(user);
				return new CreatedResult("Users' storage", UserDTO.ToDTO(editableUser));
			}
			catch (UserEditException e) when (e.InnerException.Message.Contains("Read-only file system"))
			{
				return StatusCode(403);
			}
			catch
			{
				return StatusCode(500);
			}
		}

		/// <summary>
		/// Удаляет пользователя и все его данные по его идентификатору
		/// </summary>
		/// <param name="Id">Идентификатор пользователя</param>
		/// <respons code="200">Пользователь успешно удалён</respons>
		/// <respons code="400">Ошибка синтаксиса</respons>
		/// <respons code="403">Недостаточно прав</respons>
		/// <respons code="404">Пользователь не найден</respons>
		[Authorize]
		[HttpDelete("{Id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult DeleteUser([FromRoute] Guid Id)
		{
			try
			{
				_userService.Delete(Id);
			}
			catch (UserDeleteException e) when (e.InnerException.Message.Contains("Read-only file system"))
			{
				return StatusCode(403);
			}
			catch (UserDeleteException)
			{
				return new NotFoundResult();
			}
			catch
			{
				return StatusCode(500);
			}
			return new OkResult();
		}
	}
}
