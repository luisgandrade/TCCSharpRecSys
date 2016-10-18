using System;
using System.Collections.Generic;
using System.Text;
using Utils.Metric;

namespace UnsupervisedLearning.SelfOrganizingMaps.Network
{
  /// <summary>
  /// Unidade de agrupamento do algoritmo de mapas auto-organizáveis.
  /// </summary>
  public class Neuron : IClassLabel
  {
    internal int x { get; private set; }

    internal int y { get; private set; }
    /// <summary>
    /// Pesos nas entradas desse neurônio. 
    /// </summary>
    internal IList<double> weights { get; private set; }
    
    internal Neuron(int x, int y, IList<double> weights)
    {
      if (weights == null)
        throw new ArgumentException("weights");
            
      this.weights = weights;
      this.x = x;
      this.y = y;
    }

    /// <summary>
    /// Atualiza os valores desse neurônio conforme a equação abaixo:
    /// w(i + 1) = w(i) + h * n * (x - w(i))
    /// onde:
    /// - w(i + 1) é a lista de pesos atualizados do neurônio;
    /// - h é um fator atenuador calculado pela função de vizinhança;
    /// - n é um fator atenuador calculado pela função de taxa de aprendizado;
    /// - x é a instância que foi apresentada à rede;
    /// - w(i) é a lista de pesos atual do neurônio
    /// </summary>
    /// <param name="instanceAttributesValues">valores dos atributos da instância atual apresentada a rede</param>
    /// <param name="neighborhoodFactor">fator atenuador calculado pela função de vizinhança</param>
    /// <param name="learningRateFactor">fator atenuador calculado pela função de taxa de aprendizado</param>
    internal void updateNeuron(IList<double> instanceAttributesValues, double neighborhoodFactor, double learningRateFactor)
    {
      if (instanceAttributesValues == null)
        throw new ArgumentException("instanceAttributesValues");
      if (instanceAttributesValues.Count != weights.Count)
        throw new InvalidOperationException("instanceAttributesValues.Count != weights.Count");
      for (int i = 0; i < weights.Count; i++)
        weights[i] += neighborhoodFactor * learningRateFactor * (instanceAttributesValues[i] - weights[i]);
    }

    internal double distance(Neuron other)
    {
      return EuclidianDistance.distance(new List<double> { this.x, this.y }, new List<double> { other.x, other.y });
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("[" + weights[0]);
      for (int i = 1; i < weights.Count; i++)
        sb.Append(";" + weights[i]);
      sb.Append("]");
      return string.Format("[{0};{1}],{2}", x, y, sb.ToString());
    }

    public bool Equals(Neuron other)
    {
      if (other == null)
        return false;
      return this.x == other.x && this.y == other.y; 
    }
  }
}
