using TypeBridge.Test.TestMappingClasses;
using System.Linq;
using Xunit;

namespace TypeBridge.Test
{
    public class GoblinTests
    {
        [Fact]
        public void You_can_map_a_goblin()
        {
            var goblin = new Goblin(sneakyness: 10);

            GoblinDto result = goblin.Map();

            Assert.Equal(goblin.Name, result.Name);
            Assert.Equal(goblin.HitPoints, result.HitPoints);
            Assert.Equal(goblin.Sneakyness, result.Sneakyness);
        }

        [Fact]
        public void You_can_map_goblin_by_extending_the_source_object()
        {
            Monster goblin = new Goblin(sneakyness: 10);

            GoblinDto result = goblin.Map().Extend(new GoblinTraitsDto { Sneakyness = 4 });

            Assert.Equal(goblin.Name, result.Name);
            Assert.Equal(goblin.HitPoints, result.HitPoints);
            Assert.Equal(4, result.Sneakyness);
        }

        [Fact]
        public void You_can_map_a_goblin_boss_as_a_goblin()
        {
            var goblin = new GoblinBoss(new[] { new Goblin(sneakyness: 10), new Goblin(sneakyness: 15) });

            GoblinDto result = goblin.Map();

            Assert.Equal(goblin.Name, result.Name);
            Assert.Equal(goblin.HitPoints, result.HitPoints);
            Assert.Equal(goblin.Sneakyness, result.Sneakyness);
        }

        [Fact]
        public void You_can_map_a_goblin_boss_as_a_boss()
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
        public void You_can_map_a_goblin_camp()
        {
            var goblin = new Goblin(sneakyness: 10);
            var goblinCamp = new MonsterCamp<Goblin>(goblin);

            MonsterCampDto<GoblinDto> result = goblinCamp.Map();

            Assert.Equal(result.Monsters.Count(), goblinCamp.Monsters.Length);
            Assert.Equal(result.Monsters.ElementAt(0).Name, goblinCamp.Monsters[0].Name);
            Assert.Equal(result.Monsters.ElementAt(0).HitPoints, goblinCamp.Monsters[0].HitPoints);
            Assert.Equal(result.Monsters.ElementAt(0).Sneakyness, goblinCamp.Monsters[0].Sneakyness);
        }

        //[Fact]
        //public void You_can_map_a_goblin_to_food()
        //{
        //    var goblin1 = new Goblin(sneakyness: 10);
        //    var goblin2 = new Goblin(sneakyness: 5);

        //    goblin1.Eat(goblin2.Map());

        //    Assert.False(goblin1.IsHungry);
        //}
    }
}
