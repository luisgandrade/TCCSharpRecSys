using System;

namespace TCCSharpRecSys
{
  public interface IDecayFormula
  {
    double decay(TimeSpan relativeAge);

    string decay_display { get; }
  }
}
