using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandsService.Data.Interface;
using CommandsService.Models;

namespace CommandsService.Data.Implementation
{
  public class CommandRepository : ICommandRepository
  {
    private readonly AppDbContext _context;
    public CommandRepository(AppDbContext context)
    {
      _context = context;

    }
    public void CreateCommand(int platformid, Command command)
    {
      if (command == null)
      {
        throw new ArgumentNullException(nameof(command));
      }
      command.PlatformId = platformid;
      _context.Commands.Add(command);
    }

    public void CreatePlatform(Platform platform)
    {
      if (platform == null)
      {
        throw new ArgumentNullException(nameof(platform));
      }
      _context.Platforms.Add(platform);
    }

    public bool ExternalPlatformExists(int externalPlatformId)
    {
      return _context.Platforms.Any(p => p.ExternalID == externalPlatformId);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
      return _context.Platforms.ToList();
    }

    public Command GetCommand(int platformId, int commandId)
    {
      return _context.Commands
          .Where(c => c.PlatformId == platformId && c.Id == commandId).FirstOrDefault();
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
      return _context.Commands
        .Where(c => c.PlatformId == platformId)
        .OrderBy(c => c.Platform.Name);
    }

    public bool PlatformExits(int platformId)
    {
      return _context.Platforms.Any(p => p.Id == platformId);
    }

    public bool SaveChanges()
    {
      return (_context.SaveChanges() >= 0);
    }
  }
}