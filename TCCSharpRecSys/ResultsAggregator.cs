using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCCSharpRecSys.Persistence;

namespace TCCSharpRecSys
{
  public class ResultsAggregator
  {

    private FileWritter file_writter;
    private FileReader file_reader;

    public ResultsAggregator()
    {
      FileReader.setDirPath("C:\\Users\\luis\\Documents\\Projetos\\tccsharprecsys\\Data\\");
      FileWritter.setDirPath("C:\\Users\\luis\\Documents\\Projetos\\tccsharprecsys\\Data\\");
      file_reader = FileReader.getInstance();
      file_writter = FileWritter.getInstance();      
    }
    
    public void usersWithQuantityOfRatings(decimal cutoff)
    {
      
      var userRatings = new List<Tuple<int, int, int>>();
      var ratingsParts = file_reader.getPartsOfRatings();
      foreach (var part in ratingsParts)
      {
        var ratings = file_reader.readUserRatings(part);
        userRatings.AddRange(ratings.GroupBy(r => r.user_id).Select(r => new Tuple<int, int, int>(r.Key, (int)(r.Count() * cutoff), (int)(r.Count() * (1 -cutoff)))));
      }
      file_writter.writeUsersWithQuantityOfRatings(cutoff, userRatings);
    }
  }
}
