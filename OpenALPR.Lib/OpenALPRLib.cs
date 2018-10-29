using System;
using System.IO;
using System.Linq;
using System.Reflection;
using openalprnet;
using OpenALPR.Lib.Data;

namespace OpenALPR.Lib
{
    public class OpenAlprLib
    {
        /// <summary>
        /// Gets best license plate match for given image
        /// </summary>
        /// <param name="country">Country (US / EU)</param>
        /// <param name="imagePath">Image path</param>
        /// <returns>Best license plate match</returns>
        public string GetBestMatch(Country country, string imagePath)
        {
            var result = GetPlateNumberCandidates(country, imagePath);
            return result?.PlateNumber;
        }

        private static string FindCountry(Country country) => country.ToString().ToLower();

        /// <summary>
        /// Gets all license plate matches for their confidence for given image
        /// </summary>
        /// <param name="country">Country (US / EU)</param>
        /// <param name="imagePath">Image path</param>
        /// <returns>License plate matches for their confidence</returns>
        public Result GetPlateNumberCandidates(Country country, string imagePath)
        {
            var ass = Assembly.GetExecutingAssembly();
            var assemblyDirectory = Path.GetDirectoryName(ass.Location);
            if (assemblyDirectory == null)
            {
                throw new InvalidOperationException("Assembly Directory can't be null");
            }
            var configFile = Path.Combine(assemblyDirectory, "openalpr", "openalpr.conf");
            var runtimeDataDir = Path.Combine(assemblyDirectory, "openalpr", "runtime_data");
            var foundCountry = FindCountry(country);

            using (var alpr = new AlprNet(foundCountry, configFile, runtimeDataDir))
            {
                if (!alpr.IsLoaded())
                {
                    throw new Exception("Error initializing OpenALPR");
                }

                var results = alpr.Recognize(imagePath);
                var plates = results.Plates.SelectMany(l => l.TopNPlates.Select(plate => new Result
                {
                    PlateNumber = plate.Characters,
                    Confidence = plate.OverallConfidence
                })).ToList();

                return plates.FirstOrDefault(p => p.Confidence >= 83f);
            }
        }
    }
}