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

      if (args.Count() == 0)
        throw new InvalidOperationException("Args faltando.");

      try
      {

        var cli = new CommandLineParser();
        var act = cli.parseCommands(File.ReadAllText(args[0]));

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

      }
      catch (Exception e)
      {
        Console.WriteLine(e.InnerException);
        Console.WriteLine(e.Message);
        Console.WriteLine(e.StackTrace);
        Console.ReadKey();
      }
    }
    

    //static void buildUserProfiles(string filePath)
    //{
    //  var fileReader = FileReader.getInstance();
    //  var fileWritter = new FileWritter(filePath);
      
    //  var ratings = fileReader.readUserRatings();
      
    //  var tagRelevance = fileReader.readTagRelevances();
    //  var userProfileBuilder = new UserProfileBuilder();
    //  var userProfiles = new List<UserProfile>();
    //  Console.WriteLine("Construindo perfis...");
    //  var user = 1;

    //  foreach (var userRatings in ratingsByUser)
    //  {
    //    Console.WriteLine(user);
    //    var trainingRatings = userRatings.ratings.Take((int)(.8 * userRatings.ratings.Count));
    //    var evaluationRatings = userRatings.ratings.Skip((int)(.8 * userRatings.ratings.Count));
    //    var userProfile = userProfileBuilder.buildUserProfile(trainingRatings.ToList(), tagRelevance, trainingRatings.Max(rat => rat.timestamp).AddDays(1));
    //    userProfiles.Add(userProfile);

    //    //var moviesRecommended = recommender.recommend(userProfile.profile, trainingRatings.Select(tr => tr.movie).ToList());
    //    //var matches = evaluationRatings.Select(er => er).Join(moviesRecommended, m => m.movie, cm => cm, (m, cm) => m);
    //    //recomendacoesPorUsuario.Add(new Tuple<int, int, int>(userProfile.Key, matches.Count(), evaluationRatings.Count()));
    //    Console.WriteLine("User: " + user);
    //    user++;
    //    if(userProfiles.Count == 20000)
    //    {
    //      fileWritter.appendUserProfiles(userProfiles);
    //      Console.WriteLine("Perfis escritos");
    //      userProfiles.Clear();
    //    }
    //  }

    //}

    //static void labelNodes()
    //{
    //  var fileReader = FileReader.getInstance();
    //  var fileWritter = new FileWritter();

    //  var tags = fileReader.readTags();
    //  foreach (var dimensions in new[] { 15, 20 })
    //  {
    //    for (int i = 1; i <= 3; i++)
    //    {
    //      Console.WriteLine("Rotulando redes. dimensions: " + dimensions + "; instance: " + i);
    //      var neurons = fileReader.existingNeurons(tags.Count, dimensions, dimensions, i);
    //      var som = new SOMClassification(dimensions, dimensions, tags.Count, i, neurons, true);
    //      //var labelNodes = som.bestFitAttributes(tags.Select(attr => new KeyValuePair<Tag, double>(attr, (1 - attr.population_average) / attr.population_standard_deviation)).ToList(), 10);
    //      //fileWritter.writeSOMLabels(som, labelNodes);
    //    }
    //  }     

    //}

    //static void classify()
    //{
    //  var fileReader = new FileReader();
    //  var fileWritter = new FileWritter();

    //  var moviesAttributes = fileReader.readTagRelevances();
    //  var attr_count = moviesAttributes.Select(ma => ma.tag_id).Distinct().Count();
    //  foreach (var dimensions in new[] { 10, 20, 30 })
    //  {
    //    for (int i = 1; i <= 3; i++)
    //    {
    //      Console.WriteLine("Classificando. dimensions: " + dimensions + "; instance: " + i);
    //      var neurons = fileReader.existingNeurons(attr_count, dimensions, dimensions, i);
    //      var som = new SOMClassification(dimensions, dimensions, attr_count, i, neurons, true);
    //      var moviesClassification = som.classifyInstances(
    //                                      moviesAttributes.GroupBy(ma => ma.movie_id)
    //                                                      .Select(ma => new KeyValuePair<int, List<double>>(ma.Key, ma.OrderBy(m => m.tag_id).Select(tr => tr.relevance).ToList()))
    //                                                      .ToList());
    //      fileWritter.writeSOMClassification(som, moviesClassification);
    //    }
    //  }

    //}

    //static void trainSOM(string filePath)
    //{
    //  var fileReader = FileReader.getInstance();
    //  var fileWritter = new FileWritter(filePath);

    //  foreach (var dimensions in new[] { 10, 20, 30 })
    //  {
    //    for (int i = 0; i < 3; i++)
    //    {
    //      Console.WriteLine("Criando SOM. dimensions: " + dimensions + "; instance: " + i);
    //      var tagRelevances = fileReader.readTagRelevances()
    //                                    .GroupBy(ma => ma.movie)
    //                                    .Select(ma => ma.OrderBy(m => m.tag.id).ToList())
    //                                    .ToList();
    //      Console.WriteLine(tagRelevances.Select(tr => tr.Count).Distinct().Count());
    //      var som = new SelfOrganizingMap(dimensions, dimensions, tagRelevances[0].Count, new GaussianNeighborhood((int)((double)dimensions / 2)), new LearningRateFunction(),
    //        new EuclidianDistance(), false);
    //      fileWritter.writeSOMNodes(som);
    //      var stop = false;
    //      var iteration = 0;

    //      while (!stop)
    //      {
    //        if (iteration % tagRelevances.Count == 0)
    //          tagRelevances.Shuffle();
    //        stop = som.iterate(tagRelevances[iteration % tagRelevances.Count], iteration);
    //        iteration++;
    //      }

    //      fileWritter.writeSOMNodes(som);
    //    }
    //  }
 
    //}

  //  static IList<int> createAttributes(IList<Movie> movies)
  //  {
  //    var attributeExtractor = new AttributeExtractor();
  //    var fileWriter = new FileWritter();

  //    var attr_counts = new List<int>();

  //    foreach (var percentage in new[] { .05m, .01m, .005m })
  //    {
  //      Console.WriteLine("Choosing attributes...");
  //      var attributes = attributeExtractor.chooseAttributesFromMovies(movies, percentage);
  //      Console.WriteLine("Extracting attributes...");
  //      var movieAttributes = attributeExtractor.extractAttributesFromMovies(movies, attributes, true);

  //      Console.WriteLine("Writing attributes...");
  //      fileWriter.writeAttributes(attributes);
  //      Console.WriteLine("Writing movies attributes...");
  //      fileWriter.writeMovieAttributes(movieAttributes, attributes.Count);
  //      attr_counts.Add(attributes.Count);
  //    }
  //    return attr_counts;
  //  }
  }
}
