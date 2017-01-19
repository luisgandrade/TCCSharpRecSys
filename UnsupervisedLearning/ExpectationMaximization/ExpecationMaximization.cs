using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;
using UnsupervisedLearning.KMeans;
using UnsupervisedLearning.GaussianMixtureModel;

namespace UnsupervisedLearning.ExpectationMaximization
{
  public class ExpecationMaximization : IUnsupervisedLearning
  {
    private IList<GaussianDistribution> gaussian_distributions;

    public string file_prefix
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string name
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string sub_dir
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public IEnumerable<IClassLabel> best_matching_units(UserProfile userProfile)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IMovieClassification> classify_instances(IList<Instance> instances)
    {
      throw new NotImplementedException();
    }

    public void parse_classifier(IList<string> classLabelConfig)
    {
      throw new NotImplementedException();
    }

    public Func<Movie, string, IMovieClassification> parse_movie_classification()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<string> printClassifier()
    {
      throw new NotImplementedException();
    }

    public void train(IList<Instance> instances)
    {
      throw new NotImplementedException();
    }
  }
}
