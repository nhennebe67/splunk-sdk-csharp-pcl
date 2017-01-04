using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splunk.Client.Examples
{
    class StringValueConverter : global::Splunk.Client.ValueConverter<string>
    {
        public override string DefaultValue
        {
            get
            {
                return string.Empty;
            }
        }

        public override string Convert(Object input)
        {
            if (input != null)
                return input.ToString();

            return string.Empty;
        }

    }
}
