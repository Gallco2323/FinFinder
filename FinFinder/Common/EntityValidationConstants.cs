using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Common
{
    public static class EntityValidationConstants
    {
        public static class ApplicationUser
        {
            public const int ProfilePictureUrlMaxLength = 200;
            public const int BioMaxLength = 500;
            public const int BioMinLength = 5;
            public const int UserNameMinLength = 2;
            public const int UserNameMaxLength = 50;
        }

        public static class FishCatch
        {
            public const int SpeciesMaxLength = 100;
            public const int SpeciesMinLength = 2;
            public const int DescriptionMaxLength = 500;
            public const int DescriptionMinLength = 20;
            
            public const double MinWeight = 0.1; // e.g., 0.1 kg
            public const double MaxWeight = 500; // e.g., max weight in kg
            public const double MinLength = 1; // e.g., 1 cm
            public const double MaxLength = 500; // e.g., max length in cm
            public const int LocationMinLength = 2;
            public const int LocationMaxLength = 150;

            public const string DateTimeFormat = "dd/MM/yyyy/";

        }

      

        public static class Observation
        {
            public const int NotesMaxLength = 1000;
            public const int NotesMinLength = 5;
            public const int BaitMaxLength = 100;
            public const int BaitMinLength = 5;
        }

        public static class Comment
        {
            public const int ContentMaxLength = 500;
            public const int ContentMinLength = 1;
        }

        public static class Favorite
        {
            // No specific validation constants for favorites.
        }

        public static class FishingTechnique
        {
            public const int NameMaxLength = 100;
            public const int NameMinLength = 3;
            public const int DescriptionMaxLength = 500;
            public const int DescriptionMinLength = 20;
        }

        public static class PhotoGallery
        {
            public const int PhotoUrlMaxLength = 200;
            public const int CaptionMaxLength = 300;
        }

    }
}
