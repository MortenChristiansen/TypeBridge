using TypeBridge.Test.TestMappingClasses;
using System.Linq;
using Xunit;

namespace TypeBridge.Test
{
    public class GoblinTests
    {
        [Fact]
        public void You_can_map_one_type_into_another()
        {
            var goblin = new Goblin(sneakyness: 10);

            GoblinDto result = goblin.Map();

            Assert.Equal(goblin.Name, result.Name);
            Assert.Equal(goblin.HitPoints, result.HitPoints);
            Assert.Equal(goblin.Sneakyness, result.Sneakyness);
        }

        [Fact]
        public void You_can_map_multiple_source_objects_to_a_single_destination_object()
        {
            Monster goblin = new Goblin(sneakyness: 10);

            GoblinDto result = goblin.Map().Extend(new GoblinTraitsDto { Sneakyness = 4 });

            Assert.Equal(goblin.Name, result.Name);
            Assert.Equal(goblin.HitPoints, result.HitPoints);
            Assert.Equal(4, result.Sneakyness);
        }

        [Fact]
        public void You_can_map_an_object_to_one_of_its_sub_types()
        {
            var goblin = new GoblinBoss(new[] { new Goblin(sneakyness: 10), new Goblin(sneakyness: 15) });

            GoblinDto result = goblin.Map();

            Assert.Equal(goblin.Name, result.Name);
            Assert.Equal(goblin.HitPoints, result.HitPoints);
            Assert.Equal(goblin.Sneakyness, result.Sneakyness);
        }

        [Fact]
        public void You_can_map_objects_with_collection_properties()
        {
            var goblin = new GoblinBoss(new[] { new Goblin(sneakyness: 10), new Goblin(sneakyness: 15) });

            GoblinBossDto result = goblin.Map();

            Assert.Equal(goblin.Name, result.Name);
            Assert.Equal(goblin.HitPoints, result.HitPoints);
            Assert.Equal(goblin.Sneakyness, result.Sneakyness);
            Assert.Equal(2, result.Subjects.Count());
            Assert.Equal(goblin.Subjects.ElementAt(0).Name, result.Subjects.ElementAt(0).Name);
            Assert.Equal(goblin.Subjects.ElementAt(0).HitPoints, result.Subjects.ElementAt(0).HitPoints);
            Assert.Equal(goblin.Subjects.ElementAt(0).Sneakyness, result.Subjects.ElementAt(0).Sneakyness);
            Assert.Equal(goblin.Subjects.ElementAt(1).Name, result.Subjects.ElementAt(1).Name);
            Assert.Equal(goblin.Subjects.ElementAt(1).HitPoints, result.Subjects.ElementAt(1).HitPoints);
            Assert.Equal(goblin.Subjects.ElementAt(1).Sneakyness, result.Subjects.ElementAt(1).Sneakyness);
        }

        [Fact]
        public void You_can_map_to_generic_types()
        {
            var goblin = new Goblin(sneakyness: 10);
            var goblinCamp = new MonsterCamp<Goblin>(goblin);

            MonsterCampDto<GoblinDto> result = goblinCamp.Map();

            Assert.Equal(result.Monsters.Count(), goblinCamp.Monsters.Length);
            Assert.Equal(result.Monsters.ElementAt(0).Name, goblinCamp.Monsters[0].Name);
            Assert.Equal(result.Monsters.ElementAt(0).HitPoints, goblinCamp.Monsters[0].HitPoints);
            Assert.Equal(result.Monsters.ElementAt(0).Sneakyness, goblinCamp.Monsters[0].Sneakyness);
        }

        [Fact]
        public void You_can_map_to_a_destination_type_having_a_constructor_if_the_constructor_accepts_all_the_properties()
        {
            Monster goblin = new Goblin(sneakyness: 10);

            GenericMonster monster = goblin.Map();

            Assert.Equal(goblin.Name, monster.Name);
            Assert.Equal(goblin.HitPoints, monster.HitPoints);
        }
    }
}
