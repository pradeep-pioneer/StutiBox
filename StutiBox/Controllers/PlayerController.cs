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
			var response = new
			{
                Status = true,
                TotalLibraryItems = libraryActor.LibraryItems.Count,
                LibraryRefreshedAt = libraryActor.RefreshedAt,
                PlayerState = player.PlaybackState.ToString(),
                player.CurrentLibraryItem,
				BassState = player.BassActor.State.ToString(),
                Volume = player.BassActor.CurrentVolume,
                player.BassActor.CurrentPositionBytes,
                player.BassActor.CurrentPositionSeconds,
                player.BassActor.CurrentPositionString,
                player.BassActor.Repeat
			};
            return Ok(response);
        }

        [HttpGet]
        [Route("ConversationStarted")]
        public IActionResult ConversationStarted()
		{
			var respose = new
			{
				Status = player.ConversationStarted(),
				Volume = player.BassActor.CurrentVolume
			};
			return Ok(respose);
		}

		[HttpGet]
        [Route("ConversationFinished")]
        public IActionResult ConversationFinished()
        {
            var respose = new
            {
                Status = player.ConversationFinished(),
                Volume = player.BassActor.CurrentVolume
            };
            return Ok(respose);
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
							response = new { Status = success, Message = $"Started!", MediaItem = player.CurrentLibraryItem };
						else
							response = new { Status = success, Message = "Unknown Error" };
					}
					else
						response = new { Status = false, Message = $"Invalid State!", State = player.PlaybackState.ToString() };
                    break;
                case RequestType.Pause:
					if(player.PlaybackState == PlaybackState.Playing)
					{
						var success = player.Pause();
                        if (success)
							response = new { Status = success, Message = $"Playback Paused!", MediaItem = player.CurrentLibraryItem };
                        else
                            response = new { Status = success, Message = "Unknown Error" };
					}
					else
                        response = new { Status = false, Message = $"Invalid State!", State = player.PlaybackState.ToString() };
                    break;
				case RequestType.Resume:
					if (player.PlaybackState == PlaybackState.Paused)
                    {
                        var success = player.Resume();
                        if (success)
							response = new { Status = success, Message = $"Playback Resuming!", MediaItem = player.CurrentLibraryItem };
                        else
                            response = new { Status = success, Message = "Unknown Error" };
                    }
					else
                        response = new { Status = false, Message = $"Invalid State!", State = player.PlaybackState.ToString() };
                    break;
                case RequestType.Stop:
					if (player.PlaybackState == PlaybackState.Playing || player.PlaybackState == PlaybackState.Paused)
					{
						var currentItem = player.CurrentLibraryItem;
						var success = player.Stop();
						if (success)
						{
							//Todo: Add the capability to say which song was stopped
							response = new { Status = success, Message = "Playback stopped!", MediaItem = currentItem };
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
				case ControlRequest.VolumeAbsolute:
					var volume = (byte)playerControlRequest.RequestData;
					if (player.Volume(volume))
						response = new { Status = true, Message = $"Set Volume!", Values = new {Scale1 = player.BassActor.CurrentVolume, Scale2 = (float)player.BassActor.CurrentVolume / 100f} };
					else
						response = new { Status = false, Message = $"Unknown Error!" };
					break;
				case ControlRequest.RepeatToggle:
					var result = player.ToggleRepeat();
                    response = new
                    {
                        Status = result,
                        Message = result ? $"Success! Repeat: {player.BassActor.Repeat}" : $"Failed! {player.PlaybackState.ToString()}",
						player.BassActor.Repeat
                    };
                    break;
				case ControlRequest.Seek:
					result = player.Seek(playerControlRequest.RequestData);
					response = new
                    {
                        Status = result,
						Message = result ? $"Success! Seek: {playerControlRequest.RequestData}" : $"Failed to seek to: {playerControlRequest.RequestData.ToString()}!",
						player.BassActor.CurrentPositionBytes,
                        player.BassActor.CurrentPositionSeconds,
                        player.BassActor.CurrentPositionString
                    };
					break;
				case ControlRequest.VolumeRelative:
					var volumeStep = (byte)playerControlRequest.RequestData;
					var oldVolume = player.BassActor.CurrentVolume;
					var newVolume = (byte)(oldVolume + volumeStep);
					if (player.Volume(newVolume))
						response = new { Status = true, Message = $"Set Volume!", Values = new { Scale1 = player.BassActor.CurrentVolume, Scale2 = (float)player.BassActor.CurrentVolume / 100f } };
                    else
                        response = new { Status = false, Message = $"Unknown Error!" };
                    break;
				case ControlRequest.Random:
				default:
					break;
			}
			return Ok(response);
		}
    }
}
