using System.Linq;

namespace PwC.Base.Common.Extensions
{
    public static class FrameworkExtensions
    {
        public static bool In<T>(this T @this, params T[] objects)
        {
            return objects?.Any(obj => Equals(@this, obj)) ?? false;
        }

        public static T[] Extend<T>(this T[] @this, params T[] objects)
        {
            if (@this == null)
            {
                return objects;
            }

            if (objects == null || objects.Length == 0)
            {
                return @this;
            }

            return @this.Concat(objects).ToArray();
        }
    }
}