using System.Collections.Generic;
using WindowStay.Controller;

namespace WindowStay.Interfaces
{
    public interface IObserver
    {
        void Update(ProgramWindow value);
        void Update(List<ProgramWindow> value);
    }
}
