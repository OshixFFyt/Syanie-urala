using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syanie_urala
{
    internal class PerformanceInfo
    {
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _memoryCounter;

        public PerformanceInfo()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memoryCounter = new PerformanceCounter("Memory", "Committed Bytes");
            float memoryLoad = _memoryCounter.NextValue() / 1024 / 1024; // convert to MB
        }

        public float GetCpuLoad()
        {
            return _cpuCounter.NextValue();
        }

        public float GetMemoryLoad()
        {
            return _memoryCounter.NextValue() / 1024 / 1024; // convert to MB
        }
    }
}
