﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;

namespace TCCSharpRecSys
{
  public class RecommendationResults
  {

    public UserProfile user { get; private set; }

    public int number_of_ratings { get; private set; }

    public int number_of_correct_predictions { get; private set; }
    
    public RecommendationResults(UserProfile user, int number_of_ratings, int number_of_correct_predictions)
    {
      if (user == null)
        throw new ArgumentException("user");

      this.user = user;
      this.number_of_ratings = number_of_ratings;
      this.number_of_correct_predictions = number_of_correct_predictions;
    }
  }
}
