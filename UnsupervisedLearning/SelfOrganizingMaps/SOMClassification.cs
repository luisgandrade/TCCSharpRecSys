using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning.SelfOrganizingMaps.Network;
using Utils.Metric;

namespace UnsupervisedLearning.SelfOrganizingMaps
{
  public class SOMClassification
  {

    public Network.Network network { get; private set; }

    private bool normalized_values;

    public int rows
    {
      get
      {
        return network.rows;
      }
    }

    public int columns
    {
      get
      {
        return network.columns;
      }
    }

    public int attr_count
    {
      get
      {
        return network.attr_count;
      }
    }

    public int instance
    {
      get
      {
        return network.instance;
      }
    }

    public SOMClassification(int rows, int columns, int attr_count, int instance, IMetric metric, IList<Tuple<int, int, List<double>>> neurons, bool normalizedValues)
    {
      if (neurons == null)
        throw new ArgumentException("neurons");
      if (rows <= 0 || columns <= 0 || attr_count <= 0 || instance <= 0)
        throw new InvalidOperationException("rows <= 0 || columns <= 0 || attr_count <= 0 || instance <= 0");
      if (neurons.Any(n => n.Item3.Count != attr_count))
        throw new InvalidOperationException("Pelo menos um dos nós tem lista de pesos diferente do número de atributos informados.");
      if (neurons.Count != rows * columns)
        throw new InvalidOperationException("Número de neurônios informados não bate com o número de linhas e colunas.");

      this.network = new Network.Network(rows, columns, attr_count, instance, metric, neurons);
      this.normalized_values = normalizedValues;
    }

    internal SOMClassification(Network.Network network, bool normalizedValues)
    {
      this.network = network;
      this.normalized_values = normalizedValues;
    }


    public IDictionary<IClassLabel<Neuron>, List<Movie>> groupMovies(IList<MovieSOMClassification> movieClassification, IList<Movie> movies)
    {
      if (movieClassification == null)
        throw new ArgumentException("movieClassification");

      var moviesLabeled = movies.Join(movieClassification, m => m, m1 => m1.movie, (m, m1) => new { movie = m, x = m1.neuron.coordinates.x, y = m1.neuron.coordinates.y });

      return network.neurons.SelectMany(n => n).GroupJoin(moviesLabeled, n => new { x = n.coordinates.x, y = n.coordinates.y }, mc => new { x = mc.x, y = mc.y },
          (n, mc) => new { neuron = n, movies = mc.Select(m => m.movie) }).ToDictionary(nmc => (IClassLabel<Neuron>)nmc.neuron, nmc => nmc.movies.ToList());
    }

    //public IList<InstanceClassification> classifyInstances(IList<KeyValuePair<int, List<double>>> instancesAttributes)
    //{

    //  return instancesAttributes.Select(ia => new
    //  {
    //    movie_id = ia.Key,
    //    node = network.classifyInstance(ia.Value)
    //  }).Select(ia => new InstanceClassification(ia.movie_id, ia.node.Item1, ia.node.Item2)).ToList();
    //}

    public IEnumerable<Neuron> classify(IList<double> instance_attributes)
    {
      return network.classifyInstance(instance_attributes);
    }
  }
}
