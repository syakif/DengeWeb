namespace DengeWeb.Models
{
    public class SiteSetting
    {
        public int Id { get; set; }
        
        public string Language { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        //Favicon
        public string FaviconUrl { get; set; }
        
        //Layout header
        public string HeaderLogoUrl { get; set; }
        public string HeaderWhiteLogoUrl { get; set; }

        //Layout footer
        public string FooterLogoUrl { get; set; }
        public string FooterCopyrightText { get; set; }
        public string FooterInstagramUrl { get; set; }
        public string FooterFacebookUrl { get; set; }
        public string FooterTwitterUrl { get; set; }
        public string FooterLinkedinUrl { get; set; }
        public string FooterYoutubeUrl { get; set; }

        //Index page
        public string IndexStockVideoUrl { get; set; }
        public string IndexEntryTitle { get; set; }
        public string IndexEntryDescription { get; set; }

        public string IndexAboutTitle { get; set; }
        public string IndexAboutDescription { get; set; }
        public string IndexAboutImageUrl { get; set; }
        public string IndexAboutButtonText{ get; set; }

        public string IndexContactTitle { get; set; }
        public string IndexContactSubtitle { get; set; }
        public string IndexContactDescription { get; set; }
        public string IndexContactButtonText { get; set; }

        //About page
        public string AboutTitle { get; set; }
        public string AboutDescription { get; set; }
        public string AboutMissionVisionTitle{ get; set; }
        public string AboutMissionVisionDescription { get; set; }
        public string AboutMissionVisionImageUrl { get; set; }

        public string AboutEthicalValuesTitle{ get; set; }
        public string AboutEthicalValuesDescription { get; set; }

        public string AboutValuesTitle{ get; set; }
        public string AboutValuesDescription { get; set; }

        public string AboutValuesImageUrl{ get; set; }

        //Contact page
        public string ContactTitle { get; set; }
        public string GoogleMapEmbedUrl { get; set; }




        


        
        
        
    }
}