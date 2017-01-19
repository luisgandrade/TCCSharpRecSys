using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnsupervisedLearning;
using Lib;
using TCCSharpRecSys;

namespace Tests.TCCSharpRecSys
{
  /// <summary>
  /// Summary description for RecommenderTests
  /// </summary>
  [TestClass]
  public class RecommenderTests
  { 
    [TestMethod]
    public void MoviesAlreadyWatchedAreNotCounted()
    {
      var bestMatchingClassLabel = Mock.Of<IClassLabel>();
      var secondBestMatchingClassLabel = Mock.Of<IClassLabel>();
      var movieA = new Movie(1, "MovieA", 2016);
      var movieB = new Movie(2, "MovieB", 2016);
      var movieC = new Movie(3, "MovieC", 2016);
      var movieD = new Movie(4, "MovieD", 2016);
      var movieE = new Movie(5, "MovieE", 2016);
      var movieF = new Movie(6, "MovieF", 2016);

      var moviesAlreadyWatched = new[] { movieA, movieB };
      var ratingsNotIncludedIdProfile = new[] { new Rating(movieD, 1, 1, DateTime.Now), new Rating(movieF, 1, 1, DateTime.Now) };
      var userProfile = new UserProfile(1, new List<double>());

      var algorithm = Mock.Of<IUnsupervisedLearning>(ul => ul.best_matching_units(userProfile) == new[] { bestMatchingClassLabel, secondBestMatchingClassLabel });

      var moviesByClassLabel = new Dictionary<IClassLabel, List<Movie>>();
      moviesByClassLabel.Add(bestMatchingClassLabel, new List<Movie>() { movieA, movieC, movieE });
      moviesByClassLabel.Add(secondBestMatchingClassLabel, new List<Movie>() { movieB, movieD, movieF });

      var recommender = new Recommender(algorithm, moviesByClassLabel, 2);
      var recommenderResults = recommender.recommend(userProfile, moviesAlreadyWatched, ratingsNotIncludedIdProfile);
      
      Assert.AreEqual(0, recommenderResults.first_n_recommendations_precision);
      Assert.AreEqual(2, recommenderResults.last_n_recommendations_precision);
      Assert.AreEqual(4, recommenderResults.number_of_ratings);
    }
  }
}
