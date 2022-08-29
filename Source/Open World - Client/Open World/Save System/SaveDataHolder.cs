using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorld
{
    [System.Serializable]
    public class MainDataHolder
    {
        public string ipText;
        public string portText;
        public string usernameText;

        public MainDataHolder(ParametersCache parameters)
        {
            ipText = parameters.ipText;
            portText = parameters.portText;
            usernameText = parameters.usernameText;
        }
    }
}
