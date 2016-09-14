using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Metric;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class Metric
  {

    public IMetric metric { get; set; }


    public Metric(string name)
    {
      switch (name)
      {
        case "euclidian":
          metric = new EuclidianDistance();
          break;
        case "manhattan":
          metric = new ManhattanDistance();
          break;
        case "cosine":
          metric = new CosineSimilarity();
          break;
        default:
          throw new InvalidOperationException("Métrica informada desconhecida.");
      }
    }
  }
}
