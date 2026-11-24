using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BD.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2a12c78b-cf9b-4718-ba6e-09cb72c2e05f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b3c0f6e1-5766-40d7-abc5-c55875cad672");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cc27765b-a262-4cca-926f-177e30f65106");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f0a77725-b3b5-4071-9ada-52afb9294476");

            migrationBuilder.AddColumn<bool>(
                name: "Justified",
                table: "Attendances",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Recorded",
                table: "Attendances",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "171f2a41-6715-459f-8c66-3cda319596b9", "4", "Docente", "DOCENTE" },
                    { "351fed30-76ad-4da8-a065-27b7db4aaa7d", "3", "Preceptor_Auxiliar", "PRECEPTOR_AUXILIAR" },
                    { "a962e87c-3b6e-4138-8517-4cd13ac3d9ab", "5", "Estudiante", "ESTUDIANTE" },
                    { "d4803d7e-c4a9-40dc-b145-c0cfbfc3eeb1", "2", "Preceptor", "PRECEPTOR" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ae65a54a-aab5-4d0c-aa17-554334b49e8c",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c79aa666-ee5e-43c0-b104-e86ec0965a99", "AQAAAAIAAYagAAAAENgeQ2a51Vp7jbOLtIN34BtFHTuqqtzFGsQ0PR4uIIalxlWkpjkg+d2kAYWImITjog==", "7836e86b-4521-4b61-92be-03a868470770" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "171f2a41-6715-459f-8c66-3cda319596b9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "351fed30-76ad-4da8-a065-27b7db4aaa7d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a962e87c-3b6e-4138-8517-4cd13ac3d9ab");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d4803d7e-c4a9-40dc-b145-c0cfbfc3eeb1");

            migrationBuilder.DropColumn(
                name: "Justified",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Recorded",
                table: "Attendances");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2a12c78b-cf9b-4718-ba6e-09cb72c2e05f", "3", "Preceptor_Auxiliar", "PRECEPTOR_AUXILIAR" },
                    { "b3c0f6e1-5766-40d7-abc5-c55875cad672", "4", "Docente", "DOCENTE" },
                    { "cc27765b-a262-4cca-926f-177e30f65106", "2", "Preceptor", "PRECEPTOR" },
                    { "f0a77725-b3b5-4071-9ada-52afb9294476", "5", "Estudiante", "ESTUDIANTE" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ae65a54a-aab5-4d0c-aa17-554334b49e8c",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0c9e9273-2313-46b3-b655-5b246394a1a3", "AQAAAAIAAYagAAAAEEJ1jK6P14OkK4v57vrBlQvKAhWwtNJdK+jKILxBJAt4ag3IE5VPb6O7BibTvL27JA==", "3d86c3ea-9539-4c2a-b3ed-efeb5c30a10f" });
        }
    }
}
