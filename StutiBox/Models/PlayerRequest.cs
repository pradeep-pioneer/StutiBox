using System;
namespace StutiBox.Models
{
    public class PlayerRequest
    {
        public RequestType RequestType { get; set; }
        public int Identifier { get; set; }
    }

    public enum RequestType
    {
        Play = 0,
        Pause = 1,
        Stop = 2,
        Enqueue = 3,
        DeQueue = 4
    }
}
