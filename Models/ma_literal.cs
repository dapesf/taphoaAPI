using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(cd_type), nameof(kbn1))]
public class ma_literal
{
    [StringLength(3)]
    public string cd_type { get; set; }
    [StringLength(3)]
    public string kbn1 { get; set; }
    [StringLength(30)]
    public string nm1 { get; set; }
    [StringLength(3)]
    public string kbn2 { get; set; }
    [StringLength(30)]
    public string nm2 { get; set; }
}
