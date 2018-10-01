using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Superheroes.Controllers
{
    [Route("battle")]
    public class BattleController : Controller
    {
        private readonly ICharactersProvider _charactersProvider;
        
        public BattleController(ICharactersProvider charactersProvider)
        {
            _charactersProvider = charactersProvider;
        }

        public async Task<IActionResult> Get(string hero, string villain)
        {
            var characters = await _charactersProvider.GetCharacters();

            
            var heroCharachter = characters.Items.SingleOrDefault(x => x.Name == hero && x.Type == "hero");
            var villainCharacter = characters.Items.SingleOrDefault(x => x.Name == villain && x.Type == "villain");

            if (heroCharachter == null && villainCharacter == null)
                return NotFound();

            if (heroCharachter == null)
                return BadRequest($"Hero {hero} does not exists.");

            if (villainCharacter == null)
                return BadRequest($"Villain {villain} does not exists");

            if (heroCharachter.Weakness == villainCharacter.Name)
                heroCharachter.Score -= 1;

            if(heroCharachter.Score > villainCharacter.Score)
            {
                return Ok(heroCharachter);
            }

            return Ok(villainCharacter);
        }
    }
}