using Microsoft.Xrm.Sdk;

namespace PwC.Base.Plugins.Common.Helpers
{
    /// <summary>
    /// Image helper generic class helps to manage images parameters
    /// from images collection from plugin context in plugin execution pipeline
    /// </summary>
    /// <typeparam name="T">Type of entity which is stored inside given image.</typeparam>
    internal static class ImageHelper<T>
        where T : Entity, new()
    {
        /// <summary>
        /// Gets entity image of a given name from images collection from plugin context.
        /// </summary>
        /// <param name="imageName">Name of the image from the context.</param>
        /// <param name="imagesCollection">Images collection from plugin context.</param>
        /// <param name="imageField">Image fields to reference to an image value. Afterwards if image is already loaded then image is returned from this field reference.</param>
        /// <param name="isImageFieldLoaded">if set to <c>true</c> then image value is returned from imageField reference parameter otherwise it is gathered from images collection and assigned to imageField.</param>
        /// <returns>Particular entity type object gathered from images collection.</returns>
        internal static T GetEntityImage(string imageName, EntityImageCollection imagesCollection, ref T imageField, ref bool isImageFieldLoaded)
        {
            if (!isImageFieldLoaded)
            {
                isImageFieldLoaded = true;
                if (imagesCollection.Contains(imageName))
                {
                    imageField = imagesCollection[imageName].ToEntity<T>();
                }
            }

            return imageField;
        }
    }
}
