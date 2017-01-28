using Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCCSharpRecSys.Persistence;
using UnsupervisedLearning;
using UnsupervisedLearning.KMeans;
using UnsupervisedLearning.SelfOrganizingMaps;
using UnsupervisedLearning.SelfOrganizingMaps.LearningRateFunctions;
using UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions;
using UnsupervisedLearning.SelfOrganizingMaps.Network;
using Utils;
using Utils.Metric;

namespace TCCSharpRecSys
{
  class Program
  {
    static void Main(string[] args)
    {

      //FileReader.setDirPath("C:\\Users\\luis\\Documents\\Projetos\\tccsharprecsys\\Data\\");
      //FileWritter.setDirPath("C:\\Users\\luis\\Documents\\Projetos\\tccsharprecsys\\Data\\");
      //var fileReader = FileReader.getInstance();
      //var fileWritter = FileWritter.getInstance();


      //var tags = fileReader.readTags();
      //var readSomConfig = fileReader.readAlgorithmConfig("som", "15_15_cosine_gaussian_15_369.269373068855_lr_0.11000t", 1);

      //var som = new SelfOrganizingMap(15, 15, 1127, new GaussianNeighborhood(15), new LearningRateFunction(), new CosineSimilarity(), true);
      //som.parse_classifier(readSomConfig);

      //var x = new List<Tuple<int, int, string>>();

      //for (int i = 0; i < 15; i++)
      //{
      //  for (int j = 0; j < 15; j++)
      //  {
      //    var neuron = som.getNeuron(i, j);
      //    var attrsComPesos = neuron.weights.Select((w, index) => new { attr = index, peso = w }).OrderByDescending(w => w.peso)
      //                                      .Join(tags, w => w.attr, t => t.id, (w, t) => t.tag);
      //    x.Add(new Tuple<int, int, string>(i, j, string.Join(", ", attrsComPesos.Take(5))));
      //  }
      //}

      //var a = 1;




      //var resultsAgg = new ResultsAggregator();
      //foreach (var cutoff in new[] { .3m, .5m, .8m })
      //{
      //  resultsAgg.usersWithQuantityOfRatings(cutoff);
      //}
      //resultsAgg.simpleAggregate("som");
      if (args.Count() == 0)
        throw new InvalidOperationException("Args faltando.");

      //try
      //{
      var cli = new CommandLineParser();
      var act = cli.parseCommands(File.ReadAllText(args[0]));


      ////var tagRelevances = FileReader.getInstance().readTagRelevances();
      ////var tagRelevancesGrouped = tagRelevances.GroupBy(t => t.tag);
      ////var tagsBoundariesValues = tagRelevancesGrouped.Select(t => new { tag = t.Key, min = t.Min(t1 => t1.relevance), max = t.Max(t1 => t1.relevance) });
      ////var t2 = tagRelevances.Join(tagsBoundariesValues, tr => tr.tag, tbv => tbv.tag, (tr, tbv) => new { tagRel = tr, normalizedValue = (tr.relevance - tbv.min) / (tbv.max - tbv.min) })
      ////                      .ToList();
      ////foreach (var t in t2)
      ////  t.tagRel.normalized_relevance = t.normalizedValue;
      ////FileWritter.getInstance().writeTagRel(t2.Select(t => t.tagRel).ToList());


      foreach (var a in act)
      {
        a.Invoke();
      }

      Console.ReadKey();


      //var fileReader = FileReader.getInstance();
      //var movieClassification = fileReader.readMovieClassification(1128, 20, 20, 2);
      //Console.WriteLine("Lendo filmes");
      //var movies = fileReader.readMovies();
      //Console.WriteLine("Agrupando filmes");

      //Console.WriteLine("Construindo SOM Class");
      //var somNodes = fileReader.existingNeurons(1128, 20, 20, 6);
      //var som = new SOMClassification(20, 20, 1128, 2, new EuclidianDistance(), somNodes, true);
      //var classificationsGrouped = som.groupMovies(movieClassification, movies);

      //var recommender = new Recommender<Neuron>(som, classificationsGrouped, 50);
      //Console.WriteLine("Construindo perfis de usuários.");
      //buildUserProfiles(args[0]);
      //Console.WriteLine("Reading movies...");

      //var instances = fileReader.readTagRelevances().GroupBy(tr => tr.movie).Select(tr => new Instance(tr.ToList()));

      //var kMeans = new StandardKMeans(instances.Take(100).ToList());
      //kMeans.cluster(instances.ToList());
      //var fileWritter = new FileWritter(args[0]);
      //fileWritter.writeClusters(kMeans.clusters);
      //var attr_counts = createAttributes(movies);
      //buildUserProfiles();
      //trainSOM();
      //classify();
      //labelNodes();

      //}
      //catch (Exception e)
      //{
      //  Console.WriteLine(e.InnerException);
      //  Console.WriteLine(e.Message);
      //  Console.WriteLine(e.StackTrace);
      //  Console.ReadKey();
      //}
    }
  }
}

