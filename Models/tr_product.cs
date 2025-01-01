using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(cd_product), nameof(cd_store))]
public class tr_product
{
    [StringLength(20)]
    public string cd_product { get; set; }
    [StringLength(20)]
    public string cd_store { get; set; }
    [StringLength(100)]
    public string nm_product { get; set; }
    [StringLength(100)]
    public string nm_product_en { get; set; }
    public DateOnly dt_start { get; set; }
    public DateOnly dt_end { get; set; }
    public double kin_price { get; set; }
    [StringLength(20)]
    public string type_unit { get; set; }
    public int qnt_in { get; set; }
    public int qnt_remain { get; set; }
    [StringLength(3)]
    public string cd_country { get; set; }
}
