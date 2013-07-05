using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalGUI
{
    /// <summary>
    /// Model for a 'stateable' page
    /// </summary>
    public interface ISwitchable
    {
        void UtilizeState(object state);
    }
}
