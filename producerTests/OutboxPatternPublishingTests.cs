// using System;
// using Xunit;

// namespace producerTests
// {
//     public class GIVEN_I_want_publish_events_to_the_outbox
//     {
//         public class WHEN_I_create_a_valid_preference : PreferenceSaverService.ISavePreferences, PreferenceSaverService.IWriteToOutboxQueue
//         {
//             int _preferenceId = -1;
//             PreferenceSaverService.Preference _savedPreference;
//             bool _haveWrittenToOutbox;

//             readonly int _expectedPreferenceId = new Random().Next();

//             public WHEN_I_create_a_valid_preference()
//             {
//                 var preferenceSaverService = new PreferenceSaverService(this, this);
//                 preferenceSaverService.Create(
//                     new PreferenceSaverService.Preference { Type = "Test preference" },
//                     id => _preferenceId = id);
//             }

//             [Fact] public void THEN_the_preference_is_saved_to_the_database() => Assert.NotNull(_savedPreference);

//             [Fact] public void AND_the_service_supplies_the_expected_preference_id_to_the_success_delegate() => Assert.Equal(_expectedPreferenceId, _preferenceId);

//             [Fact] public void AND_an_entry_is_added_to_the_outbox_table() => Assert.True(_haveWrittenToOutbox);

//             public void Save(PreferenceSaverService.Preference preference, Action<int> onPreferenceSaved)
//             {
//                 _savedPreference = preference;
//                 onPreferenceSaved(_expectedPreferenceId);
//             }

//             public void WriteToQueue(PreferenceSaverService.Preference preference, Action onEventQueued)
//             {
//                 _haveWrittenToOutbox = true;
//                 onEventQueued();
//             }
//         }

//         public class PreferenceSaverService
//         {
//             readonly ISavePreferences _preferencesRepository;
//             readonly IWriteToOutboxQueue _outboxWriter;

//             public interface ISavePreferences
//             {
//                 void Save(PreferenceSaverService.Preference preference, Action<int> onPreferenceSaved);
//             }

//             public interface IWriteToOutboxQueue
//             {
//                 void WriteToQueue(PreferenceSaverService.Preference preference, Action onEventQueued);
//             }


//             public class Preference
//             {
//                 public string Type { get; set; }
//             }

//             public PreferenceSaverService(
//                 ISavePreferences preferencesRepository,
//                 IWriteToOutboxQueue outboxWriter)
//             {
//                 _preferencesRepository = preferencesRepository;
//                 _outboxWriter = outboxWriter;
//             }

//             public void Create(
//                 Preference preference,
//                 Action<int> onPreferenceCreated) =>
//                     _preferencesRepository.Save(
//                         preference,
//                         id => WriteToOutbox(preference, () => onPreferenceCreated(id)));

//             void WriteToOutbox(Preference preference, Action onEventQueued) =>
//                 _outboxWriter.WriteToQueue(
//                     preference,
//                     onEventQueued);
//         }
//     }
// }