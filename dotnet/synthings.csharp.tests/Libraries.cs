namespace synthings.csharp.tests
{
    using Xunit;
    using System.Linq;
    using csharp;

    public class Libraries
    {
        [Fact]
        public void Load_the_aggregate_library()
        {
            var library = Library.Load();
            Assert.Equal("Aggregate", library.Name);
        }

        [Fact]
        public void Find_a_Behavior()
        {
            var library = Library.Load();
            var topics = library.listTopics();
            var waveTopic = topics.First(topic => topic.DisplayName.Contains("Wave"));
            var behaviors = library.listBehaviors(waveTopic);
            var sineWaveBehavior = behaviors.First(behavior => behavior.DisplayName.Contains("Sine"));
            Assert.Equal("Sine Wave", sineWaveBehavior.DisplayName);
        }

        [Fact]
        public void Create_a_machine()
        {
            var library = Library.Load();
            var topics = library.listTopics();
            var envelopeTopic = topics.First(topic => topic.DisplayName.Contains("Envelope"));
            var behaviors = library.listBehaviors(envelopeTopic);
            var linearDecayBehavior = behaviors.First(behavior => behavior.DisplayName.Contains("Linear"));
            var linearDecayMachine = library.createMachine(linearDecayBehavior);
            Assert.Equal("Linear Decay", linearDecayMachine.Name);
        }
    }
}