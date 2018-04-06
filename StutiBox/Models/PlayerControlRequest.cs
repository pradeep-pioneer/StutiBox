using System;
namespace StutiBox.Models
{
	public class PlayerControlRequest
	{

		public ControlRequest ControlRequest { get; set; }
		public dynamic RequestData { get; set; }
	}

	public enum ControlRequest
	{
		Volume = 1,
		Repeat = 2,
		Random = 3,
        Seek = 4
	}
}
