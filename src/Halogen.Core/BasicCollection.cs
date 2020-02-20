using System;
using System.Collections.Generic;
using System.Linq;

namespace Halogen.Core
{
    public class BasicCollection : ICollection
    {
        private BasicCollection()
        {

        }
        public static BasicCollection Create(string name, string shortName = null, IEnumerable<HalogenId> videos = null)
        {
            return new BasicCollection() { 
                Id = GuidUtility.Create(GuidUtility.HalogenNamespace, name),
                Name = name,
                ShortName = shortName ?? name,
                Videos = videos?.ToList() ?? new List<HalogenId>() };
        }
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public List<HalogenId> Videos { get; set; }
    }
}