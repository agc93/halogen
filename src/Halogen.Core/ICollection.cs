using System;
using System.Collections.Generic;

namespace Halogen.Core
{
    public interface ICollection
    {
        Guid Id { get; }
        string Name { get; set; }
        string ShortName { get; set; }
        List<HalogenId> Videos { get; set; }
    }
}