namespace Combat
{
    using System.Collections.Generic;
    using System.Linq;

    public interface IComponent { }
    public interface IAttachable { List<IComponent> components { get; } }

    public static class AttachableExtensions
    {
        public static T GetComponent<T>(this IAttachable attachable) where T : IComponent
        {
            return (T)attachable.components.First(component => component is T);
        }
        public static IEnumerable<T> GetComponents<T>(this IAttachable attachable) where T : IComponent
        {
            return attachable.components.Where(component => component is T).Cast<T>();
        }
    }
}
