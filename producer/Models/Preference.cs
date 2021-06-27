using System;

namespace kafka_poc.Models
{
    public class PreferenceCreationModel
    {
        public string @Type { get; set; }
    }
    public class PreferenceWithoutId : PreferenceCreationModel
    {
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }

    public class Preference : PreferenceWithoutId
    {
        public Preference() { }
        public Preference(int id, PreferenceCreationModel pref)
        {
            Id = id;
            Type = pref.Type;
        }

        public int Id { get; set; }
    }
}