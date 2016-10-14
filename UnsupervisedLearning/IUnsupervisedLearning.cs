using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;
using UnsupervisedLearning.KMeans;

namespace UnsupervisedLearning
{
  public interface IUnsupervisedLearning
  {

    void train(IList<Instance> instances);

    IEnumerable<IMovieClassification> classify_instances(IList<Instance> instances);

    IEnumerable<IClassLabel> best_matching_units(UserProfile userProfile);

    IEnumerable<string> printClassifier();

    Func<Movie, string, IMovieClassification> parse_movie_classification();

    void parse_classifier(IList<string> classLabelConfig);

    string name { get; }

    string sub_dir { get; }

    string file_prefix { get; }
  }
}
