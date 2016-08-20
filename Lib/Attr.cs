using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
  public class Attr
  {    
    public int attr_index { get; private set; }

    public string attribute { get; private set; }

    public double population_average { get; set; }

    public double population_standard_deviation { get; set; }

    public Attr(int attr_index, string attribute)
    {
      if (attribute == null)
        throw new ArgumentException("attribute");

      this.attr_index = attr_index;
      this.attribute = attribute;
    }

    public Attr(int attr_index, string attribute, double population_average, double population_standard_deviation)
      :this(attr_index, attribute)
    {            
      this.population_average = population_average;
      this.population_standard_deviation = population_standard_deviation;
    }

  }
}
