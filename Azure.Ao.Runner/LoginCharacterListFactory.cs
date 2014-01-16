using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Azure.Ao.Runner
{
    public class LoginCharacterListFactory : IMessageFactory
    {
        private static IEnumerable<LoginCharacter> ReadIds(byte[] data, ref int offset, bool isLittleEndian)
        {
            var index = offset;
            var numberOfCharacters = data.PopShort(ref index, isLittleEndian);
            var result = Enumerable.Range(0, numberOfCharacters)
                .Select(character => data.PopUInt32(ref index, isLittleEndian))
                .Select(id => new LoginCharacter { Id = id })
                .ToArray();
            offset = index;
            return result;
        }

        private static IEnumerable<string> ReadNames(byte[] data, ref int offset, bool isLittleEndian)
        {
            var index = offset;
            var numberOfCharacters = data.PopShort(ref index, isLittleEndian);
            Console.WriteLine("numberofcharacters: {0}, IsLittleEndian: {1}", numberOfCharacters, isLittleEndian);
            var result = Enumerable.Range(0, numberOfCharacters)
                .Select(character => data.PopString(ref index, isLittleEndian))
                .ToArray();
            offset = index;
            foreach (var name in result)
            {
                Console.WriteLine(name);
            }
            return result;
        }

        private static IEnumerable<int> ReadLevels(byte[] data, ref int offset, bool isLittleEndian)
        {
            var index = offset;
            var numberOfCharacters = data.PopShort(ref index, isLittleEndian);
            var result = Enumerable.Range(0, numberOfCharacters)
                .Select(character => data.PopInt32(ref index, isLittleEndian))
                .ToArray();
            offset = index;
            return result;
        }

        private static IEnumerable<bool> ReadOnlineStatuses(byte[] data, ref int offset, bool isLittleEndian)
        {
            var index = offset;
            var numberOfCharacters = data.PopShort(ref index, isLittleEndian);
            var result = Enumerable.Range(0, numberOfCharacters)
                .Select(character => Convert.ToBoolean(data.PopInt32(ref index, isLittleEndian)))
                .ToArray();
            offset = index;
            return result;
        }

        public IAoMessage Create(byte[] data)
        {
            var offset = 0;
            var characters = ReadIds(data, ref offset, true)
                .Zip(ReadNames(data, ref offset, true), (character, name) => { character.Name = name; return character; })
                .Zip(ReadLevels(data, ref offset, true), (character, level) => { character.Level = level; return character; })
                .Zip(ReadOnlineStatuses(data, ref offset, true), (character, online) => { character.Online = online; return character; })
                .ToLoginCharacterList();

            return characters;
        }
    }
}