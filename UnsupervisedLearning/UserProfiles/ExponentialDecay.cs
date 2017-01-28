using System;

namespace UnsupervisedLearning.UserProfiles
{
  public class ExponentialDecay : IDecayFormula
  {
    public string decay_display
    {
      get
      {
        return "exponential";
      }
    }

    public double decay(TimeSpan relativeAge)
    {
      return relativeAge != TimeSpan.Zero ? Math.Exp(-(7 * 24 * 3600) / relativeAge.TotalSeconds) : 1;
    }
  }
}
