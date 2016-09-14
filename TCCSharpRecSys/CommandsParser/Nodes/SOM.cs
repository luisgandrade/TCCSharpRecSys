using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;
using UnsupervisedLearning.SelfOrganizingMaps;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class SOM : IAlgorithm
  {
    public int rows { get; set; }

    public int columns { get; set; }

    public Metric metric { get; set; }

    public Neighborhood neighborhood_function { get; set; }

    public LRate learning_rate_config { get; set; }

    public bool use_normalized_values { get; set; }

    public SOM(int rows, int columns, Metric metric, Neighborhood neighborhood_function,  LRate learning_rate_config, bool use_normalized_values)
    {
      if (neighborhood_function == null)
        throw new ArgumentException("neighborhood_function");
      if (metric == null)
        throw new ArgumentException("metric");
      if (learning_rate_config == null)
        throw new ArgumentException("learning_rate_config");

      this.rows = rows;
      this.columns = columns;
      this.neighborhood_function = neighborhood_function;
      this.metric = metric;
      this.learning_rate_config = learning_rate_config;
      this.use_normalized_values = use_normalized_values;
    }

    public IUnsupervisedLearning getAlgorithm(int attr_count)
    {
      return new SelfOrganizingMap(rows, columns, attr_count, neighborhood_function.neighborhood_function, learning_rate_config.learning_rate_function, metric.metric, use_normalized_values);
    }
    
  }
}
