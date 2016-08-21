using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using TCCSharpRecSys.Persistence;
using UnsupervisedLearning;
using UnsupervisedLearning.SelfOrganizingMaps;
using UnsupervisedLearning.SelfOrganizingMaps.LearningRateFunctions;
using UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions;
using Utils;

namespace TCCSharpRecSys
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        var fileReader = new FileReader();

        Console.WriteLine("Reading movies...");
        //var movies = fileReader.readMovies();
        //var attr_counts = createAttributes(movies);        
        trainSOM();
        classify();
        labelNodes();

      }
      catch (Exception e)
      {
        Console.WriteLine(e.StackTrace);
        Console.ReadKey();
      }
    }
    
    static void labelNodes()
    {
      var fileReader = new FileReader();
      var fileWritter = new FileWritter();

      foreach (var attr_count in new[] { 157 })
      {
        var attributes = fileReader.readAttributes(attr_count);
        foreach (var dimensions in new[] { 20, 25 })
        {
          for (int i = 1; i <= 3; i++)
          {
            Console.WriteLine("Rotulando redes. attr_count: " + attr_count + "; dimensions: " + dimensions + "; instance: " + i);
            var neurons = fileReader.existingNeurons(attr_count, dimensions, dimensions, i);
            var som = new SOMClassification(dimensions, dimensions, attr_count, i, neurons, true);
            var labelNodes = som.bestFitAttributes(attributes.Select(attr => new KeyValuePair<Attr, double>(attr, (1 - attr.population_average) / attr.population_standard_deviation)).ToList(), 10);
            fileWritter.writeSOMLabels(som, labelNodes);
          }
        }
      }

    }

    static void classify()
    {
      var fileReader = new FileReader();
      var fileWritter = new FileWritter();

      foreach (var attr_count in new[] { 157 })
      {
        var moviesAttributes = fileReader.readMoviesAttributes(attr_count);
        foreach (var dimensions in new[] { 20, 25 })
        {
          for (int i = 1; i <= 3; i++)
          {
            Console.WriteLine("Classificando. attr_count: " + attr_count + "; dimensions: " + dimensions + "; instance: " + i);
            var neurons = fileReader.existingNeurons(attr_count, dimensions, dimensions, i);
            var som = new SOMClassification(dimensions, dimensions, attr_count, i, neurons, true);
            var moviesClassification = som.classifyInstances(
                                            moviesAttributes.GroupBy(ma => ma.movie_id)
                                                            .Select(ma => new KeyValuePair<int, List<MovieAttribute>>(ma.Key, ma.OrderBy(m => m.attribute_id).ToList()))
                                                            .ToList());
            fileWritter.writeSOMClassification(som, moviesClassification);
          }
        }
      }
      

    }

    static void trainSOM()
    {
      var fileReader = new FileReader();
      var fileWritter = new FileWritter();

      foreach (var attr_count in new[] { 157 })
      {
        foreach (var dimensions in new[] { 20, 25 })
        {
          for (int i = 0; i < 3; i++)
          {
            Console.WriteLine("Criando SOM. attr_count: " + attr_count + "; dimensions: " + dimensions + "; instance: " + i);
            var movieAttributes = fileReader.readMoviesAttributes(attr_count)
                                          .GroupBy(ma => ma.movie_id)
                                          .Select(ma => ma.OrderBy(m => m.attribute_id).ToList())
                                          .ToList();

            var som = new SelfOrganizingMap(dimensions, dimensions, attr_count, new GaussianNeighborhood((int)((double)dimensions/2)), new LearningRateFunction(), true);
            fileWritter.writeSOMNodes(som);
            var stop = false;
            var iteration = 0;
            
            while (!stop)
            {
              if (iteration % movieAttributes.Count == 0)
                movieAttributes.Shuffle();
              stop = som.iterate(movieAttributes[iteration % movieAttributes.Count], iteration);
              iteration++;
            }

            fileWritter.writeSOMNodes(som);
          }
        }
      } 
    }

    static IList<int> createAttributes(IList<Movie> movies)
    {
      var attributeExtractor = new AttributeExtractor();
      var fileWriter = new FileWritter();

      var attr_counts = new List<int>();

      foreach (var percentage in new[] { .05m, .01m, .005m })
      {
        Console.WriteLine("Choosing attributes...");
        var attributes = attributeExtractor.chooseAttributesFromMovies(movies, percentage);
        Console.WriteLine("Extracting attributes...");
        var movieAttributes = attributeExtractor.extractAttributesFromMovies(movies, attributes, true);

        Console.WriteLine("Writing attributes...");
        fileWriter.writeAttributes(attributes);
        Console.WriteLine("Writing movies attributes...");
        fileWriter.writeMovieAttributes(movieAttributes, attributes.Count);
        attr_counts.Add(attributes.Count);
      }
      return attr_counts;
    }
  }
}
