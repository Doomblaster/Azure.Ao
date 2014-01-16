using System;
using System.Collections.Generic;

namespace Azure.Ao.Runner
{
    public static class LoginCharacterListExtensions
    {
        public static LoginCharacterList ToLoginCharacterList(this IEnumerable<LoginCharacter> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return new LoginCharacterList(source);
        }
    }
}