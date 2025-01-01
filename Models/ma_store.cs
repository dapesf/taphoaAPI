using System.ComponentModel.DataAnnotations;
public class ma_store
{
    [Key]
    [StringLength(20)]
    public string cd_store { get; set; }
    [StringLength(100)]
    public string nm_store { get; set; }
    [StringLength(100)]
    public string nm_address { get; set; }
    [StringLength(20)]
    public string cd_phone_number { get; set; }
    public short flg_delete { get; set; }
}
