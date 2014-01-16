using System.Collections.Generic;

namespace Azure.Ao.Runner
{
    public class LoginCharacterList : List<LoginCharacter>, IAoMessage
    {
        public LoginCharacterList(IEnumerable<LoginCharacter> characters)
            : base(characters)
        {

        }
    }
}