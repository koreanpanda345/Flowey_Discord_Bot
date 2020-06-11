using System;

namespace Flowey.Bot
{
    class Program
    {
        static void Main(string[] args)
            => new Flowey().MainAsync().GetAwaiter().GetResult();
    }
}
