namespace Lib
{
  public class TagRelevance
  {

    public Movie movie { get; private set; }

    public Tag tag { get; private set; }

    public double relevance { get; private set; }    

    public TagRelevance(Movie movie, Tag tag, double relevance)
    {
      this.movie = movie;
      this.tag = tag;
      this.relevance = relevance;
    }
    
  }
}
