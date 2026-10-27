using System;
using System.Collections.Generic;
using System.Text;

namespace Verkstadsloggen.Application
{
    public record MechanicWorkload(Guid? MechanicId, string Name, int NotStarted, int InProgress, int Completed)
    {
        public int Total => NotStarted + InProgress + Completed;
    }
}
