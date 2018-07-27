using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Identificator.UnitTests
{
    public class Program
    {
        [Fact]
        public async Task GetInfos()
        {
            Identificator identificator = new Identificator();
            Identificator.CharacInfo infos = await identificator.GetAnime("Chino", "Kafuu");
            Assert.Equal("gochuumon_wa_usagi_desu_ka?", infos.source);
            Assert.Equal("blue", infos.eyesColor);
            Assert.Equal("blue", infos.hairColor);
        }

        [Fact]
        public async Task CorrectName()
        {
            Identificator identificator = new Identificator();
            Assert.NotInRange((await identificator.CorrectName("hibiki")).ToArray().Length, 0, 1);
        }
    }
}
