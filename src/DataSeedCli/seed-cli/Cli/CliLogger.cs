using System;
using System.Diagnostics;

namespace seed_cli.Cli
{
    internal sealed class ConsoleLogger
    {
        private int _stepNumber = 0;

        public void Info(string message)
        {
            // Always-verbose: no log levels / filtering in this slice.
            Console.WriteLine(message ?? string.Empty);
        }

        public IDisposable Step(string dbTarget, string action, out StepScope scope)
        {
            var stepNo = System.Threading.Interlocked.Increment(ref _stepNumber);
            scope = new StepScope(this, stepNo, dbTarget, action);
            scope.Start();
            return scope;
        }

        internal sealed class StepScope : IDisposable
        {
            private readonly ConsoleLogger _logger;
            private readonly int _stepNo;
            private readonly string _dbTarget;
            private readonly string _action;
            private readonly Stopwatch _sw = new Stopwatch();

            private bool _ended;
            private string _result = "FAIL"; // default FAIL unless Ok() called

            public StepScope(ConsoleLogger logger, int stepNo, string dbTarget, string action)
            {
                _logger = logger;
                _stepNo = stepNo;
                _dbTarget = string.IsNullOrWhiteSpace(dbTarget) ? "UNKNOWN" : dbTarget;
                _action = string.IsNullOrWhiteSpace(action) ? "UNKNOWN" : action;
            }

            public void Start()
            {
                _sw.Start();
                // start line is optional; requirement is "Each step logs ... OK/FAIL, elapsed ms."
            }

            public void Ok() => _result = "OK";
            public void Fail() => _result = "FAIL";

            public void Dispose()
            {
                if (_ended) return;
                _ended = true;

                _sw.Stop();
                if (_result != "OK")
                {
                    _logger.Info("");
                    _logger.Info("****************************************");
                }
                _logger.Info($"step={_stepNo} db={_dbTarget} action=\"{_action}\" result={_result} elapsedMs={_sw.ElapsedMilliseconds}");
                if (_result != "OK")
                {
                    _logger.Info("****************************************");
                }

            }
        }
    }
}

