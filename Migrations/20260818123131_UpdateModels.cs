using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DengeWeb.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AboutSummary",
                table: "SiteSettings",
                newName: "IndexStockVideoUrl");

            migrationBuilder.RenameColumn(
                name: "AboutDetail",
                table: "SiteSettings",
                newName: "IndexEntryTitle");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Products",
                newName: "ShortDescription");

            migrationBuilder.AddColumn<string>(
                name: "AboutDescription",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutEthicalValuesDescription",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutEthicalValuesTitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutMissionVisionDescription",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutMissionVisionImageUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutMissionVisionTitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutTitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutValuesDescription",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutValuesImageUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutValuesTitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactTitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FaviconUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FooterCopyrightText",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FooterFacebookUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FooterInstagramUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FooterLinkedinUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FooterLogoUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FooterTwitterUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FooterYoutubeUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GoogleMapEmbedUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HeaderLogoUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HeaderWhiteLogoUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IndexAboutButtonText",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IndexAboutDescription",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IndexAboutImageUrl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IndexAboutTitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IndexContactButtonText",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IndexContactDescription",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IndexContactSubtitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IndexContactTitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IndexEntryDescription",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BrochureUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GalleryUrls",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "LongDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutDescription",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "AboutEthicalValuesDescription",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "AboutEthicalValuesTitle",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "AboutMissionVisionDescription",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "AboutMissionVisionImageUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "AboutMissionVisionTitle",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "AboutTitle",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "AboutValuesDescription",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "AboutValuesImageUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "AboutValuesTitle",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "ContactTitle",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "FaviconUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "FooterCopyrightText",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "FooterFacebookUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "FooterInstagramUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "FooterLinkedinUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "FooterLogoUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "FooterTwitterUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "FooterYoutubeUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "GoogleMapEmbedUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "HeaderLogoUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "HeaderWhiteLogoUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "IndexAboutButtonText",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "IndexAboutDescription",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "IndexAboutImageUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "IndexAboutTitle",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "IndexContactButtonText",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "IndexContactDescription",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "IndexContactSubtitle",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "IndexContactTitle",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "IndexEntryDescription",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "BrochureUrl",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "GalleryUrls",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LongDescription",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "IndexStockVideoUrl",
                table: "SiteSettings",
                newName: "AboutSummary");

            migrationBuilder.RenameColumn(
                name: "IndexEntryTitle",
                table: "SiteSettings",
                newName: "AboutDetail");

            migrationBuilder.RenameColumn(
                name: "ShortDescription",
                table: "Products",
                newName: "Description");
        }
    }
}
