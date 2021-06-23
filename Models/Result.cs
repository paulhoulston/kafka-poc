using System.Collections.Generic;

namespace kafka_poc.Models
{
    public class Link
    {
        public Link(string href) => Href = href;
        public string Href { get; set; }
    }

    public class Result<T> : EmptyLinkedResult
    {
        public Result(T data) : base() => Data = data;
        public readonly T Data;
    }

    public class EmptyLinkedResult
    {
        public EmptyLinkedResult() => Links = new List<Link>();
        public readonly IList<Link> Links;
    }
}