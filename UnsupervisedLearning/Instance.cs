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

    private IList<double> _relevances;

    private IList<double> _normalized_relevances;

    public IList<double> getRelevances(bool useNormalizedValues)
    {
      if (useNormalizedValues)
        return _normalized_relevances;
      else
        return _relevances;
    }

    public Instance(IList<TagRelevance> tag_relevances)
    {
      if (tag_relevances == null)
        throw new ArgumentException("tag_relevances");

      this.movie = tag_relevances.Select(tr => tr.movie).Distinct().Single();
      this.tag_relevances = tag_relevances.OrderBy(tr => tr.tag.id).ToList();
      _relevances = this.tag_relevances.Select(tr => tr.relevance).ToList();
      _normalized_relevances = this.tag_relevances.Select(tr => tr.normalized_relevance.Value).ToList();
    }
  }
}
