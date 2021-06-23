namespace kafka_poc.Models
{
    public class PreferenceWithoutInternals
    {
        public string @Type { get; set; }
    }

    public class Preference : PreferenceWithoutInternals
    {
        public Preference() { }
        public Preference(int id, PreferenceWithoutInternals pref)
        {
            Id = id;
            Type = pref.Type;
        }

        public int Id { get; set; }
    }
}