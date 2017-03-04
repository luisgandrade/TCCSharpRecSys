using System;

namespace UnsupervisedLearning.UserProfiles.Decay
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
      return relativeAge.Days != 0 ? Math.Exp(-(7) / relativeAge.Days) : 1;
    }
  }
}
