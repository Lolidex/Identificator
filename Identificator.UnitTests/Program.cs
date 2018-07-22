using System.Threading.Tasks;
using Xunit;

namespace Identificator.UnitTests
{
    public class Program
    {
        [Fact]
        public async Task Test()
        {
            Identificator identificator = new Identificator();
            Identificator.CharacInfo infos = await identificator.GetAnime("Chino", "Kafuu");
            Assert.Equal("gochuumon_wa_usagi_desu_ka?", infos.source);
            Assert.Equal("blue", infos.eyesColor);
            Assert.Equal("blue", infos.hairColor);
        }
    }
}
