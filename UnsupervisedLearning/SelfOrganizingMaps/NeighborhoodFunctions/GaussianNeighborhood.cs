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
      if (bmu == null)
        throw new ArgumentException("bmu");
      if (neighbor == null)
        throw new ArgumentException("neighbor");
      var neighborhoodWidth = initial_neighborhood_width * Math.Exp(-(iteration / time_constant));
      //Se a largura na iteração for suficiente pequena a exponencial abaixo retornará um valor bem menor. Então paramos por aqui
      //para evitar que a atualização do quadrado da largura da vizinhança decaia ao ponto de causar underflow.
      if (neighborhoodWidth < .001)
        return bmu == neighbor ? 1 : 0;
      var neighborhoodWidthSquared = Math.Pow(neighborhoodWidth, 2);
      return Math.Exp(-(Math.Pow(bmu.distance(neighbor), 2) / (2 * neighborhoodWidthSquared)));      
    }
    
    public GaussianNeighborhood(double initialNeighborhoodWidth, double timeConstant)
    {
      if (initialNeighborhoodWidth <= 0)
        throw new InvalidOperationException("Largura inicial da vizinhança deve ser maior que zero.");

      this.initial_neighborhood_width = initialNeighborhoodWidth;
      this.time_constant = timeConstant/Math.Log(initialNeighborhoodWidth);
    }

    public GaussianNeighborhood(double initialWidth)
      : this(initialWidth, 1000) { }
  }
}
