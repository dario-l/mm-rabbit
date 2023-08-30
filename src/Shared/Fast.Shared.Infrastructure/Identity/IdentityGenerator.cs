using Fast.Shared.Abstractions.Identity;
using IdGen;

namespace Fast.Shared.Infrastructure.Identity;

internal sealed class IdentityGenerator : IIdGen
{
    private readonly IdGenerator _idGen;

    public IdentityGenerator(int generatorId = 0)
    {
        _idGen = new IdGenerator(generatorId);
    }

    public long Create() => _idGen.CreateId();
}