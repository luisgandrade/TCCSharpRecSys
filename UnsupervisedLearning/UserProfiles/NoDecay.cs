using System;

namespace UnsupervisedLearning.UserProfiles
{
  public class NoDecay : IDecayFormula
  {
    public string decay_display
    {
      get
      {
        return "constant";
      }
    }

    public double decay(TimeSpan relativeAge)
    {
      return 1;
    }
  }
}
