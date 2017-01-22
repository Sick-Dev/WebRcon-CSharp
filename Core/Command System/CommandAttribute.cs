﻿using System;

namespace WebRcon {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandAttribute : Attribute {
        public string description { get; set; }
        public string[] aliases { get; set; }
        
        public CommandAttribute() {}
    }
}