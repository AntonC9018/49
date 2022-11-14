using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fourtynine.DataAccess;

#pragma warning disable 8618 // Disable nullability warnings for EF Core

public sealed class Author
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }
    
    [MaxLength(100)]
    public string Email { get; set; }
    
    public ICollection<Posting> Postings { get; set; }
}