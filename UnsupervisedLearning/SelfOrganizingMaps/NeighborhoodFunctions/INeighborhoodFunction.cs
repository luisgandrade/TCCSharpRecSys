using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning.SelfOrganizingMaps.Network;

namespace UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions
{
  /// <summary>
  /// Disponibiliza uma interface para implementações de funções de vizinhança. O valor da função de vizinhança deve decair com o tempo
  /// </summary>
  public interface INeighborhoodFunction
  {    
    /// <summary>
    /// Aplica a função de vizinhança aos neurônios <paramref name="bmu"/> and <paramref name="neighbor"/> da rede na iteração <paramref name="iteration"/>
    /// </summary>
    /// <returns>distancia entre neurônios <paramref name="bmu"/> e <paramref name="neighbor"/></returns>
    double apply(Neuron bmu, Neuron neighbor, int iteration);

  }
}
