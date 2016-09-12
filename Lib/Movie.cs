using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
  public class Movie
  {

    public int id { get; private set; }

    public string title { get; private set; }

    public int year { get; private set; }

    //public Genre genres { get; private set; }

    //public string keywords { get; private set; }    

    public Movie(int id, string title, int year)
    {
      if (title == null)
        throw new ArgumentException("title");

      this.id = id;
      this.title = title;
      this.year = year;
    }
  }
}
