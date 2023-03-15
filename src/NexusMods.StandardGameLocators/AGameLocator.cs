using GameFinder.Common;
using Microsoft.Extensions.Logging;
using NexusMods.DataModel.Games;
using NexusMods.Paths;

namespace NexusMods.StandardGameLocators;

public abstract class AGameLocator<TStore, TRecord, TId, TGame> : IGameLocator
    where TStore : AHandler<TRecord, TId>
    where TRecord : class
    where TGame : IGame
{
    private readonly ILogger _logger;
    private readonly AHandler<TRecord, TId> _handler;
    private IDictionary<TId, TRecord>? _cachedGames;

    protected AGameLocator(ILogger logger, AHandler<TRecord, TId> handler)
    {
        _logger = logger;
        _handler = handler;
    }

    public IEnumerable<GameLocatorResult> Find(IGame game)
    {
        if (game is not TGame tg) return Enumerable.Empty<GameLocatorResult>();

        if (_cachedGames is null)
        {
            _cachedGames = _handler.FindAllGamesById(out var errors);
            if (errors.Any())
            {
                foreach (var error in errors)
                    _logger.LogError("While looking for games: {Error}", error);
            }
        }

        return Ids(tg)
            .Where(id => _cachedGames.ContainsKey(id))
            .Select(id => _cachedGames[id])
            .Select(found => new GameLocatorResult(Path(found)));
    }

    protected abstract IEnumerable<TId> Ids(TGame game);
    protected abstract AbsolutePath Path(TRecord record);
}
