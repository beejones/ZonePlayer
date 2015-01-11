﻿//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonePlayer
{
    /// <summary>
    /// Enumeration of the supported players
    /// </summary>
    public enum PlayerType
    {
        /// <summary>
        /// vlc player
        /// </summary>
        vlc = 1,

        /// <summary>
        /// vlc player activeX control
        /// </summary>
        axVlc,

        /// <summary>
        /// wmp player
        /// </summary>
        wmp,

        /// <summary>
        /// wmp player activeX control
        /// </summary>
        axWmp,

        /// <summary>
        /// No player specified
        /// </summary>
        None
    }
}
