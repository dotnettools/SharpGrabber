﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// A multi grabber is a grabber that combines multiple grabbers.
    /// </summary>
    public interface IMultiGrabber : IGrabber
    {
        /// <summary>
        /// Enumerates all registered grabbers on this multi grabber.
        /// </summary>
        IEnumerable<IGrabber> GetRegisteredGrabbers();

        /// <summary>
        /// Registers a grabber.
        /// </summary>
        void Register(IGrabber grabber);

        /// <summary>
        /// Unregisters a grabber.
        /// </summary>
        void Unregister(IGrabber grabber);
    }
}
