using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Driver.Base
{
    public enum EndianType
    {
        AB, BA,
        ABCD, CDAB, BADC, DCBA,
        ABCDEFGH, GHEFCDAB, BADCFEHG, HGFEDCBA
    }
}
