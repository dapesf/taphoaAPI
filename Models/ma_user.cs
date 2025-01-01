using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ma_user : IdentityUser
{
    [Key]
    [StringLength(20)]
    public string cd_phone_number { get; set; }
    [StringLength(20)]
    public string cd_store { get; set; }
    [StringLength(100)]
    public string cd_password { get; set; }
    [StringLength(20)]
    public short flg_announce_exp { get; set; }
    public short flg_announce_fast { get; set; }
    public short flg_announce_slow { get; set; }
    public short flg_announce_soldout { get; set; }
    [StringLength(10)]
    public string lst_announce { get; set; }
    public short flg_delete { get; set; }
}
