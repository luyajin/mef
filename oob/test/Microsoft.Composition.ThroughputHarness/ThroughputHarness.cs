﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CompositionThroughput
{
    static class ThroughputHarness
    {
        public static long MeasureOperationsPerSecond(Action operation, int operations, bool isParallel)
        {
            GC.Collect();

            var cores = Enumerable.Range(0, System.Environment.ProcessorCount);
            var opsPerCore = operations / System.Environment.ProcessorCount;

            try
            {
                var sw = new Stopwatch();
                sw.Start();

                if (isParallel)
                {
                    cores.AsParallel().ForAll(core =>
                    {
                        for (var j = 0; j < opsPerCore; ++j)
                            operation();
                    });
                }
                else
                {
                    for (var op = 0; op < operations; ++op)
                        operation();
                }

                sw.Stop();
                return (long)((decimal)operations * Stopwatch.Frequency / sw.ElapsedTicks);
            }
            catch
            {
                return 0;
            }
        }
    }

}
