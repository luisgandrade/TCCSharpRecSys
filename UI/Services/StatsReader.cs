using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Services
{
  public class StatsReader
  {



    public ConfigStats readConfigStats(string config)
    {


      var histogram = new[]
      {
        new KeyValuePair<double, double>(.02, 10),
        new KeyValuePair<double, double>(.04, 12),
        new KeyValuePair<double, double>(.06, 15),
        new KeyValuePair<double, double>(.08, 11),
      };

      var average = new[]
      {
        new KeyValuePair<double, double>(.02, 50),
        new KeyValuePair<double, double>(.04, 60),
        new KeyValuePair<double, double>(.06, 70),
        new KeyValuePair<double, double>(.08, 10),
      };

      var std_dev = new[]
      {
        new KeyValuePair<double, double>(.02, 56.2),
        new KeyValuePair<double, double>(.04, 42.5),
        new KeyValuePair<double, double>(.06, 15.5),
        new KeyValuePair<double, double>(.08, 42.3),
      };

      var min = new[]
      {
        new KeyValuePair<double, double>(.02, 50.3),
        new KeyValuePair<double, double>(.04, 10.5),
        new KeyValuePair<double, double>(.06, 15.8),
        new KeyValuePair<double, double>(.08, 10.7),
      };

      var max = new[]
      {
        new KeyValuePair<double, double>(.02, 100),
        new KeyValuePair<double, double>(.04, 150),
        new KeyValuePair<double, double>(.06, 200),
        new KeyValuePair<double, double>(.08, 100),
      };

      return new ConfigStats(histogram, average, std_dev, min, max);
    }

    
  }
}