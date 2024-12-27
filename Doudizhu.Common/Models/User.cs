using System.ComponentModel.DataAnnotations;

namespace Doudizhu.Api.Models;

public class User : GuidModelBase
{
    [MaxLength(60)]
    public required string Name { get; set; }
    public long Coin { get; set; }
    
    [MaxLength(20)]
    public string? Qq { get; set; }
}