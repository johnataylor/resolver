﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolver.Metadata
{
    public interface IGallery
    {
        Registration GetRegistration(string id);
    }
}