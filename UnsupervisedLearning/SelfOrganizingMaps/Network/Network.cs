using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning.SelfOrganizingMaps.LearningRateFunctions;
using UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions;
using Utils;
using Utils.Metric;

namespace UnsupervisedLearning.SelfOrganizingMaps.Network
{
  /// <summary>
  /// Representa uma rede bidimensional de Kohonen.
  /// </summary>
  public class Network
  {
    /// <summary>
    /// Número de linhas da rede
    /// </summary>
    internal int rows { get; private set; }
    /// <summary>
    /// Número de colunas da rede.
    /// </summary>
    internal int columns { get; private set; }
    /// <summary>
    /// Número de atributos dessa rede.
    /// </summary>
    internal int attr_count { get; private set; }
    /// <summary>
    /// Instância dessa rede. (somente para controle manutenção de nome dos arquivos gerados)
    /// </summary>
    internal int instance { get; private set; }
    /// <summary>
    /// Grade bidimensional de neurônios.
    /// </summary>
    internal Neuron[][] neurons { get; private set; }

    private IMetric metric;
    
    internal Network(int rows, int columns, int attr_count, IMetric metric)      
    {
      if (rows <= 0 || columns <= 0 || attr_count <= 0)
        throw new InvalidOperationException("rows <= 0 || columns <= 0 || attr_count <= 0");

      this.rows = rows;
      this.columns = columns;
      this.attr_count = attr_count;
      this.metric = metric;
      neurons = new Neuron[rows][];
      for (int i = 0; i < rows; i++)
        neurons[i] = new Neuron[columns];

      var random = new Random(DateTime.Now.Millisecond);
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          var initialWeights = Enumerable.Range(1, attr_count).Select(r => random.NextDouble());
          neurons[i][j] = new Neuron(i, j, initialWeights.ToList());
        }
      }
    }

    internal Network(int rows, int columns, int attr_count, int instance, IMetric metric, IEnumerable<Tuple<int, int, List<double>>> existingNeurons)      
    {
      this.rows = rows;
      this.columns = columns;
      this.attr_count = attr_count;
      this.metric = metric;
      neurons = new Neuron[rows][];
      for (int i = 0; i < rows; i++)
        neurons[i] = new Neuron[columns];

      this.instance = instance;
      foreach (var existingNeuron in existingNeurons)
      {
        if (existingNeuron.Item1 > rows - 1 || existingNeuron.Item1 < 0 || existingNeuron.Item2 > columns - 1 || existingNeuron.Item2 < 0)
          throw new IndexOutOfRangeException("Coordenadas fora do intervalo. rows: " + rows + ", columns: " + columns + ", x: " + existingNeuron.Item1 + ", y: " + existingNeuron.Item2);
        if (existingNeuron.Item3.Count != attr_count)
          throw new InvalidOperationException("Tamanho do vetor de pesos != attr_count");
        neurons[existingNeuron.Item1][existingNeuron.Item2] = new Neuron(existingNeuron.Item1, existingNeuron.Item2, existingNeuron.Item3);
      }
    }

    internal Network(Network network)      
    {
      this.rows = network.rows;
      this.columns = network.columns;
      this.attr_count = network.attr_count;
      this.metric = network.metric;
      neurons = new Neuron[rows][];
      for (int i = 0; i < rows; i++)
        neurons[i] = new Neuron[columns];

      this.instance = network.instance;
      for (int i = 0; i < rows; i++)
        for (int j = 0; j < columns; j++)
          neurons[i][j] = new Neuron(network.neurons[i][j].coordinates.x, network.neurons[i][j].coordinates.y, network.neurons[i][j].weights);      
    }

    /// <summary>
    /// Encontra a best matching unit (BMU). A BMU é o neurônio cujos pesos estão mais próximos dos valores de atributos
    /// da instancia <paramref name="instanceAttributes"/> apresentada à rede. Essa distância é calculada usando a distância
    /// euclidiana
    /// </summary>
    /// <param name="instanceAttributes">valores dos atributos da instância atual apresentada à rede</param>
    /// <returns>o neurônio mais próximo a instância apresentada em termos de distância euclidiana</returns>
    internal Neuron getBMU(IList<double> instanceAttributes)
    {
      double minDistance = double.MaxValue;
      Neuron bmu = null;

      foreach (var neuronArray in neurons)
      {
        foreach (var neuron in neuronArray)
        {
          var distance = metric.applyMetric(neuron.weights, instanceAttributes);
          if (distance <= minDistance)
          {
            minDistance = distance;
            bmu = neuron;
          }
        }
      }

      return bmu;
    }

    /// <summary>
    /// Atualiza os neurônios da rede conforme os parâmetros informados.
    /// </summary>
    /// <param name="bmu">o neurônio que mais se aproxima da última instância apresentada à rede</param>
    /// <param name="neighborhoodFunction">função que acrescenta um fator atenuador na atualização de um neurônio de acordo com sua distância do BMU e da iteração atual</param>
    /// <param name="learningRateFunction">função que acrescenta um fator atenuador na atualização de um neurônio de acordo com a taxa de aprendizagem</param>
    /// <param name="instanceAttributes">instância apresentada a rede</param>
    /// <param name="iteration">iteração atual</param>
    internal void updateNetwork(Neuron bmu, INeighborhoodFunction neighborhoodFunction, ILearningRateFunction learningRateFunction, IList<double> instanceAttributes, int iteration)
    {      
      for (int i = 0; i < neurons.Length; i++)
        for (int j = 0; j < neurons[i].Length; j++)
          neurons[i][j].updateNeuron(instanceAttributes, neighborhoodFunction.apply(bmu, neurons[i][j], iteration), learningRateFunction.apply(iteration));
    }

    internal IEnumerable<Neuron> classifyInstance(IList<double> movieAttributes)
    {
      if (movieAttributes == null)
        throw new ArgumentException("movieAttributes");

      return neurons.SelectMany(n1 => n1.Select(n2 => new { distance = metric.applyMetric(n2.weights, movieAttributes), neuron = n2 }))
                    .OrderBy(n => n.distance)
                    .Select(n => n.neuron);
    }

    /// <summary>
    /// Retorna um enumerável com as linhas a serem impressas no arquivo, onde cada linha representa um neurônio na rede.
    /// </summary>
    /// <returns></returns>
    internal IEnumerable<string> printNetwork()
    {
      foreach (var neuronArray in neurons)
        foreach (var neuron in neuronArray)
          yield return neuron.ToString();
    }   

  }
}
