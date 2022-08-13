using System;

namespace STT_CLI
{
    class Program
    {
        static STT stt;

        static void Main(string[] args)
        {
            stt = new STT(args[0]);
        }
    }
}
