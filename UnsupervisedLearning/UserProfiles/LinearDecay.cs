using System;

namespace TCCSharpRecSys
{
  public class LinearDecay : IDecayFormula
  {
    public string decay_display
    {
      get
      {
        return "linear";
      }
    }

    public double decay(TimeSpan relativeAge)
    {
      return relativeAge != TimeSpan.Zero ? 1 - 7 * 24 * 3600 / relativeAge.TotalSeconds : 1;
    }
  }
}
