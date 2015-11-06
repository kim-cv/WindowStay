using System.Collections.Generic;
using WindowStay.Controller;

namespace WindowStay.Interfaces
{
    public interface IObserver
    {
        void Update(GetWindow value);
        void Update(List<GetWindow> value);
    }
}
