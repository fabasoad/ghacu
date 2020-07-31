using System;

namespace Ghacu.Runner.Cli.Progress
{
  public interface IProgressBar : IDisposable, IProgress<double>
  {
  }
}