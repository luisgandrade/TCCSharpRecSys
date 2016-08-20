using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning.SelfOrganizingMaps.Network;

namespace UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions
{
  /// <summary>
  /// Implementação da função de vizinhança usando uma função Gaussiana. Essa função começa com uma largura inicial próxima ao raio
  /// da grade bidimensional de neurônios e vai se tornando mais concentrada com o tempo. A função aplicada é a seguinte:
  /// h(j,i) = exp(d2(j,i) / (2 * sigma^2(n))), onde
  /// - d2(j,i) é a disância euclidiana entre os neurônios j e i ao quadrado;
  /// - sigma^2(n) é uma função com decai exponencial conforme n aumenta
  /// </summary>
  public class GaussianNeighborhood : INeighborhoodFunction
  {

    private double initial_neighborhood_width;
    private double time_constant;    
    
    public double apply(Neuron bmu, Neuron neighbor, int iteration)
    {
      var neighborhoodWidthSquared = Math.Pow(initial_neighborhood_width * Math.Exp(-(iteration / time_constant)), 2);
      return Math.Exp(-(Math.Pow(bmu.distance(neighbor), 2) / (2 * neighborhoodWidthSquared)));      
    }
    
    public GaussianNeighborhood(double initialNeighborhoodWidth)
    {
      if (initialNeighborhoodWidth <= 0)
        throw new InvalidOperationException("Largura inicial da vizinhança deve ser maior que zero.");

      this.initial_neighborhood_width = initialNeighborhoodWidth;
      this.time_constant = 1000/Math.Log(initialNeighborhoodWidth);
    }
  }
}
