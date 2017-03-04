using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Services
{
  public class ConfigStats
  {

    public IList<KeyValuePair<double, double>> histogram { get; private set; }

    public IList<KeyValuePair<double, double>> average { get; private set; }

    public IList<KeyValuePair<double, double>> std_dev { get; private set; }

    public IList<KeyValuePair<double, double>> min { get; private set; }

    public IList<KeyValuePair<double, double>> max { get; private set; }

    public ConfigStats(IList<KeyValuePair<double, double>> histogram, IList<KeyValuePair<double, double>> average, IList<KeyValuePair<double, double>> std_dev, 
      IList<KeyValuePair<double, double>> min, IList<KeyValuePair<double, double>> max)
    {
      this.histogram = histogram;
      this.average = average;
      this.std_dev = std_dev;
      this.min = min;
      this.max = max;
    }
  }
}