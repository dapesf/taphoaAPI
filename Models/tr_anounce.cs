using System.ComponentModel.DataAnnotations;
public class tr_anounce
{
    [Key]
    [StringLength(20)]
    public string cd_announce { get; set; }
    public short type_announce { get; set; }
    [StringLength(20)]
    public string cd_store { get; set; }
    public DateOnly dt_send { get; set; }
    [StringLength(50)]
    public string nm_title { get; set; }
    [StringLength(1000)]
    public string nm_content { get; set; }
    public short flg_viewed { get; set; }
}
