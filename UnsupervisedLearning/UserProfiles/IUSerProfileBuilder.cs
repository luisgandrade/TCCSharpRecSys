using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.UserProfiles
{
  public interface IUSerProfileBuilder
  {
    IList<UserProfile> buildUserProfiles(IList<Rating> ratings, IList<TagRelevance> tagRelevances, double trainingCutoff);

    string label { get; }
  }
}
