using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StutiBox.Actors;
using StutiBox.Models;
using Autofac;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StutiBox.Controllers
{
    [Route("api/[controller]")]
    public class PlayerController : Controller
    {
        IPlayerActor player;
		ILibraryActor libraryActor;

        public PlayerController()
        {
            player = DependencyActor.Container.Resolve<IPlayerActor>();
			libraryActor = DependencyActor.Container.Resolve<ILibraryActor>();
        }

        [HttpGet]
        [Route("")]
        [Route("Status")]
        public IActionResult Status()
        {
            return Ok();
        }

        [HttpPost]
        [Route("Request")]
        public IActionResult RequestAction([FromBody]PlayerRequest playerRequest)
        {
			dynamic response = null;
            switch (playerRequest.RequestType)
            {
                case RequestType.Play:
					if (player.PlaybackState == PlaybackState.Stopped)
					{
						var success = player.Play(playerRequest.Identifier);
						if (success)
							response = new { Status = success, Message = $"Started: {libraryActor.GetItem(playerRequest.Identifier).Name}" };
						else
							response = new { Status = success, Message = "Unknown Error" };
					}
					else
						response = new { Status = false, Message = $"Invalid State!", State = player.PlaybackState.ToString() };
                    break;
                case RequestType.Pause:
                    break;
                case RequestType.Stop:
					if (player.PlaybackState == PlaybackState.Playing || player.PlaybackState == PlaybackState.Paused)
					{
						var success = player.Stop();
						if (success)
						{
							//Todo: Add the capability to say which song was stopped
							response = new { Status = success, Message = "Playback stopped!" };
						}
						else
							response = new { Status = success, Message = "Unknown Error" }; //Todo: add ability to provide error message as well
					}
					else
						response = new { Status = false, Message = $"Invalid State", State = player.PlaybackState.ToString() };
                    break;
                case RequestType.Enqueue:
                    break;
                case RequestType.DeQueue:
                    break;
                default:
					return BadRequest(new { Status = false, Message = "Unknown RequestType" });
            }
            return Ok(response);
        }

		[HttpPost]
		[Route("Control")]
		public IActionResult PlayerControlAction([FromBody]PlayerControlRequest playerControlRequest)
		{
			dynamic response = null;
            switch (playerControlRequest.ControlRequest)
			{
				case ControlRequest.Volume:
					var volume = (byte)playerControlRequest.RequestData;
					if (player.Volume(volume))
						response = new { Status = true, Message = $"Set Volume: {volume} Units: {(float)volume / 100f}" };
					else
						response = new { Status = false, Message = $"Unknown Error!" };
					break;
				case ControlRequest.Repeat:
					break;
				case ControlRequest.Random:
				default:
					break;
			}
			return Ok(response);
		}
    }
}
