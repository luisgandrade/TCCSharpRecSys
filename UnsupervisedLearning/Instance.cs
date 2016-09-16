using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
  public class Instance
  {
    public Movie movie { get; private set; }

    public IList<TagRelevance> tag_relevances { get; private set; }

    public Instance(IList<TagRelevance> tag_relevances)
    {
      if (tag_relevances == null)
        throw new ArgumentException("tag_relevances");

      this.movie = tag_relevances.Select(tr => tr.movie).Distinct().Single();
      this.tag_relevances = tag_relevances.OrderBy(tr => tr.tag.id).ToList();
    }
  }
}
