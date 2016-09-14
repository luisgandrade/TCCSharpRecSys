using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class Neighborhood
  {    
    public INeighborhoodFunction neighborhood_function { get; set; }
    
    public Neighborhood(string identifier, double initial_width, double time_constant)
    {
      switch (identifier)
      {
        case "gaussian":
          neighborhood_function = new GaussianNeighborhood(time_constant, initial_width);
          break;
        default:
          throw new InvalidOperationException("Identificador de função de vizinhança desconhecido");
      }
    }

    public Neighborhood(string identifier, double initial_width)
    {
      switch (identifier)
      {
        case "gaussian":
          neighborhood_function = new GaussianNeighborhood(initial_width);
          break;
        default:
          throw new InvalidOperationException("Identificador de função de vizinhança desconhecido");
      }
    }
  }
}
