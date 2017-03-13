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

    public double min { get; private set; }

    public double max { get; private set; }        

    public double average { get; private set; }

    public double std_dev { get; private set; }

    

    public AggregatedResults(string alg_config, double min, double max, double average, double std_dev)
    {
      this.alg_config = alg_config;
      this.average = average;
      this.std_dev = std_dev;
      this.min = min;
      this.max = max;
    }
  }
}
