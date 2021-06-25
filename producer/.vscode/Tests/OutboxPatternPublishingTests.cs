using xunit;

namespace kafka_poc.Tests
{
    public class GIVEN_I_want_publish_events_to_the_outbox
    {
        public class WHEN_I_create_a_valid_preference
        {
            public OutboxPatternPublishingTests()
            {
                var preferenceId = -1;
                var preferenceSaverService = new PreferenceSaverService();
                //var outboxCreatorService = new MessageQueuerService();

                preferenceSaverService.Create(
                    new Preference { Type = "Test preference" },
                    id => preferenceId = id);
            }

            public class PreferenceSaverService
            {
                public class Preference
                {
                    public string Type { get; set; }
                }
                public void Create(
                    Preference preference,
                    Action<id> onPreferenceCreated)
                {
                }
            }

            [Fact]
            public void THEN_the_preference_is_created()
            {

            }

            public void AND_an_entry_is_added_to_the_outbox_table()
            {

            }
        }
    }
}