using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace taphoa.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "cd_country",
                table: "tr_product",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "AspNetUsers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<short>(
                name: "cd_country",
                table: "tr_product",
                type: "smallint",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);
        }
    }
}
