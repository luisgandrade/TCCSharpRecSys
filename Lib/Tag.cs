using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
  public class Tag
  {    
    public int id { get; private set; }

    public string tag { get; private set; }

    public double population_average { get; set; }

    public double population_standard_deviation { get; set; }

    public Tag(int id, string tag)
    {
      if (tag == null)
        throw new ArgumentException("tag");
      
      this.id = id;
      this.tag = tag;
    }

    public Tag(int id, string tag, double population_average, double population_standard_deviation)
      :this(id, tag)
    {            
      this.population_average = population_average;
      this.population_standard_deviation = population_standard_deviation;
    }

  }
}
