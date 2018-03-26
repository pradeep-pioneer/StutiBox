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
        public PlayerController()
        {
            player = DependencyActor.Container.Resolve<IPlayerActor>();
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
            switch (playerRequest.RequestType)
            {
                case RequestType.Play:
                    if (player.PlaybackState==PlaybackState.Stopped)
                        player.Play(playerRequest.Identifier);
                    break;
                case RequestType.Pause:
                    break;
                case RequestType.Stop:
                    player.Stop();
                    break;
                case RequestType.Enqueue:
                    break;
                case RequestType.DeQueue:
                    break;
                default:
                    return BadRequest();
            }
            return Ok();
        }
    }
}
