using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCCSharpRecSys.Persistence;

namespace TCCSharpRecSys
{
  public class AggregatedResults
  {

    public string alg_config { get; private set; }

    public double precision { get; private set; }

    public int count { get; private set; }

    public double average { get; private set; }

    public double std_dev { get; private set; }

    public double min { get; private set; }

    public double max { get; private set; }

    public AggregatedResults(string alg_config, double precision, int count, double average, double std_dev, double min, double max)
    {
      this.alg_config = alg_config;
      this.precision = precision;
      this.count = count;
      this.average = average;
      this.std_dev = std_dev;
      this.min = min;
      this.max = max;
    }
  }
}
