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

    public int count { get; private set; }

    public Tag(int id, string tag, int count)
    {
      if (tag == null)
        throw new ArgumentException("tag");
      
      this.id = id;
      this.tag = tag;
      this.count = count;
      
    }
    

  }
}
