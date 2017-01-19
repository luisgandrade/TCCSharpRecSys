using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCCSharpRecSys.Persistence
{
  public class ResultsAggregate
  {

    public string configuration { get; set; }

    public decimal cutoff { get; set; }

    public int recommendation_list_size { get; set; }
    
    public decimal average_precision_first_n { get; set; }

    public decimal std_dev_precision_first_n { get; set; }

    public decimal average_precision_second_n { get; set; }

    public decimal std_dev_precision_second_n { get; set; }


    public ResultsAggregate(string configuration, decimal cutoff, int recomendation_list_size)
    {
      this.configuration = configuration;
      this.cutoff = cutoff;
      this.recommendation_list_size = recommendation_list_size;
    }
  }
}
