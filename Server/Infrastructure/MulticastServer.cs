using System;
using System.Net;
using Server.Generator;

namespace Server.Infrastructure
{
    internal sealed class MulticastServer : BaseMulticastServer
    {
        private readonly IValueGenerator _generator;

        public MulticastServer(IValueGenerator generator, IPEndPoint endpoint, int bufferCapacity) :base(endpoint,bufferCapacity)
        {
            _generator = generator ?? throw new ArgumentNullException("Не задан аргумент IValueGenerator generator");
        }

        public override int GetData()
        {
            return _generator.Next();
        }
    }
}
