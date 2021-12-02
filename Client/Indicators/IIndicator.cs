using System.Collections.Generic;

namespace Client.Indicators
{
    internal interface IIndicator
    {
        IReadOnlyDictionary<string, double> Results { get; }
       

        void Calculate(int data);
        void Reset();
    }
}
